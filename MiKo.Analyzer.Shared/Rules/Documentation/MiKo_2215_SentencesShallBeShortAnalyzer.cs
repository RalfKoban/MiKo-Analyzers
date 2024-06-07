using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2215_SentencesShallBeShortAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2215";

        private const int MediumSentenceWords = 15; // 15-20 words are considered medium sentence length
        private const int LongSentenceWords = 30; // 30 and more words are considered long sentence length

        private static readonly char[] WordSeparators = ("/" + string.Concat(Constants.WhiteSpaces)).ToCharArray();

        private static readonly IEnumerable<string> XmlTags = new HashSet<string>
                                                                  {
                                                                      Constants.XmlTag.Summary,
                                                                      Constants.XmlTag.Remarks,
                                                                      Constants.XmlTag.Param,
                                                                      Constants.XmlTag.Returns,
                                                                      Constants.XmlTag.Value,
                                                                      Constants.XmlTag.Exception,
                                                                      Constants.XmlTag.TypeParam,
                                                                      Constants.XmlTag.Example,
                                                                      Constants.XmlTag.Note,
                                                                      Constants.XmlTag.Overloads,
                                                                      Constants.XmlTag.Para,
                                                                      Constants.XmlTag.Permission,
                                                                  };

        public MiKo_2215_SentencesShallBeShortAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            if (HasIssue(comment))
            {
                return new[] { Issue(symbol) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static bool HasIssue(DocumentationCommentTriviaSyntax comment)
        {
            foreach (var node in comment.DescendantNodes<XmlElementSyntax>().Where(_ => XmlTags.Contains(_.GetName())))
            {
                var commentBuilder = new StringBuilder();

                foreach (var syntax in node.DescendantNodes(_ => _.IsCode() is false, true).OfType<XmlTextSyntax>())
                {
                    commentBuilder.Append(' ').WithoutXmlCommentExterior(syntax).Append(' ');
                }

                var specificComment = commentBuilder.ToString();

                if (HasIssue(specificComment.AsSpan().Trim()))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasIssue(ReadOnlySpan<char> text)
        {
            foreach (var sentence in text.SplitBy(Constants.SentenceMarkers, StringSplitOptions.RemoveEmptyEntries))
            {
                if (SentenceHasIssue(sentence))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool SentenceHasIssue(ReadOnlySpan<char> sentence)
        {
            var words = sentence.SplitBy(WordSeparators, StringSplitOptions.RemoveEmptyEntries);
            var wordsLength = words.Count();

            if (wordsLength > MediumSentenceWords)
            {
                // maybe we have sentence clauses split by comma, so inspect each of them
                if (SentenceClauseHasIssue(sentence))
                {
                    return true;
                }

                if (wordsLength >= LongSentenceWords)
                {
                    // sentence is too long
                    return true;
                }
            }

            return false;
        }

        private static bool SentenceClauseHasIssue(ReadOnlySpan<char> sentence)
        {
            var clauses = sentence.SplitBy(Constants.SentenceClauseMarkers, StringSplitOptions.RemoveEmptyEntries);
            var count = clauses.Count();

            if (count > 1)
            {
                foreach (var clause in clauses)
                {
                    if (SentenceHasIssue(clause))
                    {
                        return true;
                    }
                }
            }

            // just a single sentence
            return false;
        }
    }
}
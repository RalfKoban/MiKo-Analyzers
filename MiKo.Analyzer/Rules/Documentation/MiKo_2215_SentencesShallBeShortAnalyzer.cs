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

        public MiKo_2215_SentencesShallBeShortAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            // inspect the comments
            var comment = symbol.GetSyntax().DescendantNodes(_ => true, true).OfType<DocumentationCommentTriviaSyntax>().First();
            var constructedComment = ConstructComment(comment);

            return HasIssue(constructedComment)
                       ? new[] { Issue(symbol) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static string ConstructComment(DocumentationCommentTriviaSyntax comment)
        {
            var builder = new StringBuilder();

            foreach (var text in comment.DescendantNodes(_ => _.IsCode() is false, true).OfType<XmlTextSyntax>())
            {
                builder.Append(' ').Append(text.WithoutXmlCommentExterior()).Append(' ');
            }

            return builder.ToString().Trim();
        }

        private static bool HasIssue(string text)
        {
            var sentences = text.Split(Constants.SentenceMarkers, StringSplitOptions.RemoveEmptyEntries);

            return sentences.Any(SentenceHasIssue);
        }

        private static bool SentenceHasIssue(string sentence)
        {
            var words = sentence.Replace("/", " ").Split(Constants.WhiteSpaceCharacters, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length > MediumSentenceWords)
            {
                // maybe we have sentence clauses split by comma, so inspect each of them
                if (SentenceClauseHasIssue(sentence))
                {
                    return true;
                }

                if (words.Length >= LongSentenceWords)
                {
                    // sentence is too long
                    return true;
                }
            }

            return false;
        }

        private static bool SentenceClauseHasIssue(string sentence)
        {
            var clauses = sentence.Split(Constants.SentenceClauseMarkers, StringSplitOptions.RemoveEmptyEntries);

            if (clauses.Length > 1)
            {
                return clauses.Any(SentenceHasIssue);
            }

            // just a single sentence
            return false;
        }
    }
}
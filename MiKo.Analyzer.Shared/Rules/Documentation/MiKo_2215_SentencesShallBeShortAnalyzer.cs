﻿using System;
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
                yield return Issue(symbol);
            }
        }

        private static bool HasIssue(DocumentationCommentTriviaSyntax comment)
        {
            var elements = comment.DescendantNodes<XmlElementSyntax>();
            var hasIssue = Analyze(elements, XmlTags);

            return hasIssue;
        }

        private static bool HasIssue(string text)
        {
            var sentences = text.Split(Constants.SentenceMarkers, StringSplitOptions.RemoveEmptyEntries);

            return sentences.Any(SentenceHasIssue);
        }

        // TODO RKN: rewerite this to avoid all the constructions of the strings, arrays and the other stuff
        private static bool Analyze(IEnumerable<XmlElementSyntax> nodes, IEnumerable<string> tagNames) => nodes.Where(_ => tagNames.Contains(_.GetName())).Select(ConstructComment).Any(HasIssue);

        private static string ConstructComment(SyntaxNode comment)
        {
            var builder = new StringBuilder();

            foreach (var text in comment.DescendantNodes(_ => _.IsCode() is false, true).OfType<XmlTextSyntax>())
            {
                builder.Append(' ').Append(text.WithoutXmlCommentExterior()).Append(' ');
            }

            return builder.ToString().Trim();
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
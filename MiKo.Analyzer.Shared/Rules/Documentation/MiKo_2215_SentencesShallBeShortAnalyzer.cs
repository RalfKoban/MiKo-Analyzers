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

        public MiKo_2215_SentencesShallBeShortAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            if (HasIssue(comment))
            {
                yield return Issue(symbol);
            }
        }

        private static bool HasIssue(DocumentationCommentTriviaSyntax comment)
        {
            if (comment is null)
            {
                // it might be that there is no documentation comment available
                return false;
            }

            var elements = comment.DescendantNodes<XmlElementSyntax>().ToList();

            var hasIssue = Analyze(elements, Constants.XmlTag.Summary)
                        || Analyze(elements, Constants.XmlTag.Remarks)
                        || Analyze(elements, Constants.XmlTag.Param)
                        || Analyze(elements, Constants.XmlTag.Returns)
                        || Analyze(elements, Constants.XmlTag.Value)
                        || Analyze(elements, Constants.XmlTag.Exception)
                        || Analyze(elements, Constants.XmlTag.TypeParam)
                        || Analyze(elements, Constants.XmlTag.Example)
                        || Analyze(elements, Constants.XmlTag.Note)
                        || Analyze(elements, Constants.XmlTag.Overloads)
                        || Analyze(elements, Constants.XmlTag.Para)
                        || Analyze(elements, Constants.XmlTag.Permission);

            return hasIssue;
        }

        private static bool HasIssue(string text)
        {
            var sentences = text.Split(Constants.SemiSentenceMarkers, StringSplitOptions.RemoveEmptyEntries);

            return sentences.Any(SentenceHasIssue);
        }

        private static bool Analyze(IEnumerable<XmlElementSyntax> nodes, string tagName) => nodes.Where(_ => _.GetName() == tagName).Select(ConstructComment).Any(HasIssue);

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
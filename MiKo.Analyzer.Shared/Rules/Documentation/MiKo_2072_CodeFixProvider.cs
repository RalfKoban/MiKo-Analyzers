using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2072_CodeFixProvider)), Shared]
    public sealed class MiKo_2072_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => "MiKo_2072";

        protected override string Title => Resources.MiKo_2072_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            return Comment(comment, ReplacementMap.Keys, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            const string SyncPhrase = MiKo_2072_TrySummaryAnalyzer.StartingPhrase + " ";

            var lowerCasePhrase = SyncPhrase.ToLowerCaseAt(0);
            var asyncPhrase = Constants.Comments.AsynchrounouslyStartingPhrase + lowerCasePhrase;

            var result = new Dictionary<string, string>();

            foreach (var startingWord in MiKo_2072_TrySummaryAnalyzer.Words)
            {
                var phrase = startingWord + " ";
                var alternativePhrase = startingWord + " to ";

                result.Add(phrase, SyncPhrase);
                result.Add(alternativePhrase, SyncPhrase);

                result.Add(Constants.Comments.AsynchrounouslyStartingPhrase + phrase, asyncPhrase);
                result.Add(Constants.Comments.AsynchrounouslyStartingPhrase + alternativePhrase, asyncPhrase);

                result.Add(phrase.ToLowerCaseAt(0), lowerCasePhrase);
                result.Add(alternativePhrase.ToLowerCaseAt(0), lowerCasePhrase);
            }

            // fix the wrong replacements (such as "Tries to" which was replaced into "Attempts to to" due to only first word was replaced)
            result.Add(" to to ", " to ");

            return result;
        }
    }
}
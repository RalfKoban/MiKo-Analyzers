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
//// ncrunch: rdi off

        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2072";

        protected override string Title => Resources.MiKo_2072_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            return Comment(comment, ReplacementMap.Keys, ReplacementMap);
        }

//// ncrunch: rdi off

        private static Dictionary<string, string> CreateReplacementMap()
        {
            const string SyncPhrase = Constants.Comments.TryStartingPhrase + " ";

            var lowerCasePhrase = SyncPhrase.ToLowerCaseAt(0);
            var asyncPhrase = Constants.Comments.AsynchronouslyStartingPhrase + lowerCasePhrase;

            var result = new Dictionary<string, string>();

            foreach (var startingWord in Constants.Comments.TryWords)
            {
                var phrase = startingWord + " ";
                var alternativePhrase = startingWord + " to ";

                result.Add(phrase, SyncPhrase);
                result.Add(alternativePhrase, SyncPhrase);

                result.Add(Constants.Comments.AsynchronouslyStartingPhrase + phrase, asyncPhrase);
                result.Add(Constants.Comments.AsynchronouslyStartingPhrase + alternativePhrase, asyncPhrase);

                result.Add(phrase.ToLowerCaseAt(0), lowerCasePhrase);
                result.Add(alternativePhrase.ToLowerCaseAt(0), lowerCasePhrase);
            }

            // fix the wrong replacements (such as "Tries to" which was replaced into "Attempts to to" due to only first word was replaced)
            result.Add(" to to ", " to ");

            return result;
        }

//// ncrunch: rdi off
    }
}
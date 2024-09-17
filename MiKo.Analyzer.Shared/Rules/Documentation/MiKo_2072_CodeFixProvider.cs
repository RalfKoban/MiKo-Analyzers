using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2072_CodeFixProvider)), Shared]
    public sealed class MiKo_2072_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly KeyValuePair<string, string>[] ReplacementMap = CreateReplacementMap().ToArray();
        private static readonly string[] ReplacementMapKeys = ReplacementMap.Select(_ => _.Key).ToArray();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2072";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            return Comment(comment, ReplacementMapKeys, ReplacementMap);
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
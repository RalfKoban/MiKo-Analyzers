using System;
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

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();
        private static readonly string[] ReplacementMapKeys = ReplacementMap.ToArray(_ => _.Key);

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2072";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            return Comment(comment, ReplacementMapKeys, ReplacementMap);
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            const string SyncPhrase = Constants.Comments.TryStartingPhrase + " ";

            var lowerCasePhrase = SyncPhrase.ToLowerCaseAt(0);
            var asyncPhrase = Constants.Comments.AsynchronouslyStartingPhrase + lowerCasePhrase;

            var startingWords = Constants.Comments.TryWords;

            var result = new Pair[1 + (6 * startingWords.Length)];
            var resultIndex = 0;

            foreach (var startingWord in startingWords)
            {
                var phrase = startingWord + " ";
                var alternativePhrase = startingWord + " to ";

                result[resultIndex++] = new Pair(phrase, SyncPhrase);
                result[resultIndex++] = new Pair(alternativePhrase, SyncPhrase);

                result[resultIndex++] = new Pair(Constants.Comments.AsynchronouslyStartingPhrase + phrase, asyncPhrase);
                result[resultIndex++] = new Pair(Constants.Comments.AsynchronouslyStartingPhrase + alternativePhrase, asyncPhrase);

                result[resultIndex++] = new Pair(phrase.ToLowerCaseAt(0), lowerCasePhrase);
                result[resultIndex++] = new Pair(alternativePhrase.ToLowerCaseAt(0), lowerCasePhrase);
            }

            // fix the wrong replacements (such as "Tries to" which was replaced into "Attempts to to" due to only first word was replaced)
            result[resultIndex] = new Pair(" to to ", " to ");

            return result;
        }

//// ncrunch: rdi off
    }
}
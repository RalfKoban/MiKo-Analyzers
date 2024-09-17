using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2203_CodeFixProvider)), Shared]
    public sealed class MiKo_2203_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2203";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, Constants.Comments.GuidTermsWithDelimiters, ReplacementMap);
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var terms = Constants.Comments.GuidTermsWithDelimiters;
            var result = new Pair[2 * terms.Length];
            var resultIndex = 0;

            foreach (var term in terms)
            {
                var termWithA = "A " + term.TrimStart();

                var replacement = term.ToUpperInvariant().Replace("GUID", "unique identifier");
                var replacementWithA = "An " + replacement.TrimStart();

                result[resultIndex++] = new Pair(termWithA, replacementWithA);
                result[resultIndex++] = new Pair(term, replacement);
            }

            return result;
        }

//// ncrunch: rdi default
    }
}
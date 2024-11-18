using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2222_CodeFixProvider)), Shared]
    public sealed class MiKo_2222_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2222";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, Constants.Comments.IdentTerms, ReplacementMap);
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var terms = Constants.Comments.IdentTerms;

            var result = new Pair[3 * terms.Length];
            var resultIndex = 0;

            foreach (var term in terms)
            {
                var replacement = term.Replace(Constants.Comments.IdentTerm, "identification");
                result[resultIndex++] = new Pair(term, replacement);

                // alternative 1
                result[resultIndex++] = new Pair(term.Replace('i', 'I'), replacement);

                // alternative 2
                result[resultIndex++] = new Pair(term.ToUpperCase(), replacement);
            }

            return result;
        }

//// ncrunch: rdi default
    }
}
using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2210_CodeFixProvider)), Shared]
    public sealed class MiKo_2210_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2210";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, Constants.Comments.InfoTerms, ReplacementMap);
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var terms = Constants.Comments.InfoTerms;

            var result = new Pair[2 * terms.Length];
            var resultIndex = 0;

            foreach (var term in terms)
            {
                var replacement = term.Replace(Constants.Comments.InfoTerm, "information");
                result[resultIndex++] = new Pair(term, replacement);

                var alternative = term.Replace('i', 'I');
                result[resultIndex++] = new Pair(alternative, replacement);
            }

            return result;
        }

//// ncrunch: rdi default
    }
}
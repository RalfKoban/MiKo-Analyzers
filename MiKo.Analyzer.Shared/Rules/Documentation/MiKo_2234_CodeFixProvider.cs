using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2234_CodeFixProvider)), Shared]
    public sealed class MiKo_2234_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => "MiKo_2234";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic issue)
        {
            return Comment(syntax, Constants.Comments.WhichIsToTerms, ReplacementMap);
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var terms = Constants.Comments.WhichIsToTerms;
            var termsLength = terms.Length;

            var result = new Pair[termsLength];

            for (var index = 0; index < termsLength; index++)
            {
                result[index] = new Pair(terms[index], Constants.Comments.ToTerm);
            }

            return result;
        }

//// ncrunch: rdi default
    }
}
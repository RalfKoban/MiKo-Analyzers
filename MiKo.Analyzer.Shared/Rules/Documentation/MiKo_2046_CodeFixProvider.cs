using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2046_CodeFixProvider)), Shared]
    public sealed class MiKo_2046_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2046";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is null)
            {
                return null;
            }

            var name = syntax.GetReferencedName();

            return TypeParamRef(name);
        }
    }
}
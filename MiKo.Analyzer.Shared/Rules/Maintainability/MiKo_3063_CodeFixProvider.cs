using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3063_CodeFixProvider)), Shared]
    public sealed class MiKo_3063_CodeFixProvider : StringMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3063";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => GetUpdatedSyntaxWithTextEnding(syntax, ".");
    }
}
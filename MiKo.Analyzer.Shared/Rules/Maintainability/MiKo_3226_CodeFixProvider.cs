using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3226_CodeFixProvider)), Shared]
    public sealed class MiKo_3226_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3226";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<FieldDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is FieldDeclarationSyntax field)
            {
                var modifiers = field.Modifiers;
                var readonlyModifier = modifiers.First(SyntaxKind.ReadOnlyKeyword);
                var updatedModifiers = modifiers.Replace(readonlyModifier, SyntaxKind.ConstKeyword.AsToken());

                var staticModifier = updatedModifiers.First(SyntaxKind.StaticKeyword);

                return field.WithModifiers(staticModifier.IsDefaultValue() ? updatedModifiers : updatedModifiers.Remove(staticModifier));
            }

            return syntax;
        }
    }
}
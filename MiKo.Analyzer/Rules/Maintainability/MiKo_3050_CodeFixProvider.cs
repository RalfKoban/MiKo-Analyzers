using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3050_CodeFixProvider)), Shared]
    public sealed class MiKo_3050_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3050_DependencyPropertyPublicStaticReadOnlyFieldAnalyzer.Id;

        protected override string Title => Resources.MiKo_3050_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<FieldDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var field = (FieldDeclarationSyntax)syntax;
            var modifiers = SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

            return field.WithModifiers(modifiers);
        }
    }
}
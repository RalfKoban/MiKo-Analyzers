using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3075_CodeFixProvider)), Shared]
    public sealed class MiKo_3075_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3075_NonPublicClassesShouldPreventInheritanceAnalyzer.Id;

        protected override string Title => Resources.MiKo_3075_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ClassDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var declaration = (ClassDeclarationSyntax)syntax;

            var keyword = MakeStatic(context, declaration)
                            ? SyntaxKind.StaticKeyword
                            : SyntaxKind.SealedKeyword;

            return declaration.AddModifiers(SyntaxFactory.Token(keyword));
        }

        private static bool MakeStatic(CodeFixContext context, ClassDeclarationSyntax syntax)
        {
            var semanticModel = GetSemanticModel(context);
            var symbol = syntax.GetTypeSymbol(semanticModel);

            // Inspect members, if all are static, then make it static, else make it sealed
            if (symbol.BaseType is null || symbol.BaseType.IsObject())
            {
                return symbol.GetMembers().All(_ => _.IsStatic || _.GetSyntax() is null);
            }

            return false;
        }
    }
}
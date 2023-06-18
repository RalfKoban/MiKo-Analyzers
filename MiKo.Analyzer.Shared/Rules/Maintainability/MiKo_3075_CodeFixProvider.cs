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

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var syntaxNode in syntaxNodes)
            {
                switch (syntaxNode)
                {
                    case ClassDeclarationSyntax c: return c;
                    case RecordDeclarationSyntax r: return r;
                    default:
                        return null;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case ClassDeclarationSyntax classDeclaration:
                {
                    var keyword = MakeStatic(document, classDeclaration)
                                  ? SyntaxKind.StaticKeyword
                                  : SyntaxKind.SealedKeyword;

                    var modifiers = CreateModifiers(classDeclaration, keyword);

                    return classDeclaration.WithModifiers(modifiers);
                }

                case RecordDeclarationSyntax recordDeclaration:
                {
                    var modifiers = CreateModifiers(recordDeclaration, SyntaxKind.SealedKeyword);

                    return recordDeclaration.WithModifiers(modifiers);
                }

                default:
                {
                    return syntax;
                }
            }
        }

        private static bool MakeStatic(Document document, ClassDeclarationSyntax syntax)
        {
            var type = syntax.GetTypeSymbol(GetSemanticModel(document));

            // Inspect members, if all are static, then make it static, else make it sealed
            if (type.BaseType is null || type.BaseType.IsObject())
            {
                return type.GetMembers().All(_ => _.IsStatic || _.GetSyntax() is null);
            }

            return false;
        }

        private static SyntaxTokenList CreateModifiers(MemberDeclarationSyntax declaration, SyntaxKind keyword)
        {
            var modifiers = declaration.Modifiers;
            var position = modifiers.IndexOf(SyntaxKind.PartialKeyword);

            var syntaxToken = SyntaxFactory.Token(keyword);

            return position > -1
                   ? modifiers.Insert(position, syntaxToken)
                   : modifiers.Add(syntaxToken);
        }
    }
}
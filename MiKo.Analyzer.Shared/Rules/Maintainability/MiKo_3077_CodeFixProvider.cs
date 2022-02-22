using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3077_CodeFixProvider)), Shared]
    public sealed class MiKo_3077_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3077_EnumPropertyHasDefaultValueAnalyzer.Id;

        protected override string Title => Resources.MiKo_3077_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<PropertyDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic issue)
        {
            var propertySyntax = (PropertyDeclarationSyntax)syntax;

            // fix auto getter and setter
            var accessors = propertySyntax.AccessorList?.Accessors;
            if (accessors?.Count == 2 && accessors.Value[0].Body is null && accessors.Value[0].ExpressionBody is null && accessors.Value[1].Body is null && accessors.Value[1].ExpressionBody is null)
            {
                // append a semicolon to the end
                var initializer = CreateInitializer(context, propertySyntax.Type);
                var updatedNode = propertySyntax.WithInitializer(initializer).WithSemicolonToken(";".ToSyntaxToken(SyntaxKind.SemicolonToken));

                return root.ReplaceNode(propertySyntax, updatedNode);
            }

            // fix backing fields (such as arrow clause or normal return statements)
            var identifierName = MiKo_3077_EnumPropertyHasDefaultValueAnalyzer.GetIdentifierNameFromPropertyExpression(propertySyntax);
            if (identifierName != null)
            {
                var classDeclarationSyntax = syntax.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                if (classDeclarationSyntax != null)
                {
                    foreach (var field in classDeclarationSyntax.DescendantNodes<FieldDeclarationSyntax>())
                    {
                        var declaration = field.Declaration;

                        foreach (var variable in declaration.Variables)
                        {
                            if (variable.GetName() == identifierName)
                            {
                                var initializer = CreateInitializer(context, declaration.Type);
                                var updatedNode = variable.WithInitializer(initializer);

                                return root.ReplaceNode(variable, updatedNode);
                            }
                        }
                    }
                }
            }

            return root;
        }

        private static EqualsValueClauseSyntax CreateInitializer(CodeFixContext context, TypeSyntax typeSyntax)
        {
            var type = typeSyntax.GetTypeSymbol(GetSemanticModel(context));

            var memberAccess = SimpleMemberAccess(type.Name, type.GetFields().First().Name);

            return SyntaxFactory.EqualsValueClause(memberAccess);
        }
    }
}
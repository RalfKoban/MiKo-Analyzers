using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3077_CodeFixProvider)), Shared]
    public sealed class MiKo_3077_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3077";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<PropertyDeclarationSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult(syntax);
        }

        protected override async Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var propertySyntax = (PropertyDeclarationSyntax)syntax;

            // fix auto getter and setter
            var accessors = propertySyntax.AccessorList?.Accessors;

            if (accessors?.Count is 2 && accessors.Value[0].Body is null && accessors.Value[0].ExpressionBody is null && accessors.Value[1].Body is null && accessors.Value[1].ExpressionBody is null)
            {
                // append a semicolon to the end
                var initializer = await CreateInitializerAsync(propertySyntax.Type, document, cancellationToken).ConfigureAwait(false);

                var updatedNode = propertySyntax.WithInitializer(initializer).WithSemicolonToken();

                return root.ReplaceNode(propertySyntax, updatedNode);
            }

            // fix backing fields (such as arrow clause or normal return statements)
            var identifierName = propertySyntax.GetIdentifierNameFromPropertyExpression();

            if (identifierName != null)
            {
                var classDeclarationSyntax = syntax.FirstAncestorOrSelf<ClassDeclarationSyntax>();

                if (classDeclarationSyntax != null)
                {
                    foreach (var field in classDeclarationSyntax.ChildNodes<FieldDeclarationSyntax>())
                    {
                        var declaration = field.Declaration;

                        foreach (var variable in declaration.Variables)
                        {
                            if (variable.GetName() == identifierName)
                            {
                                var initializer = await CreateInitializerAsync(declaration.Type, document, cancellationToken).ConfigureAwait(false);

                                var updatedNode = variable.WithInitializer(initializer);

                                return root.ReplaceNode(variable, updatedNode);
                            }
                        }
                    }
                }
            }

            return root;
        }

        private static async Task<EqualsValueClauseSyntax> CreateInitializerAsync(TypeSyntax typeSyntax, Document document, CancellationToken cancellationToken)
        {
            var type = await typeSyntax.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            var memberAccess = Member(type.Name, type.GetFields()[0].Name);

            return SyntaxFactory.EqualsValueClause(memberAccess);
        }
    }
}
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3054_CodeFixProvider)), Shared]
    public sealed class MiKo_3054_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private static readonly SyntaxKind[] PublicStaticReadOnly = { SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword };

        public override string FixableDiagnosticId => "MiKo_3054";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            var declarator = (VariableDeclaratorSyntax)syntax;

            var prefix = ExtractFieldNamePrefix(declarator);
            var identifier = prefix + Constants.DependencyProperty.FieldSuffix;

            var assignment = SimpleMemberAccess(declarator.Identifier.Text, Constants.DependencyProperty.TypeName);
            var variableDeclarator = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(identifier), default, SyntaxFactory.EqualsValueClause(assignment));

            var type = SyntaxFactory.ParseTypeName(Constants.DependencyProperty.TypeName);
            var variableDeclaration = SyntaxFactory.VariableDeclaration(type, variableDeclarator.ToSeparatedSyntaxList());

            var field = SyntaxFactory.FieldDeclaration(default, PublicStaticReadOnly.ToTokenList(), variableDeclaration);

            var originalField = syntax.FirstAncestorOrSelf<FieldDeclarationSyntax>();

            return root.InsertNodeAfter(originalField, field);
        }

        private static string ExtractFieldNamePrefix(VariableDeclaratorSyntax declarator)
        {
            if (declarator.Initializer?.Value is InvocationExpressionSyntax invocation)
            {
                var arguments = invocation.ArgumentList.Arguments;

                if (arguments.Count > 1)
                {
                    switch (arguments[0].Expression)
                    {
                        case InvocationExpressionSyntax nameofExpression: // first argument is nameof or string
                        {
                            var args = nameofExpression.ArgumentList.Arguments;

                            if (args.Count == 1 && args[0].Expression is IdentifierNameSyntax identifier)
                            {
                                return identifier.GetName();
                            }

                            break;
                        }

                        case LiteralExpressionSyntax literal:
                            return literal.GetName();
                    }
                }
            }

            return null;
        }
    }
}
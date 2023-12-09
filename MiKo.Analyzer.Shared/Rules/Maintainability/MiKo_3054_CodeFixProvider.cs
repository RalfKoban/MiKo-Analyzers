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
        public override string FixableDiagnosticId => MiKo_3054_DependencyPropertyKeyUsageOfDependencyPropertyAnalyzer.Id;

        protected override string Title => Resources.MiKo_3054_CodeFixTitle;

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
            var variableDeclaration = SyntaxFactory.VariableDeclaration(type, new[] { variableDeclarator }.ToSeparatedSyntaxList());

            var field = SyntaxFactory.FieldDeclaration(
                                                   default,
                                                   SyntaxFactory.TokenList(SyntaxKind.PublicKeyword.AsToken(), SyntaxKind.StaticKeyword.AsToken(), SyntaxKind.ReadOnlyKeyword.AsToken()),
                                                   variableDeclaration);

            var originalField = syntax.FirstAncestorOrSelf<FieldDeclarationSyntax>();

            return root.InsertNodeAfter(originalField, field);
        }

        private static string ExtractFieldNamePrefix(VariableDeclaratorSyntax declarator)
        {
            if (declarator.Initializer?.Value is InvocationExpressionSyntax ies)
            {
                var arguments = ies.ArgumentList.Arguments;

                if (arguments.Count > 1)
                {
                    // first argument is nameof or string
                    if (arguments[0].Expression is InvocationExpressionSyntax nameofExpressionSyntax)
                    {
                        var args = nameofExpressionSyntax.ArgumentList.Arguments;

                        if (args.Count == 1 && args[0].Expression is IdentifierNameSyntax ins)
                        {
                            return ins.GetName();
                        }
                    }
                    else if (arguments[0].Expression is LiteralExpressionSyntax literal)
                    {
                        return literal.GetName();
                    }
                }
            }

            return null;
        }
    }
}
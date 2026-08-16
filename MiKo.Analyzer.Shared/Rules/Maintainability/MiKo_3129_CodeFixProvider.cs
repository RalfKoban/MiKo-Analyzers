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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3129_CodeFixProvider)), Shared]
    public sealed class MiKo_3129_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3129";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ArgumentSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken) => Task.FromResult(syntax);

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedRoot = GetUpdatedSyntaxRoot(root, syntax);

            return Task.FromResult(updatedRoot);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax)
        {
            if (syntax is ArgumentSyntax argument)
            {
                var statement = LocalVariable("awaitedResult", argument.Expression).WithTrailingEmptyLine();
                var assertion = argument.FirstAncestor<ExpressionStatementSyntax>();

                if (assertion is null)
                {
                    // we seem to be part of an expression body
                    var arrowClause = argument.FirstAncestor<ArrowExpressionClauseSyntax>();
                    var method = arrowClause.FirstAncestor<MethodDeclarationSyntax>();

                    var updatedAssertion = SyntaxFactory.ExpressionStatement(arrowClause.Expression.ReplaceNode(argument, argument.WithExpression(IdentifierName("awaitedResult"))));

                    var updatedMethod = method.WithoutExpressionBody().WithBody(SyntaxFactory.Block(statement, updatedAssertion));

                    return root.ReplaceNode(method, updatedMethod);
                }
                else
                {
                    var updatedAssertion = assertion.ReplaceNode(argument, argument.WithExpression(IdentifierName("awaitedResult")));

                    return root.ReplaceNode(assertion, new SyntaxNode[] { statement, updatedAssertion });
                }
            }

            return root;
        }
    }
}
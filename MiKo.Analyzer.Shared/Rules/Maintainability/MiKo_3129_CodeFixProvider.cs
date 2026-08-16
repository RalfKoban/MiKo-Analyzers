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
        private const string VariableName = "awaitedResult";

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
                var addedLocalVariable = LocalVariable(VariableName, argument.Expression).WithTrailingEmptyLine();
                var assertion = argument.FirstAncestor<ExpressionStatementSyntax>();

                return assertion is null // we seem to be part of an expression body
                       ? GetUpdatedSyntaxRootForExpressionBody(root, argument, addedLocalVariable)
                       : GetUpdatedSyntaxRootForBody(root, assertion, argument, addedLocalVariable);
            }

            return root;
        }

        private static SyntaxNode GetUpdatedSyntaxRootForExpressionBody(SyntaxNode root, ArgumentSyntax argument, LocalDeclarationStatementSyntax addedLocalVariable)
        {
            var arrowClause = argument.FirstAncestor<ArrowExpressionClauseSyntax>();
            var method = arrowClause.FirstAncestor<MethodDeclarationSyntax>();

            var updatedAssertion = SyntaxFactory.ExpressionStatement(arrowClause.Expression.ReplaceNode(argument, argument.WithExpression(IdentifierName(VariableName))));

            return root.ReplaceNode(method, method.WithoutExpressionBody()
                                                  .WithBody(SyntaxFactory.Block(addedLocalVariable, updatedAssertion)));
        }

        private static SyntaxNode GetUpdatedSyntaxRootForBody(SyntaxNode root, ExpressionStatementSyntax assertion, ArgumentSyntax argument, LocalDeclarationStatementSyntax addedLocalVariable)
        {
            var updatedAssertion = assertion.ReplaceNode(argument, argument.WithExpression(IdentifierName(VariableName)));

            return assertion.PreviousSibling() is null
                   ? root.ReplaceNode(assertion, new SyntaxNode[] { addedLocalVariable, updatedAssertion })
                   : root.ReplaceNode(assertion, new SyntaxNode[] { addedLocalVariable.WithTriviaFrom(assertion), updatedAssertion });
        }
    }
}
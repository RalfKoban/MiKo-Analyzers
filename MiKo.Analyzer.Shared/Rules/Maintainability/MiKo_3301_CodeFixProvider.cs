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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3301_CodeFixProvider)), Shared]
    public sealed class MiKo_3301_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3301";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParenthesizedLambdaExpressionSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var parenthesized = (ParenthesizedLambdaExpressionSyntax)syntax;

            var body = GetBody(parenthesized);

            if (body is null)
            {
                // we cannot fix it
                return parenthesized;
            }

            var parameterList = parenthesized.ParameterList;
            var parameters = parameterList.Parameters;

            if (parameters.Count is 1)
            {
                return SyntaxFactory.SimpleLambdaExpression(parameters.First(), body).WithModifiers(parenthesized.Modifiers);
            }

            return SyntaxFactory.ParenthesizedLambdaExpression(parameterList, body).WithModifiers(parenthesized.Modifiers);
        }

        private static CSharpSyntaxNode GetBody(ParenthesizedLambdaExpressionSyntax node)
        {
            var body = node.Block.FirstChild<CSharpSyntaxNode>();

            switch (body)
            {
                case ReturnStatementSyntax statement:
                    return statement.Expression;

                case ExpressionStatementSyntax statement:
                    return statement.Expression;

                case ThrowStatementSyntax statement when statement.Expression != null:
                    return SyntaxFactory.ThrowExpression(statement.Expression);

                default:
                    return body;
            }
        }
    }
}
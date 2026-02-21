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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3231_CodeFixProvider)), Shared]
    public sealed class MiKo_3231_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3231";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var invocation = syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

            if (invocation?.Parent is PrefixUnaryExpressionSyntax unary && unary.IsKind(SyntaxKind.LogicalNotExpression))
            {
                return unary;
            }

            return invocation;
        }

        protected override async Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            if (syntax is PrefixUnaryExpressionSyntax unary)
            {
                var updatedSyntax = await GetUpdatedSyntaxAsync(unary.Operand, document, cancellationToken).ConfigureAwait(false);

                return updatedSyntax is null ? null : IsFalsePattern(updatedSyntax).WithTriviaFrom(unary);
            }

            return await GetUpdatedSyntaxAsync(syntax, document, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IsPatternExpressionSyntax> GetUpdatedSyntaxAsync(SyntaxNode syntax, Document document, CancellationToken cancellationToken)
        {
            if (syntax is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax maes)
            {
                var updatedSyntax = await GetUpdatedSyntaxAsync(maes.Expression, invocation.ArgumentList.Arguments, document, cancellationToken).ConfigureAwait(false);

                if (updatedSyntax != null)
                {
                    return updatedSyntax.WithTriviaFrom(invocation);
                }
            }

            return null;
        }

        private static async Task<IsPatternExpressionSyntax> GetUpdatedSyntaxAsync(ExpressionSyntax expression, SeparatedSyntaxList<ArgumentSyntax> arguments, Document document, CancellationToken cancellationToken)
        {
            var argument = arguments[0];
            var argumentExpression = argument.Expression;

            if (argumentExpression.IsKind(SyntaxKind.StringLiteralExpression))
            {
                return IsPattern(expression, argumentExpression);
            }

            if (expression.IsKind(SyntaxKind.StringLiteralExpression))
            {
                return IsPattern(argumentExpression, expression);
            }

            if (argumentExpression.IsNameOf())
            {
                return IsPattern(expression, argumentExpression);
            }

            if (expression.IsNameOf())
            {
                return IsPattern(argumentExpression, expression);
            }

            var isConst = await argument.IsConstAsync(document, cancellationToken).ConfigureAwait(false);

            if (isConst)
            {
                return IsPattern(expression, argumentExpression);
            }

            return IsPattern(argumentExpression, expression);
        }
    }
}
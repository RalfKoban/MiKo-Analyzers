using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UsePatternMatchingCodeFixProvider : MaintainabilityCodeFixProvider
    {
        private readonly SyntaxKind m_syntaxKind;

        protected UsePatternMatchingCodeFixProvider(in SyntaxKind syntaxKind = SyntaxKind.EqualsExpression) => m_syntaxKind = syntaxKind;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.First(_ => _.IsKind(m_syntaxKind));

        protected sealed override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = syntax;

            if (syntax is BinaryExpressionSyntax binary)
            {
                updatedSyntax = GetUpdatedSyntax(binary);
            }

            if (syntax is IsPatternExpressionSyntax pattern)
            {
                updatedSyntax = GetUpdatedPatternSyntax(pattern);
            }

            return Task.FromResult(updatedSyntax);
        }

        protected virtual IsPatternExpressionSyntax GetUpdatedPatternSyntax(IsPatternExpressionSyntax pattern) => pattern;

        protected virtual IsPatternExpressionSyntax GetUpdatedPatternSyntax(ExpressionSyntax operand, ExpressionSyntax expression) => IsPattern(operand, expression);

        private static bool SwapSides(ExpressionSyntax operand, ExpressionSyntax expression)
        {
            if (expression is LiteralExpressionSyntax)
            {
                return false; // we are already on the correct part
            }

            if (operand is LiteralExpressionSyntax)
            {
                return true; // literal is on wrong side, so swap sides
            }

            if (expression is PrefixUnaryExpressionSyntax)
            {
                return false; // we are already on the correct part
            }

            if (operand is PrefixUnaryExpressionSyntax)
            {
                return true; // literal is on wrong side, so swap sides
            }

            // we probably have an enum
            if (expression is MemberAccessExpressionSyntax)
            {
                return false; // we are already on the correct sides
            }

            if (operand is MemberAccessExpressionSyntax)
            {
                return true; // enum is on wrong side, so swap sides
            }

            // we probably have a 'nameof'
            if (expression is InvocationExpressionSyntax right && right.IsNameOf())
            {
                return false; // we are already on the correct sides
            }

            if (operand is InvocationExpressionSyntax left && left.IsNameOf())
            {
                return true; // enum is on wrong side, so swap sides
            }

            return false;
        }

        private IsPatternExpressionSyntax GetUpdatedSyntax(BinaryExpressionSyntax binary)
        {
            var operand = binary.Left;
            var expression = binary.Right;

            if (SwapSides(operand, expression))
            {
                var temp = operand;

                operand = expression.WithTriviaFrom(operand);
                expression = temp.WithTriviaFrom(expression)
                                 .WithoutTrailingSpaces();
            }

            return GetUpdatedPatternSyntax(operand, expression);
        }
    }
}
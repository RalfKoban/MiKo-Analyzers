using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UsePatternMatchingCodeFixProvider : MaintainabilityCodeFixProvider
    {
        private readonly SyntaxKind m_syntaxKind;

        protected UsePatternMatchingCodeFixProvider(in SyntaxKind syntaxKind = SyntaxKind.EqualsExpression) => m_syntaxKind = syntaxKind;

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.First(_ => _.IsKind(m_syntaxKind));

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var binary = (BinaryExpressionSyntax)syntax;

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

            return false;
        }
    }
}
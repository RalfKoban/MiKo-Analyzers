using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is PrefixUnaryExpressionSyntax unary)
            {
                var updatedSyntax = GetUpdatedSyntax(document, unary.Operand);

                return updatedSyntax is null ? null : IsFalsePattern(updatedSyntax).WithTriviaFrom(unary);
            }

            return GetUpdatedSyntax(document, syntax);
        }

        private static IsPatternExpressionSyntax GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            if (syntax is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax maes)
            {
                var updatedSyntax = GetUpdatedSyntax(document, maes.Expression, invocation.ArgumentList.Arguments);

                if (updatedSyntax != null)
                {
                    return updatedSyntax.WithTriviaFrom(invocation);
                }
            }

            return null;
        }

        private static IsPatternExpressionSyntax GetUpdatedSyntax(Document document, ExpressionSyntax expression, in SeparatedSyntaxList<ArgumentSyntax> arguments)
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

            if (argument.IsConst(document))
            {
                return IsPattern(expression, argumentExpression);
            }

            return IsPattern(argumentExpression, expression);
        }
    }
}
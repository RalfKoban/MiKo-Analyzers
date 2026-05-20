using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3234_CodeFixProvider)), Shared]
    public sealed class MiKo_3234_CodeFixProvider : UsePatternMatchingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3234";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                if (node is IsPatternExpressionSyntax pattern)
                {
                    return pattern;
                }
            }

            return base.GetSyntax(syntaxNodes);
        }

        protected override IsPatternExpressionSyntax GetUpdatedPatternSyntax(IsPatternExpressionSyntax pattern)
        {
            var updatedOperand = GetUpdatedOperand(pattern.Expression, out var expressionForPattern);

            if (expressionForPattern is null)
            {
                return pattern;
            }

            return IsPattern(updatedOperand, expressionForPattern);
        }

        protected override IsPatternExpressionSyntax GetUpdatedPatternSyntax(ExpressionSyntax operand, ExpressionSyntax expression)
        {
            var updatedOperand = GetUpdatedOperand(operand, out var expressionForPattern);

            return IsPattern(updatedOperand, expressionForPattern ?? expression);
        }

        private static ExpressionSyntax GetUpdatedOperand(ExpressionSyntax operand, out ExpressionSyntax expressionForPattern)
        {
            expressionForPattern = null;

            var node = operand;

            while (node != null)
            {
                switch (node)
                {
                    case ConditionalAccessExpressionSyntax c:
                    {
                        switch (c.WhenNotNull)
                        {
                            case ConditionalAccessExpressionSyntax nested:
                            {
                                node = nested;

                                continue;
                            }

                            case InvocationExpressionSyntax i when i.GetName() is nameof(Equals):
                            {
                                var arguments = i.ArgumentList.Arguments;

                                if (arguments.Count is 1)
                                {
                                    expressionForPattern = arguments[0].Expression;

                                    return operand.ReplaceNode(c, c.Expression);
                                }

                                return operand;
                            }

                            default:
                                return operand;
                        }
                    }

                    case InvocationExpressionSyntax invocation when invocation.GetName() is nameof(Equals):
                    {
                        var arguments = invocation.ArgumentList.Arguments;

                        if (arguments.Count is 1 && invocation.Expression is MemberAccessExpressionSyntax meas)
                        {
                            expressionForPattern = arguments[0].Expression;

                            return operand.ReplaceNode(invocation, meas.Expression);
                        }

                        return operand;
                    }

                    default:
                        return operand;
                }
            }

            return operand;
        }
    }
}
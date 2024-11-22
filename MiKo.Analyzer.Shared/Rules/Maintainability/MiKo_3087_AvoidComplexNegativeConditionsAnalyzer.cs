using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    /// <inheritdoc/>
    /// <seealso cref="MiKo_3202_InvertNegativeIfWhenReturningOnAllPathsAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3087_AvoidComplexNegativeConditionsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3087";

        public MiKo_3087_AvoidComplexNegativeConditionsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.IfStatement);

        private static bool HasIssue(ExpressionSyntax condition)
        {
            switch (condition)
            {
                case PrefixUnaryExpressionSyntax unary when unary.IsKind(SyntaxKind.LogicalNotExpression):
                    return HasIssueInLogicalNot(unary);

                case IsPatternExpressionSyntax pattern:
                    return HasIssueInIsPattern(pattern);

                case BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.LogicalOrExpression) && (binary.Left.IsKind(SyntaxKind.LogicalNotExpression) || binary.Right.IsKind(SyntaxKind.LogicalNotExpression)):
                    return true;

                default:
                    return false;
            }
        }

        private static bool HasIssueInIsPattern(IsPatternExpressionSyntax pattern)
        {
            if (pattern.IsPatternCheckFor(SyntaxKind.FalseLiteralExpression))
            {
                switch (pattern.Expression)
                {
                    case ParenthesizedExpressionSyntax _: return true; // we have an (abc is Xyz xyz is false) pattern
                    case IsPatternExpressionSyntax _: return true; // we have an ((xyz) is false) pattern
                }
            }

            if (pattern.Expression is BinaryExpressionSyntax)
            {
                // we have an (abc is Xyz xyz is false) pattern
                return true;
            }

            return false;
        }

        private static bool HasIssueInLogicalNot(PrefixUnaryExpressionSyntax unary)
        {
            switch (unary.Operand)
            {
                case ParenthesizedExpressionSyntax parenthesizedExpression:
                {
                    if (parenthesizedExpression.WithoutParenthesis() is IdentifierNameSyntax)
                    {
                        // we have a (!(xyz)) condition
                        return false;
                    }

                    // we have a more complex condition, starting with an !
                    return true;
                }

                case InvocationExpressionSyntax invocation:
                {
                    var arguments = invocation.ArgumentList.Arguments;

                    // keep in local variable to avoid multiple requests (see Roslyn implementation)
                    var argumentsCount = arguments.Count;

                    for (var index = 0; index < argumentsCount; index++)
                    {
                        if (arguments[index].Expression is InvocationExpressionSyntax)
                        {
                            // we have a more complex condition with multiple invocations, starting with an !
                            return true;
                        }
                    }

                    return false;
                }

                default:
                    return false;
            }
        }

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (IfStatementSyntax)context.Node;
            var condition = node.Condition;

            if (HasIssue(condition))
            {
                ReportDiagnostics(context, Issue(condition));
            }
        }
    }
}
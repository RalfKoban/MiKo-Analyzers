using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    /// <inheritdoc/>
    /// <seealso cref="MiKo_3087_AvoidComplexNegativeConditionsAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3202_InvertNegativeIfWhenReturningOnAllPathsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3202";

        public MiKo_3202_InvertNegativeIfWhenReturningOnAllPathsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeStatement, SyntaxKind.IfStatement, SyntaxKind.ConditionalExpression);

        private static bool ReturnsImmediately(IfStatementSyntax node)
        {
            if (node.ReturnsImmediately())
            {
                return node.Else?.ReturnsImmediately()
                    ?? node.NextSibling() is ReturnStatementSyntax; // happens in case we do not have an else clause
            }

            return false;
        }

        private static bool ReturnsImmediately(SyntaxNode syntaxNode)
        {
            var node = syntaxNode;

            while (true)
            {
                switch (node.Parent)
                {
                    case ReturnStatementSyntax _:
                    case ArrowExpressionClauseSyntax _:
                        return true;

                    case ParenthesizedExpressionSyntax parenthesized:
                        node = parenthesized;

                        continue;

                    default:
                        return false;
                }
            }
        }

        private static bool IsNegative(SyntaxNode condition)
        {
            var expression = condition;

            while (true)
            {
                if (expression.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    return true;
                }

                switch (expression)
                {
                    case IsPatternExpressionSyntax pattern when pattern.Pattern is ConstantPatternSyntax c && c.Expression.IsKind(SyntaxKind.FalseLiteralExpression):
                        return true;

                    case ParenthesizedExpressionSyntax parenthesized:
                        expression = parenthesized.Expression;

                        continue;

                    default:
                        return false;
                }
            }
        }

        private static SyntaxNode GetCondition(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case IfStatementSyntax ifStatement when ReturnsImmediately(ifStatement):
                    return ifStatement.Condition;

                case ConditionalExpressionSyntax conditional when ReturnsImmediately(conditional):
                    return conditional.Condition;

                default:
                    return null;
            }
        }

        private void AnalyzeStatement(SyntaxNodeAnalysisContext context)
        {
            var condition = GetCondition(context);

            if (condition != null && IsNegative(condition))
            {
                ReportDiagnostics(context, Issue(condition));
            }
        }
    }
}
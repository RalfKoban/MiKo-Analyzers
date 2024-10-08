using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class LogicalConditionsComparisonCanBeSimplifiedMaintainabilityAnalyzer : MaintainabilityAnalyzer
    {
        private static readonly SyntaxKind[] LogicalOr = { SyntaxKind.LogicalOrExpression };

        protected LogicalConditionsComparisonCanBeSimplifiedMaintainabilityAnalyzer(string id) : base(id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLogicalExpressions, LogicalOr);

        protected abstract bool IsApplicable(IReadOnlyList<ArgumentSyntax> arguments);

        protected abstract bool IsApplicable(ITypeSymbol typeSymbol);

        private static bool TryGetBinaryExpression(ExpressionSyntax expression, out BinaryExpressionSyntax result)
        {
            if (expression.WithoutParenthesis() is BinaryExpressionSyntax left && left.IsKind(SyntaxKind.EqualsExpression))
            {
                result = left;

                return true;
            }

            result = null;

            return false;
        }

        private static bool TryGetInvocationExpression(ExpressionSyntax expression, out InvocationExpressionSyntax result)
        {
            result = null;

            var syntax = expression.WithoutParenthesis();

            switch (syntax)
            {
                // (a != null && a.Equals(b))
                case BinaryExpressionSyntax b when b.IsKind(SyntaxKind.LogicalAndExpression)
                                                && b.Right.WithoutParenthesis() is InvocationExpressionSyntax invocation:
                {
                    result = invocation;

                    return true;
                }

                // a?.Equals(b) == true
                case BinaryExpressionSyntax b when b.IsKind(SyntaxKind.EqualsExpression)
                                                && b.Right.IsKind(SyntaxKind.TrueLiteralExpression)
                                                && b.Left is ConditionalAccessExpressionSyntax c
                                                && c.WhenNotNull is InvocationExpressionSyntax invocation:
                {
                    result = invocation;

                    return true;
                }

                // a?.Equals(b) is true
                case IsPatternExpressionSyntax p when p.IsPatternCheckFor(SyntaxKind.TrueLiteralExpression)
                                                   && p.Expression is ConditionalAccessExpressionSyntax c
                                                   && c.WhenNotNull is InvocationExpressionSyntax invocation:
                {
                    result = invocation;

                    return true;
                }
            }

            return false;
        }

        private bool HasIssue(BinaryExpressionSyntax node, SyntaxNodeAnalysisContext context)
        {
            // "a == b || (a != null && a.Equals(b))"
            // "a == b || a?.Equals(b) is true)"
            // "left.A == right.A || (left.A != null && left.A.Equals(right.A))"
            if (TryGetBinaryExpression(node.Left, out var left) && TryGetInvocationExpression(node.Right, out var invocation))
            {
                // "a == b || (a != null && a.Equals(b, StringComparison.Ordinal))"
                // "a == b || a?.Equals(b, StringComparison.Ordinal) is true)"
                // "left.A == right.A || (left.A != null && left.A.Equals(right.A, StringComparison.Ordinal))"
                var arguments = invocation.ArgumentList.Arguments;

                if (IsApplicable(arguments))
                {
                    var name = invocation.GetName();

                    if (name != nameof(Equals))
                    {
                        // no Equals method, so nothing to report here as simplifiable
                        return false;
                    }

                    var semanticModel = context.SemanticModel;

                    // we might have properties, so we investigate the complete expression
                    var typeSymbol = invocation.GetIdentifierExpression().GetTypeSymbol(semanticModel);

                    if (IsApplicable(typeSymbol))
                    {
                        var partA = left.Left;
                        var partB = left.Right;

                        var nameA = partA.ToString();
                        var nameB = partB.ToString();

                        var argument = arguments[0];
                        var argumentName = argument.ToString();

                        if (nameB == argumentName || nameA == argumentName)
                        {
                            return IsApplicable(partA.GetTypeSymbol(semanticModel)) && IsApplicable(partB.GetTypeSymbol(semanticModel)) && IsApplicable(argument.GetTypeSymbol(semanticModel));
                        }
                    }
                }
            }

            return false;
        }

        private void AnalyzeLogicalExpressions(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax node)
            {
                if (HasIssue(node, context))
                {
                    ReportDiagnostics(context, Issue(node));
                }
            }
        }
    }
}
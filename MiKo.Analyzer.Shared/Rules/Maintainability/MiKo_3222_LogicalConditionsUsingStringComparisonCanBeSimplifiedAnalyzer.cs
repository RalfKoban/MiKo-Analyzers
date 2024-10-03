using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3222_LogicalConditionsUsingStringComparisonCanBeSimplifiedAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3222";

        private static readonly SyntaxKind[] LogicalOr = { SyntaxKind.LogicalOrExpression };

        public MiKo_3222_LogicalConditionsUsingStringComparisonCanBeSimplifiedAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLogicalExpressions, LogicalOr);

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
                // (a != null && a.Equals(b, StringComparison.Ordinal))
                case BinaryExpressionSyntax b when b.IsKind(SyntaxKind.LogicalAndExpression)
                                                && b.Right.WithoutParenthesis() is InvocationExpressionSyntax invocation:
                {
                    result = invocation;

                    return true;
                }

                // a?.Equals(b, StringComparison.Ordinal) == true
                case BinaryExpressionSyntax b when b.IsKind(SyntaxKind.EqualsExpression)
                                                && b.Right.IsKind(SyntaxKind.TrueLiteralExpression)
                                                && b.Left is ConditionalAccessExpressionSyntax c
                                                && c.WhenNotNull is InvocationExpressionSyntax invocation:
                {
                    result = invocation;

                    return true;
                }

                // a?.Equals(b, StringComparison.Ordinal) is true
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

        private void AnalyzeLogicalExpressions(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax node)
            {
                var issues = AnalyzeLogicalExpressions(node, context);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeLogicalExpressions(BinaryExpressionSyntax node, SyntaxNodeAnalysisContext context)
        {
            // "a == b || (a != null && a.Equals(b, StringComparison.Ordinal))"
            // "a == b || a?.Equals(b, StringComparison.Ordinal) is true)"
            // "left.A == right.A || (left.A != null && left.A.Equals(right.A, StringComparison.Ordinal))"
            if (TryGetBinaryExpression(node.Left, out var left) && TryGetInvocationExpression(node.Right, out var invocation))
            {
                var arguments = invocation.ArgumentList.Arguments;

                if (arguments.Count <= 0)
                {
                    // no arguments, so nothing to report here as simplifiable
                    yield break;
                }

                var name = invocation.GetName();

                if (name != nameof(Equals))
                {
                    // no Equals method, so nothing to report here as simplifiable
                    yield break;
                }

                var semanticModel = context.SemanticModel;

                // we might have properties, so we investigate the complete expression
                if (invocation.GetIdentifierExpression().GetTypeSymbol(semanticModel).IsString())
                {
                    var partA = left.Left;
                    var partB = left.Right;

                    var nameA = partA.ToString();
                    var nameB = partB.ToString();

                    var argument = arguments[0];
                    var argumentName = argument.ToString();

                    if (nameB == argumentName || nameA == argumentName)
                    {
                        if (partA.GetTypeSymbol(semanticModel).IsString() && partB.GetTypeSymbol(semanticModel).IsString() && argument.GetTypeSymbol(semanticModel).IsString())
                        {
                            yield return Issue(node);
                        }
                    }
                }
            }
        }
    }
}
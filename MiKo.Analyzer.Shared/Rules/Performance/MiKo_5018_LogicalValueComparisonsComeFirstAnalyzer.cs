using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5018_LogicalValueComparisonsComeFirstAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5018";

        private static readonly SyntaxKind[] LogicalConditions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        public MiKo_5018_LogicalValueComparisonsComeFirstAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLogicalCondition, LogicalConditions);

        private static bool CanBeSkipped(ExpressionSyntax node, SemanticModel semanticModel)
        {
            switch (node)
            {
                case IdentifierNameSyntax _:
                    return true; // do not report checks on boolean members

                case PrefixUnaryExpressionSyntax pu when pu.IsKind(SyntaxKind.LogicalNotExpression):
                    return true; // do not report on boolean !xyz checks

                case IsPatternExpressionSyntax e:
                    if (e.Pattern is DeclarationPatternSyntax)
                    {
                        return true;  // do not report pattern checks
                    }

                    if (e.IsNullCheck())
                    {
                        return true; // do not report null pattern checks
                    }

                    if (e.IsBooleanCheck())
                    {
                        return true; // do not report boolean pattern checks
                    }

                    return false;

                case MemberAccessExpressionSyntax m:
                    return m.Name.IsValueType(semanticModel); // do not report checks on value type members

                case BinaryExpressionSyntax b:
                {
                    if (b.IsNullCheck())
                    {
                        return true; // do not report on checks for null
                    }

                    // do not report on value types
                    return b.Right.IsValueType(semanticModel) && b.Left.IsValueType(semanticModel);
                }

                default:
                    return false;
            }
        }

        private static bool HasIssue(BinaryExpressionSyntax binary, SemanticModel semanticModel)
        {
            if (binary.Parent.WithoutParenthesisParent() is BinaryExpressionSyntax parent && parent.IsAnyKind(LogicalConditions))
            {
                // ignore if we have already a parent logical expression as that will get inspected as well
                return false;
            }

            // first get all leaf nodes, not the logical expressions
            var leafs = binary.GetLeafs();

            // first ignore all that could be skipped, but then see if some nodes are left that could be skipped
            if (leafs.SkipWhile(_ => CanBeSkipped(_, semanticModel)).Any(_ => CanBeSkipped(_, semanticModel)))
            {
                // seems we have a non-number that is ordered before, so we have to report that
                return true;
            }

            // it still might be that we have array access before number comparisons, so investigate that
            // (note: as the element access nodes can be in between others, we have to sort them out
            return leafs.SkipWhile(_ => _.IsElementAccess() is false) // jump over all non-element access leafs
                        .SkipWhile(_ => _.IsElementAccess()) // now jump over all element access leafs
                        .Any(_ => CanBeSkipped(_, semanticModel)); // now no leaf should be left, otherwise we have an issue
        }

        private void AnalyzeLogicalCondition(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax expression)
            {
                if (HasIssue(expression, context.SemanticModel))
                {
                    context.ReportDiagnostic(Issue(expression));
                }
            }
        }
    }
}
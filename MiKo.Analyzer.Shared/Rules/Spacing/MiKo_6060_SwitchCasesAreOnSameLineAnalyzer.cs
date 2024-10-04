using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6060_SwitchCasesAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6060";

        private static readonly SyntaxKind[] SwitchLabels = { SyntaxKind.CaseSwitchLabel, SyntaxKind.CasePatternSwitchLabel };

        public MiKo_6060_SwitchCasesAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SwitchLabels);

        private static bool HasIssue(SyntaxNode node)
        {
            switch (node)
            {
                case CaseSwitchLabelSyntax label:
                    return label.IsSpanningMultipleLines();

                case CasePatternSwitchLabelSyntax pattern:
                {
                    if (pattern.WhenClause?.Condition is BinaryExpressionSyntax condition)
                    {
                        if (condition.Left is BinaryExpressionSyntax || condition.Right is BinaryExpressionSyntax)
                        {
                            return false;
                        }
                    }

                    return pattern.IsSpanningMultipleLines();
                }

                default:
                    return false;
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            if (HasIssue(node))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}
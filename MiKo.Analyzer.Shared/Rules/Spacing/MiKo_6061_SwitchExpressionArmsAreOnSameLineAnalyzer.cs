using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6061_SwitchExpressionArmsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6061";

        private static readonly SyntaxKind[] SwitchArms = { SyntaxKind.SwitchExpressionArm };

        public MiKo_6061_SwitchExpressionArmsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SwitchArms);

        private static bool HasIssue(SwitchExpressionArmSyntax arm)
        {
            if (arm.IsSpanningMultipleLines())
            {
                foreach (var descendant in arm.DescendantNodes())
                {
                    switch (descendant)
                    {
                        case InitializerExpressionSyntax _: // we have an initializer, so we cannot do anything
                        case InterpolatedStringExpressionSyntax i when i.IsSpanningMultipleLines(): // we have an interpolated string, so we cannot do anything
                        case LiteralExpressionSyntax l when l.IsKind(SyntaxKind.StringLiteralExpression) && l.IsSpanningMultipleLines(): // we seem to have verbatim string, so we cannot do anything
                            return false;
                    }
                }

                return true;
            }

            // maybe the comma is not placed at same line, so let's find out
            if (arm.Parent is SwitchExpressionSyntax switchExpression)
            {
                var arms = switchExpression.Arms;

                var index = arms.IndexOf(arm);

                // be aware that the last arm might have no separator
                if (index < arms.SeparatorCount)
                {
                    var token = arms.GetSeparator(index);

                    if (token.IsOnSameLineAs(arm) is false)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is SwitchExpressionArmSyntax arm && HasIssue(arm))
            {
                ReportDiagnostics(context, Issue(arm));
            }
        }
    }
}
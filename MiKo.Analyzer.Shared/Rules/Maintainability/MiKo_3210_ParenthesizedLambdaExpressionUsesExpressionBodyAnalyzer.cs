using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3210_ParenthesizedLambdaExpressionUsesExpressionBodyAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3210";

        public MiKo_3210_ParenthesizedLambdaExpressionUsesExpressionBodyAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (ParenthesizedLambdaExpressionSyntax)context.Node;
            if (node.ExpressionBody != null)
            {
                // it's already an expression body, so no need to report
                return;
            }

            var statements = 0;
            foreach (var _ in node.Block.DescendantNodes<StatementSyntax>())
            {
                statements++;

                if (statements > 1)
                {
                    break;
                }
            }

            // simplification works only if it is a single statement
            if (statements == 1)
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}
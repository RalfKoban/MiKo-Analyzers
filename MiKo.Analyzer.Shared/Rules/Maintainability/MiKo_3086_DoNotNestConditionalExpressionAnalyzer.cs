using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3086_DoNotNestConditionalExpressionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3086";

        public MiKo_3086_DoNotNestConditionalExpressionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.ConditionalExpression);

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (ConditionalExpressionSyntax)context.Node;

            foreach (var descendant in node.DescendantNodes())
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                switch (descendant.Kind())
                {
                    case SyntaxKind.ConditionalExpression:
                    case SyntaxKind.CoalesceExpression:
                    {
                        ReportIssue(context, descendant);

                        break;
                    }
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            var issue = Issue(string.Empty, node);

            ReportDiagnostics(context, issue);
        }
    }
}
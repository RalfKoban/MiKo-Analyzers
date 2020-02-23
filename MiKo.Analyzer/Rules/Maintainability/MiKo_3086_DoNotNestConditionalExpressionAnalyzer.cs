using System.Linq;

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

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.ConditionalExpression);

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (ConditionalExpressionSyntax)context.Node;

            foreach (var descendant in node.DescendantNodes().OfType<CSharpSyntaxNode>())
            {
                if (descendant.IsKind(SyntaxKind.ConditionalExpression))
                {
                    ReportIssue(context, descendant);
                }
                else if (descendant.IsKind(SyntaxKind.CoalesceExpression))
                {
                    ReportIssue(context, descendant);
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, CSharpSyntaxNode node)
        {
            var issue = Issue(string.Empty, node.GetLocation());
            context.ReportDiagnostic(issue);
        }
    }
}
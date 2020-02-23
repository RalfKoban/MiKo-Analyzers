using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3085_ConditionalExpressionTooLongAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3085";

        private const int MaxExpressionLength = 35;

        public MiKo_3085_ConditionalExpressionTooLongAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.ConditionalExpression);

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (ConditionalExpressionSyntax)context.Node;

            AnalyzeLength(context, node.Condition);
            AnalyzeLength(context, node.WhenTrue);
            AnalyzeLength(context, node.WhenFalse);
        }

        private void AnalyzeLength(SyntaxNodeAnalysisContext context, CSharpSyntaxNode node)
        {
            if (node.Span.Length > MaxExpressionLength)
            {
                ReportIssue(context, node);
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, CSharpSyntaxNode node)
        {
            var issue = Issue(string.Empty, node.GetLocation());
            context.ReportDiagnostic(issue);
        }
    }
}
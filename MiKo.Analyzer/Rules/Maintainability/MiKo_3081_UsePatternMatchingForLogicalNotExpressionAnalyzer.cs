using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3081";

        public MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLogicalNotExpression, SyntaxKind.LogicalNotExpression);

        private void AnalyzeLogicalNotExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var location = node.GetLocation();
            var issue = Issue(string.Empty, location);
            context.ReportDiagnostic(issue);
        }
    }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UsePatternMatchingForExpressionAnalyzer : MaintainabilityAnalyzer
    {
        private readonly SyntaxKind m_syntaxKind;

        protected UsePatternMatchingForExpressionAnalyzer(string diagnosticId, SyntaxKind syntaxKind) : base(diagnosticId, (SymbolKind)(-1)) => m_syntaxKind = syntaxKind;

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpressionLanguageAware, m_syntaxKind);

        protected abstract void AnalyzeExpression(SyntaxNodeAnalysisContext context);

        protected void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxToken token) => context.ReportDiagnostic(Issue(string.Empty, token));

        private void AnalyzeExpressionLanguageAware(SyntaxNodeAnalysisContext context)
        {
            if (context.IsSupported(LanguageVersion.CSharp7))
            {
                AnalyzeExpression(context);
            }
        }
    }
}
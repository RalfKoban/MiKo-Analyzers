using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UsePatternMatchingForExpressionAnalyzer : MaintainabilityAnalyzer
    {
        private readonly SyntaxKind m_syntaxKind;
        private readonly LanguageVersion m_languageVersion;

        protected UsePatternMatchingForExpressionAnalyzer(string diagnosticId, SyntaxKind syntaxKind, LanguageVersion languageVersion = LanguageVersion.CSharp7) : base(diagnosticId, (SymbolKind)(-1))
        {
            m_syntaxKind = syntaxKind;
            m_languageVersion = languageVersion;
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpressionLanguageAware, m_syntaxKind);

        protected abstract void AnalyzeExpression(SyntaxNodeAnalysisContext context);

        protected void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxToken token) => ReportDiagnostics(context, Issue(string.Empty, token));

        private void AnalyzeExpressionLanguageAware(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SyntaxTree.HasMinimumCSharpVersion(m_languageVersion))
            {
                AnalyzeExpression(context);
            }
        }
    }
}
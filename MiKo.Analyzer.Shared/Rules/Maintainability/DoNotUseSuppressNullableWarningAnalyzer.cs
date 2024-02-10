using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class DoNotUseSuppressNullableWarningAnalyzer : MaintainabilityAnalyzer
    {
        protected DoNotUseSuppressNullableWarningAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSuppressNullableWarningExpression, SyntaxKind.SuppressNullableWarningExpression);

        protected abstract bool HasIssue(PostfixUnaryExpressionSyntax warningExpression);

        private void AnalyzeSuppressNullableWarningExpression(SyntaxNodeAnalysisContext context)
        {
            var warningExpression = (PostfixUnaryExpressionSyntax)context.Node;

            if (HasIssue(warningExpression))
            {
                var issue = Issue(warningExpression.OperatorToken);

                ReportDiagnostics(context, issue);
            }
        }
    }
}
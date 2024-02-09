using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3501_DoNotUseSuppressNullableWarningOnConditionalAccessAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3501";

        public MiKo_3501_DoNotUseSuppressNullableWarningOnConditionalAccessAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSuppressNullableWarningExpression, SyntaxKind.SuppressNullableWarningExpression);

        private void AnalyzeSuppressNullableWarningExpression(SyntaxNodeAnalysisContext context)
        {
            var warningExpression = (PostfixUnaryExpressionSyntax)context.Node;

            if (warningExpression.Operand is ConditionalAccessExpressionSyntax)
            {
                var issue = Issue(warningExpression.OperatorToken);

                ReportDiagnostics(context, issue);
            }
        }
    }
}
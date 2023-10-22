using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5015_StringLiteralGetsInternedAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5015";

        public MiKo_5015_StringLiteralGetsInternedAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeStringLiteral, SyntaxKind.StringLiteralExpression);

        private void AnalyzeStringLiteral(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LiteralExpressionSyntax literal
             && literal.Parent is ArgumentSyntax argument
             && argument.Parent is ArgumentListSyntax list
             && list.Parent is InvocationExpressionSyntax invocation
             && invocation.Expression.GetName() == nameof(string.Intern))
            {
                ReportDiagnostics(context, Issue(invocation));
            }
        }
    }
}
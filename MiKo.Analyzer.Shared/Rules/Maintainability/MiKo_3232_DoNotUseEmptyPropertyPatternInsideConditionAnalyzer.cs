using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3232_DoNotUseEmptyPropertyPatternInsideConditionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3232";

        public MiKo_3232_DoNotUseEmptyPropertyPatternInsideConditionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is IfStatementSyntax i && i.Condition is IsPatternExpressionSyntax expression)
            {
                AnalyzePattern(context, expression);
            }
        }

        private void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ConditionalExpressionSyntax c && c.Condition is IsPatternExpressionSyntax expression)
            {
                AnalyzePattern(context, expression);
            }
        }

        private void AnalyzePattern(in SyntaxNodeAnalysisContext context, IsPatternExpressionSyntax expression)
        {
            var pattern = expression.Pattern;

            if (pattern is UnaryPatternSyntax u)
            {
                pattern = u.Pattern;
            }

            if (pattern is RecursivePatternSyntax p && p.PropertyPatternClause is PropertyPatternClauseSyntax clause && clause.Subpatterns.Count is 0)
            {
                ReportDiagnostics(context, Issue(clause));
            }
        }
    }
}
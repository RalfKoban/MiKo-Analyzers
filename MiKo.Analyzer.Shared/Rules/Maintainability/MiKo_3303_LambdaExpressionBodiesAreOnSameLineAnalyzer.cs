using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3303_LambdaExpressionBodiesAreOnSameLineAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3303";

        public MiKo_3303_LambdaExpressionBodiesAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression);

        private void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = context.Node;

            switch (lambda)
            {
                case ParenthesizedLambdaExpressionSyntax p when p.ExpressionBody != null:
                case SimpleLambdaExpressionSyntax s when s.ExpressionBody != null:
                    AnalyzeBody(context, lambda);

                    break;
            }
        }

        private void AnalyzeBody(SyntaxNodeAnalysisContext context, SyntaxNode lambda)
        {
            var startingLine = lambda.GetStartingLine();
            var endingLine = lambda.GetEndingLine();

            if (startingLine != endingLine)
            {
                ReportDiagnostics(context, Issue(lambda));
            }
        }
    }
}
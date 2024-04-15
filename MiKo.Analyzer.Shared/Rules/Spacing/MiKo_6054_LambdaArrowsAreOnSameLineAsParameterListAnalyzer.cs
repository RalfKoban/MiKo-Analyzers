using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6054_LambdaArrowsAreOnSameLineAsParameterListAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6054";

        public MiKo_6054_LambdaArrowsAreOnSameLineAsParameterListAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression);

        private void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case ParenthesizedLambdaExpressionSyntax p:
                {
                    AnalyzeLambda(context, p.ParameterList, p.ArrowToken, p.ExpressionBody);

                    break;
                }

                case SimpleLambdaExpressionSyntax s:
                {
                    AnalyzeLambda(context, s.Parameter, s.ArrowToken, s.ExpressionBody);

                    break;
                }
            }
        }

        private void AnalyzeLambda(SyntaxNodeAnalysisContext context, SyntaxNode parameter, SyntaxToken arrowToken, ExpressionSyntax expressionBody)
        {
            var parametersStartingLine = parameter.GetStartingLine();
            var tokenStartingLine = arrowToken.GetStartingLine();

            if (parametersStartingLine != tokenStartingLine)
            {
                ReportDiagnostics(context, Issue(arrowToken));
            }
            else
            {
                // only consider expression bodies to be placed on same line as arrow token
                if (expressionBody != null)
                {
                    var bodyStartingLine = expressionBody.GetStartingLine();

                    if (bodyStartingLine != tokenStartingLine)
                    {
                        ReportDiagnostics(context, Issue(arrowToken));
                    }
                }
            }
        }
    }
}
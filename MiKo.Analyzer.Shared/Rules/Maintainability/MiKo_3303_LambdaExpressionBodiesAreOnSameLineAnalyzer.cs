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

        private static bool CanAnalyzeBody(SyntaxNode lambda)
        {
            switch (lambda)
            {
                case ParenthesizedLambdaExpressionSyntax p when p.ExpressionBody != null: return CanAnalyze(p.Body);
                case SimpleLambdaExpressionSyntax s when s.ExpressionBody != null: return CanAnalyze(s.Body);
                default:
                    return false; // nothing to analyze
            }

            bool CanAnalyze(SyntaxNode body)
            {
                if (body is ObjectCreationExpressionSyntax o)
                {
                    if (o.Initializer?.Expressions.Count > 0)
                    {
                        // initializers are allowed to span multiple lines, so nothing to analyze here
                        return false;
                    }

                    if (o.ArgumentList?.Arguments.Count > 1)
                    {
                        // a lot of arguments are allowed to span multiple lines, so nothing to analyze here
                        return false;
                    }
                }

                return true;
            }
        }

        private void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = context.Node;

            if (CanAnalyzeBody(lambda))
            {
                AnalyzeBody(context, lambda);
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
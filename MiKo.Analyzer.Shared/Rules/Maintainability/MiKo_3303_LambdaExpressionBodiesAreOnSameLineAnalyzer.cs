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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression);

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
                switch (body)
                {
                    case ObjectCreationExpressionSyntax o: return CanAnalyzeObjectCreationExpressionSyntax(o);
                    case InvocationExpressionSyntax i: return CanAnalyzeInvocationExpressionSyntax(i);
                    default:
                        return true;
                }
            }

            bool CanAnalyzeObjectCreationExpressionSyntax(ObjectCreationExpressionSyntax syntax)
            {
                if (syntax.Initializer?.Expressions.Count > 0)
                {
                    // initializers are allowed to span multiple lines, so nothing to analyze here
                    return false;
                }

                if (syntax.ArgumentList?.Arguments.Count > 1)
                {
                    // a lot of arguments are allowed to span multiple lines, so nothing to analyze here
                    return false;
                }

                return true;
            }

            bool CanAnalyzeInvocationExpressionSyntax(InvocationExpressionSyntax syntax)
            {
                var argumentList = syntax.ArgumentList;

                if (argumentList is null)
                {
                    return true;
                }

                var arguments = argumentList.Arguments;

                switch (arguments.Count)
                {
                    case 0:
                        return true;

                    case 1:
                    {
                        if (arguments[0].Expression is ObjectCreationExpressionSyntax o && o.Initializer?.Expressions.Count > 0)
                        {
                            // initializers are allowed to span multiple lines, so nothing to analyze here
                            return false;
                        }

                        return true;
                    }

                    default:
                        return true; // TODO RKN: return false; // a lot of arguments are allowed to span multiple lines, so nothing to analyze here
                }
            }
        }

        private void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
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
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6043_LambdaExpressionBodiesAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6043";

        private static readonly SyntaxKind[] LogicalExpressions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        public MiKo_6043_LambdaExpressionBodiesAreOnSameLineAnalyzer() : base(Id)
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
                    case BinaryExpressionSyntax b: return CanAnalyzeBinaryExpressionSyntax(b);
                    default:
                        return true;
                }
            }

            bool CanAnalyzeBinaryExpressionSyntax(BinaryExpressionSyntax syntax)
            {
                if (syntax.IsAnyKind(LogicalExpressions) && syntax.DescendantNodes<BinaryExpressionSyntax>().Any(_ => _.IsAnyKind(LogicalExpressions)))
                {
                    // multiple binary expressions such as && or || are allowed to span multiple lines, so nothing to analyze here
                    return false;
                }

                return true;
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

                if (syntax.DescendantNodes<LambdaExpressionSyntax>().Any())
                {
                    // the other lambda get inspected itself, so nothing to analyze here
                    return false;
                }

                var arguments = argumentList.Arguments;

                switch (arguments.Count)
                {
                    case 0:
                        return true;

                    case 1:
                    {
                        var expression = arguments[0].Expression;

                        if (expression is ObjectCreationExpressionSyntax o && o.Initializer?.Expressions.Count > 0)
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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6062_ComplexInitializerElementExpressionsAreIndentedBesideOpenBraceAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6062";

        public MiKo_6062_ComplexInitializerElementExpressionsAreIndentedBesideOpenBraceAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ComplexElementInitializerExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InitializerExpressionSyntax initializer)
            {
                var issue = AnalyzeNode(initializer);

                if (issue != null)
                {
                    ReportDiagnostics(context, issue);
                }
            }
        }

        private Diagnostic AnalyzeNode(InitializerExpressionSyntax initializer)
        {
            var expressions = initializer.Expressions;

            if (expressions.Count > 0)
            {
                var expression = expressions[0];
                var expressionPosition = expression.GetStartPosition();
                var openBracePosition = initializer.OpenBraceToken.GetStartPosition();

                var expectedPosition = openBracePosition.Character + Constants.IndentationForComplexElementInitializerExpression;

                if (expressionPosition.Line > openBracePosition.Line && expressionPosition.Character != expectedPosition)
                {
                    return Issue(expression, CreateProposalForSpaces(expectedPosition));
                }
            }

            return null;
        }
    }
}
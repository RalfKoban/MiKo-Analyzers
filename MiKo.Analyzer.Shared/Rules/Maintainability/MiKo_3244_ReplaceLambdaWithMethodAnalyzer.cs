using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3244_ReplaceLambdaWithMethodAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3244";

        public MiKo_3244_ReplaceLambdaWithMethodAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        public int MaxLinesOfCode { get; set; } = 5;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLambda, SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLambda, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void AnalyzeLambda(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LambdaExpressionSyntax lambda)
            {
                var lines = lambda.Body.CountLines();

                if (lines > MaxLinesOfCode)
                {
                    if (lambda.IsExpressionTree(context.SemanticModel))
                    {
                        return; // we cannot convert expression trees
                    }

                    ReportDiagnostics(context, Issue(string.Empty, lambda.Body, lines, MaxLinesOfCode));
                }
            }
        }
    }
}
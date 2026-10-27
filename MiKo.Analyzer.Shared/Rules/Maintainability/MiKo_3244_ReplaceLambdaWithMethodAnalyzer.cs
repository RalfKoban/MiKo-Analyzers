using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Rules.Metrics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3244_ReplaceLambdaWithMethodAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3244";

        public MiKo_3244_ReplaceLambdaWithMethodAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        public int MaxLinesOfCode { get; set; } = 7;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLambda, SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLambda, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private static bool ShallAnalyze(in SyntaxNodeAnalysisContext context, LambdaExpressionSyntax lambda)
        {
            switch (lambda.Body)
            {
                case ObjectCreationExpressionSyntax _:
                case ImplicitObjectCreationExpressionSyntax _:
                    return false; // do not report object creations

                case BlockSyntax block when block.Statements.FirstOrDefault() is ReturnStatementSyntax:
                    return false; // do not report direct returns
            }

            if (context.ContainingSymbol is IMethodSymbol method && method.IsTestMethod())
            {
                var invocation = lambda.FirstAncestorWithinMethod<InvocationExpressionSyntax>();
                var identifierName = invocation.GetIdentifierName();

                switch (invocation.GetName())
                {
                    case "Multiple" when identifierName is "Assert":
                    case "MultipleAsync" when identifierName is "Assert":
                        return false; // do not report assertions
                }
            }

            if (lambda.IsExpressionTree(context.SemanticModel))
            {
                return false; // we cannot convert expression trees
            }

            return true;
        }

        private void AnalyzeLambda(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LambdaExpressionSyntax lambda && ShallAnalyze(context, lambda))
            {
                var lines = Counter.CountLinesOfCode(lambda);

                if (lines > MaxLinesOfCode)
                {
                    ReportDiagnostics(context, Issue(string.Empty, lambda.Body, lines, MaxLinesOfCode));
                }
            }
        }
    }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Rules.Metrics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3244_ReplaceAnonymousMethodBodiesWithMethodAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3244";

        public MiKo_3244_ReplaceAnonymousMethodBodiesWithMethodAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        public int MaxLinesOfCode { get; set; } = 7;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeAnonymousMethodExpression, SyntaxKind.AnonymousMethodExpression);

        private static bool ShallAnalyze(in SyntaxNodeAnalysisContext context, AnonymousMethodExpressionSyntax anonymous)
        {
            switch (anonymous.Body)
            {
                case ObjectCreationExpressionSyntax _:
                case ImplicitObjectCreationExpressionSyntax _:
                case AnonymousObjectCreationExpressionSyntax _:
                    return false; // do not report object creations
            }

            if (context.ContainingSymbol is IMethodSymbol method && method.IsTestMethod())
            {
                if (anonymous.Parent is ArgumentSyntax a && a.Parent?.Parent is InvocationExpressionSyntax invocation)
                {
                    var identifierName = invocation.GetIdentifierName();

                    switch (invocation.GetName())
                    {
                        case "Multiple" when identifierName is "Assert":
                        case "MultipleAsync" when identifierName is "Assert":
                            return false; // do not report assertions
                    }
                }
            }

            if (anonymous.Body is BlockSyntax block && block.Statements.FirstOrDefault() is ReturnStatementSyntax statement)
            {
                switch (statement.Expression)
                {
                    case ObjectCreationExpressionSyntax _:
                    case ImplicitObjectCreationExpressionSyntax _:
                    case AnonymousObjectCreationExpressionSyntax _:
                        return false; // do not report object creations
                }
            }

            return true;
        }

        private void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is AnonymousMethodExpressionSyntax anonymous && ShallAnalyze(context, anonymous))
            {
                var lines = Counter.CountLinesOfCode(anonymous);

                if (lines > MaxLinesOfCode)
                {
                    ReportDiagnostics(context, Issue(string.Empty, anonymous.Body, lines, MaxLinesOfCode));
                }
            }
        }
    }
}
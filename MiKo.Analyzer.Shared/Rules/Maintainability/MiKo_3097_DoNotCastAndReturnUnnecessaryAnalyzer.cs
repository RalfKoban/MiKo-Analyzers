using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3097_DoNotCastAndReturnUnnecessaryAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3097";

        public MiKo_3097_DoNotCastAndReturnUnnecessaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.CastExpression);

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var cast = (CastExpressionSyntax)context.Node;

            switch (cast.Parent)
            {
                case ReturnStatementSyntax _:
                case ArrowExpressionClauseSyntax _:
                {
                    if (context.ContainingSymbol is IMethodSymbol method && method.ReturnType.IsObject())
                    {
                        ReportDiagnostics(context, Issue(cast));
                    }

                    break;
                }
            }
        }
    }
}
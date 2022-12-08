using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    public abstract class MetricsAnalyzer : Analyzer
    {
        protected MetricsAnalyzer(string diagnosticId) : base(nameof(Metrics), diagnosticId)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterCodeBlockAction(AnalyzeCodeBlock);

        protected abstract Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol);

        private static BlockSyntax GetBody(CodeBlockAnalysisContext context)
        {
            switch (context.CodeBlock)
            {
                case AccessorDeclarationSyntax s: return s.Body;
                case ConstructorDeclarationSyntax s: return s.Body;
                case LocalFunctionStatementSyntax l: return l.Body;
                case MethodDeclarationSyntax s: return s.Body;
                default: return null;
            }
        }

        private void AnalyzeCodeBlock(CodeBlockAnalysisContext context)
        {
            var body = GetBody(context);

            if (body != null)
            {
                ReportDiagnostics(context, AnalyzeBody(body, context.OwningSymbol));
            }
        }
    }
}
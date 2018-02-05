using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingAnalyzer : Analyzer
    {
        protected NamingAnalyzer(string diagnosticId) : base("Naming", diagnosticId)
        {
        }

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method) => Enumerable.Empty<Diagnostic>();

        private void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var diagnostics = AnalyzeMethod((IMethodSymbol)context.Symbol);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
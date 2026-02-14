using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1501_FilterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1501";

        public MiKo_1501_FilterAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => symbol?.IsTestMethod() is false; // do not consider local functions inside tests

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        private Diagnostic[] AnalyzeName(ISymbol symbol)
        {
            var hasFilter = symbol.Name.Contains("Filter", StringComparison.OrdinalIgnoreCase);

            return hasFilter
                   ? new[] { Issue(symbol) }
                   : Array.Empty<Diagnostic>();
        }
    }
}
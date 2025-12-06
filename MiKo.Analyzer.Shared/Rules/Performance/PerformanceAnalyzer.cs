using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    /// <summary>
    /// Provides the base class for analyzers that enforce performance rules.
    /// </summary>
    public abstract class PerformanceAnalyzer : Analyzer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceAnalyzer"/> class with the unique identifier of the diagnostic and the kind of symbol to analyze.
        /// </summary>
        /// <param name="diagnosticId">
        /// The unique identifier of the diagnostic.
        /// </param>
        /// <param name="symbolKind">
        /// One of the enumeration members that specifies the kind of symbol to analyze.
        /// The default is <see cref="SymbolKind.Method"/>.
        /// </param>
        protected PerformanceAnalyzer(string diagnosticId, in SymbolKind symbolKind = SymbolKind.Method) : base(nameof(Performance), diagnosticId, symbolKind)
        {
        }

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                          ? Analyze(symbol, compilation)
                                                                                                                          : Array.Empty<Diagnostic>();

        /// <summary>
        /// Determines whether the specified method symbol shall be analyzed for performance issues.
        /// </summary>
        /// <param name="symbol">
        /// The method symbol to verify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the symbol shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => true;

        /// <summary>
        /// Analyzes the specified method symbol for performance issues and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The method symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that contains the symbol.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();
    }
}
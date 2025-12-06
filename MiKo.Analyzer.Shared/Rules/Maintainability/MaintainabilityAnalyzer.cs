using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    /// <summary>
    /// Provides the base class for analyzers that enforce maintainability rules.
    /// </summary>
    public abstract class MaintainabilityAnalyzer : Analyzer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaintainabilityAnalyzer"/> class with the unique identifier of the diagnostic and the kind of symbol to analyze.
        /// </summary>
        /// <param name="diagnosticId">
        /// The unique identifier of the diagnostic.
        /// </param>
        /// <param name="symbolKind">
        /// One of the enumeration members that specifies the kind of symbol to analyze.
        /// The default is <see cref="SymbolKind.Method"/>.
        /// </param>
        protected MaintainabilityAnalyzer(string diagnosticId, in SymbolKind symbolKind = SymbolKind.Method) : base(nameof(Maintainability), diagnosticId, symbolKind)
        {
        }

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                ? Analyze(symbol, compilation)
                                                                                                                                : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                           ? Analyze(symbol, compilation)
                                                                                                                           : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                          ? Analyze(symbol, compilation)
                                                                                                                          : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                              ? Analyze(symbol, compilation)
                                                                                                                              : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                        ? Analyze(symbol, compilation)
                                                                                                                        : Array.Empty<Diagnostic>();

        /// <inheritdoc/>
        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                        ? Analyze(symbol, compilation)
                                                                                                                        : Array.Empty<Diagnostic>();

        /// <summary>
        /// Determines whether the specified namespace symbol shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The namespace symbol to verify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the symbol shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(INamespaceSymbol symbol) => true;

        /// <summary>
        /// Determines whether the specified type symbol shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The type symbol to verify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the symbol shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(INamedTypeSymbol symbol) => true;

        /// <summary>
        /// Determines whether the specified method symbol shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The method symbol to verify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the symbol shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsOverride is false;

        /// <summary>
        /// Determines whether the specified property symbol shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The property symbol to verify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the symbol shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => symbol.IsOverride is false;

        /// <summary>
        /// Determines whether the specified field symbol shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The field symbol to verify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the symbol shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => symbol.IsOverride is false;

        /// <summary>
        /// Determines whether the specified event symbol shall be analyzed.
        /// </summary>
        /// <param name="symbol">
        /// The event symbol to verify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the symbol shall be analyzed; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ShallAnalyze(IEventSymbol symbol) => symbol.IsOverride is false;

        /// <summary>
        /// Analyzes the specified namespace symbol for maintainability issues and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The namespace symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that contains the symbol.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> Analyze(INamespaceSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the specified type symbol for maintainability issues and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The type symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that contains the symbol.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the specified method symbol for maintainability issues and returns any diagnostics found.
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

        /// <summary>
        /// Analyzes the specified property symbol for maintainability issues and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The property symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that contains the symbol.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the specified field symbol for maintainability issues and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The field symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that contains the symbol.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Analyzes the specified event symbol for maintainability issues and returns any diagnostics found.
        /// </summary>
        /// <param name="symbol">
        /// The event symbol to analyze.
        /// </param>
        /// <param name="compilation">
        /// The compilation that contains the symbol.
        /// </param>
        /// <returns>
        /// A collection of diagnostics found during analysis.
        /// </returns>
        protected virtual IEnumerable<Diagnostic> Analyze(IEventSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        /// <summary>
        /// Creates a diagnostic for the specified type symbol at the appropriate location.
        /// </summary>
        /// <param name="type">
        /// The type symbol to create the diagnostic for.
        /// </param>
        /// <param name="container">
        /// The symbol that contains the type.
        /// </param>
        /// <returns>
        /// A diagnostic for the specified type.
        /// </returns>
        protected Diagnostic IssueOnType(ITypeSymbol type, ISymbol container)
        {
            // detect for same assembly to avoid AD0001 (which reports that the return type is in a different compilation than the method/property)
            if (type.Locations.IsEmpty || SymbolEqualityComparer.Default.Equals(container.ContainingAssembly, type.ContainingAssembly) is false)
            {
                switch (container.GetSyntax())
                {
                    case MethodDeclarationSyntax method: return Issue(type.ToString(), method.ReturnType);
                    case BasePropertyDeclarationSyntax property: return Issue(type.ToString(), property.Type);
                    case ParameterSyntax parameter: return Issue(type.ToString(), parameter.Type);
                }
            }

            return Issue(type);
        }
    }
}
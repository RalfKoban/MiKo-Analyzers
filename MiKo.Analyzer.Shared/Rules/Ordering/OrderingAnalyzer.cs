using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    /// <summary>
    /// Provides the base class for analyzers that enforce ordering rules.
    /// </summary>
    public abstract class OrderingAnalyzer : Analyzer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderingAnalyzer"/> class with the unique identifier of the diagnostic and the kind of symbol to analyze.
        /// </summary>
        /// <param name="diagnosticId">
        /// The unique identifier of the diagnostic.
        /// </param>
        /// <param name="symbolKind">
        /// One of the enumeration members that specifies the kind of symbol to analyze.
        /// The default is <see cref="SymbolKind.NamedType"/>.
        /// </param>
        protected OrderingAnalyzer(string diagnosticId, in SymbolKind symbolKind = SymbolKind.NamedType) : base(nameof(Ordering), diagnosticId, symbolKind)
        {
        }

        /// <summary>
        /// Gets a collection of method symbols of the specified kind, ordered by their location in the source file.
        /// </summary>
        /// <param name="type">
        /// The type symbol containing the methods.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of method.
        /// The default is <see cref="MethodKind.Ordinary"/>.
        /// </param>
        /// <returns>
        /// A collection of method symbols ordered by their source code location.
        /// </returns>
        protected static IList<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol type, MethodKind kind = MethodKind.Ordinary) => GetMethodsOrderedByLocation(type, type.GetLineSpan().Path, kind);

        /// <summary>
        /// Gets a collection of method symbols of the specified kind from a specific file path, ordered by their location in the source file.
        /// </summary>
        /// <param name="type">
        /// The type symbol containing the methods.
        /// </param>
        /// <param name="path">
        /// The file path to filter methods by location.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of method.
        /// The default is <see cref="MethodKind.Ordinary"/>.
        /// </param>
        /// <returns>
        /// A collection of method symbols from the specified file path, ordered by their source code location.
        /// </returns>
        protected static IList<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol type, string path, MethodKind kind = MethodKind.Ordinary) => type.GetMethods(kind)
                                                                                                                                                            .Where(_ => _.GetLineSpan().Path == path)
                                                                                                                                                            .OrderBy(_ => _.GetLineSpan().StartLinePosition)
                                                                                                                                                            .ToList();

        /// <summary>
        /// Gets a collection of field symbols from a specific file path, ordered by their location in the source file.
        /// </summary>
        /// <param name="type">
        /// The type symbol containing the fields.
        /// </param>
        /// <param name="path">
        /// The file path to filter fields by location.
        /// </param>
        /// <returns>
        /// A collection of field symbols from the specified file path, ordered by their source code location.
        /// </returns>
        protected static IReadOnlyList<IFieldSymbol> GetFieldsOrderedByLocation(INamedTypeSymbol type, string path) => type.GetFields()
                                                                                                                           .Where(_ => _.GetLineSpan().Path == path)
                                                                                                                           .OrderBy(_ => _.GetLineSpan().StartLinePosition)
                                                                                                                           .ToList();
    }
}
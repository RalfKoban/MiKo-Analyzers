using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1510_TypesWithInfoSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1510";

        public MiKo_1510_TypesWithInfoSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => symbol.Name.EndsWith("Info", StringComparison.Ordinal)
                                                                                                                    ? new[] { Issue(symbol) }
                                                                                                                    : Array.Empty<Diagnostic>();
    }
}
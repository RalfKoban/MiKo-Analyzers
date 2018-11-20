using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1402_TestNamespaceAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1402";

        public MiKo_1402_TestNamespaceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeName(symbol.ContainingNamespace);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol)
        {
            while (true)
            {
                if (symbol is null)
                    return Enumerable.Empty<Diagnostic>();

                if (symbol.Name.Contains("Test", StringComparison.OrdinalIgnoreCase))
                    return new[] { ReportIssue(symbol) };

                symbol = symbol.ContainingNamespace;
            }
        }
    }
}
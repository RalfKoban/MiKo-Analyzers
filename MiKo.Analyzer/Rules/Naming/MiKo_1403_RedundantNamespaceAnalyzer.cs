using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1403_RedundantNamespaceAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1403";

        public MiKo_1403_RedundantNamespaceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeName(symbol.ContainingNamespace);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol)
        {
            var knownNamespaces = new HashSet<string>();

            while (true)
            {
                if (symbol is null)
                    return Enumerable.Empty<Diagnostic>();

                if (!knownNamespaces.Add(symbol.Name))
                    return new[] { ReportIssue(symbol) };

                symbol = symbol.ContainingNamespace;
            }
        }
    }
}
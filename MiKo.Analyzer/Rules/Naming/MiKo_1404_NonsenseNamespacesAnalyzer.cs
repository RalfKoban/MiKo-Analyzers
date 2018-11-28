using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1404_NonsenseNamespacesAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1404";

        public MiKo_1404_NonsenseNamespacesAnalyzer() : base(Id, SymbolKind.Namespace)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol)
        {
            var namespaceMarkers = Constants.NonsenseNamespaceMarkers;

            var fullNamespaceName = symbol.ToString();
            if (fullNamespaceName.ContainsAny(namespaceMarkers))
            {
                var name = namespaceMarkers.First(_ => fullNamespaceName.Contains(_, StringComparison.OrdinalIgnoreCase));
                return new[] { ReportIssue(symbol, name) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}
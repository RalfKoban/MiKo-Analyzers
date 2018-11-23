using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1401_TechnicalNamespacesAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1401";

        public MiKo_1401_TechnicalNamespacesAnalyzer() : base(Id, SymbolKind.Namespace)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol)
        {
            var fullNamespaceName = symbol.ToString();

            if (HasMarker(fullNamespaceName))
            {
                var name = Constants.TechnicalNamespaceMarkers.First(_ => fullNamespaceName.Contains(_, StringComparison.OrdinalIgnoreCase));
                return new[] { ReportIssue(symbol, name) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static bool HasMarker(string name) => name.EqualsAny(Constants.TechnicalNamespaceMarkers)
                                                   || name.StartsWithAny(Constants.TechnicalNamespaceStartMarkers)
                                                   || name.EndsWithAny(Constants.TechnicalNamespaceEndMarkers)
                                                   || name.ContainsAny(Constants.TechnicalNamespaceMiddleMarkers);
    }
}
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
            var symbolName = symbol.Name;

            if (symbolName.StartsWithAny(Constants.TechnicalNamespaceMarkers)
             || symbolName.EndsWithAny(Constants.TechnicalNamespaceEndMarkers)
             || symbolName.ContainsAny(Constants.TechnicalNamespaceMiddleMarkers))
            {
                return new[] { ReportIssue(symbol) };
            }

            return Enumerable.Empty<Diagnostic>();

        }
    }
}
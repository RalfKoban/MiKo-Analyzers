using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1401_TechnicalNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1401";

        public MiKo_1401_TechnicalNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location)
        {
            var markers = Constants.TechnicalNamespaceMarkers;

            return HasMarker(qualifiedName)
                       ? new[] { ReportIssue(qualifiedName, location, markers.First(_ => qualifiedName.Contains(_, StringComparison.OrdinalIgnoreCase))) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static bool HasMarker(string name) => name.EqualsAny(Constants.TechnicalNamespaceMarkers)
                                                   || name.StartsWithAny(Constants.TechnicalNamespaceStartMarkers)
                                                   || name.EndsWithAny(Constants.TechnicalNamespaceEndMarkers)
                                                   || name.ContainsAny(Constants.TechnicalNamespaceMiddleMarkers);
    }
}
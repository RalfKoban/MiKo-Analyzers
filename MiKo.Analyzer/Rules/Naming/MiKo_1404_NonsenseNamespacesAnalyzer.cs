using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1404_NonsenseNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1404";

        public MiKo_1404_NonsenseNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location)
        {
            var markers = Constants.NonsenseNamespaceMarkers;

            return qualifiedName.ContainsAny(markers)
                       ? new[] { ReportIssue(qualifiedName, location, markers.First(_ => qualifiedName.Contains(_, StringComparison.OrdinalIgnoreCase))) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}
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

        private static readonly string[] NonsenseNamespaces = { "Helper", "Helpers", "Util", "Utils", "Utility", "Utilities", "Misc", "Miscellaneous" };

        public MiKo_1404_NonsenseNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location)
        {
            if (qualifiedName.ContainsAny(NonsenseNamespaces))
            {
                var ns = NonsenseNamespaces.Last(_ => qualifiedName.Contains(_, StringComparison.OrdinalIgnoreCase));

                yield return Issue(qualifiedName, location, ns);
            }
        }
    }
}
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

        private static readonly string[] TechnicalNamespaces =
            {
                "Base", "Class", "Classes",
                "Enum", "Enums", "Enumeration", "Enumerations",
                "Exception", "Exceptions",
                "Imp", "Impl", "Implementation", "Implementations",
                "Interface", "Interfaces",
                "Proxies", "Proxy", "ServiceProxies", "ServiceProxy",
                "Struct", "Structs", "Action", "Actions",
            };

        private static readonly string[] TechnicalNamespacesStart = TechnicalNamespaces.Select(_ => _ + ".").ToArray();
        private static readonly string[] TechnicalNamespacesEnd = TechnicalNamespaces.Select(_ => "." + _).ToArray();
        private static readonly string[] TechnicalNamespacesMiddle = TechnicalNamespaces.Select(_ => "." + _ + ".").ToArray();

        public MiKo_1401_TechnicalNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location)
        {
            var name = qualifiedName.Replace("Structure", "#");

            if (HasMarker(name))
            {
                var marker = TechnicalNamespaces.Last(_ => name.Contains(_, StringComparison.OrdinalIgnoreCase));

                yield return Issue(qualifiedName, location, marker);
            }
        }

        private static bool HasMarker(string name) => name.EqualsAny(TechnicalNamespaces)
                                                   || name.StartsWithAny(TechnicalNamespacesStart)
                                                   || name.EndsWithAny(TechnicalNamespacesEnd)
                                                   || name.ContainsAny(TechnicalNamespacesMiddle);
    }
}
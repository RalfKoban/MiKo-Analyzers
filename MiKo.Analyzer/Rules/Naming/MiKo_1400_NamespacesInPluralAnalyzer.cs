using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1400_NamespacesInPluralAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1400";

        private static readonly string[] AllowedSuffixes =
            {
                "s",
                "ing",
                "ComponentModel",
                "Composition",
                "Data",
                "Documentation",
                "Framework",
                "Generic",
                "IO",
                "Infrastructure",
                "Lifetime",
                "Linq",
                "Maintainability",
                "Runtime",
                "Security",
                "ServiceModel",
                "Serialization",
                "System",
                "Threading",
                "UserExperience",

                // known company / framework names
                "Microsoft",
                "PostSharp",
                "NDepend",
            };

        public MiKo_1400_NamespacesInPluralAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location)
        {
            var lastName = qualifiedName.Substring(qualifiedName.LastIndexOf('.') + 1);
            return lastName.EndsWithAny(AllowedSuffixes)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(qualifiedName, location) };
        }
    }
}
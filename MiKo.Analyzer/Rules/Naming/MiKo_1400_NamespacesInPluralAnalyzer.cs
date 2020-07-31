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
                "ren",
                "ComponentModel",
                "Composition",
                "Data",
                "Documentation",
                "Framework",
                "Generic",
                "IO",
                "Infrastructure",
                "Interop",
                "Lifetime",
                "Linq",
                "Maintainability",
                "Performance",
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
            var name = qualifiedName.GetNameOnlyPart();

            // maybe it's a number, so we have to check for that
            if (name.EndsWithNumber() || name.EndsWithAny(AllowedSuffixes))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var pluralName = Pluralizer.GetPluralName(name);

            return new[] { Issue(qualifiedName, location, pluralName) };
        }
    }
}
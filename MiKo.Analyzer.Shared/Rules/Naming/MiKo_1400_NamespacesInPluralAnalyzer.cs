﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

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
                "Build",
                "ComponentModel",
                "Composition",
                "Core",
                "Data",
                "Design",
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
                "Shared",
                "System",
                "Threading",
                "UserExperience",
                "UI",
                "Web",

                // known company / framework names
                "Microsoft",
                "Office",
                "PostSharp",
                "NDepend",
            };

        public MiKo_1400_NamespacesInPluralAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(string name)
        {
            // maybe it's a number, so we have to check for that
            if (name.IsAcronym() || name.EndsWithNumber() || name.EndsWithAny(AllowedSuffixes))
            {
                // nothing to do here
                return name;
            }

            return name == "Model"
                    ? Pluralizer.GetPluralName("Entity")
                    : Pluralizer.GetPluralName(name);
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location)
        {
            var name = qualifiedName.GetNameOnlyPart();
            var betterName = FindBetterName(name);

            if (name != betterName)
            {
                yield return Issue(qualifiedName, location, betterName);
            }
        }
    }
}
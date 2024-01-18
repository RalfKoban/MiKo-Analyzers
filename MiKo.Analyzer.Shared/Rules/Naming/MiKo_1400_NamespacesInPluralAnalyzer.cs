using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1400_NamespacesInPluralAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1400";

        private static readonly string[] AllowedSuffixes = new[]
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
                                                                   "Support",
                                                                   "System",
                                                                   "UserExperience",
                                                                   "UI",
                                                                   "Web",
                                                                   "Microsoft",
                                                                   "Office",
                                                                   "PostSharp",
                                                                   "NDepend",
                                                                   "CSharp",
                                                                   "VisualBasic",
                                                                   "CPlusPlus",
                                                                   "TypeScript",
                                                                   "JavaScript",
                                                                   "Perl",
                                                               }.OrderBy(_ => _.Length).ToArray();

        public MiKo_1400_NamespacesInPluralAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(string name)
        {
            if (name == "Model")
            {
                return Pluralizer.GetPluralName(Constants.Entity);
            }

            // maybe it's a number, so we have to check for that
            if (name.EndsWithNumber() || name.IsAcronym() || name.EndsWithAny(AllowedSuffixes))
            {
                // nothing to do here
                return name;
            }

            return Pluralizer.GetPluralName(name);
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            var namespaceNames = names.ToList();
            var namespaceNamesCount = namespaceNames.Count;

            if (namespaceNamesCount > 1)
            {
                var namePart = namespaceNames[namespaceNamesCount - 1];

                var name = namePart.ValueText;
                var betterName = FindBetterName(name);

                if (betterName != null && name != betterName)
                {
                    yield return Issue(name, namePart, betterName);
                }
            }
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1400_NamespacesInPluralAnalyzer : NamespaceNamingAnalyzer
    {
        public const string Id = "MiKo_1400";

        private static readonly ConcurrentDictionary<string, string> BetterNames = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        private static readonly string[] AllowedSuffixes = new[]
                                                               {
                                                                   "s",
                                                                   "ing",
                                                                   "ren",
                                                                   "Build",
                                                                   "ComponentModel",
                                                                   "Composition",
                                                                   Constants.Core,
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
                                                                   "Azure",
                                                                   "Docker",
                                                               }.OrderBy(_ => _.Length).ToArray();

        public MiKo_1400_NamespacesInPluralAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(string name)
        {
            return BetterNames.GetOrAdd(name, FindBetterNameLocal);

            string FindBetterNameLocal(string originalName)
            {
                if (originalName is "Model")
                {
                    return Pluralizer.GetPluralName(Constants.Entity);
                }

                // maybe it's a number, so we have to check for that
                if (originalName.EndsWithNumber() || originalName.IsAcronym() || originalName.EndsWithAny(AllowedSuffixes, StringComparison.OrdinalIgnoreCase))
                {
                    // nothing to do here
                    return originalName;
                }

                return Pluralizer.GetPluralName(originalName);
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames)
        {
            var namespaceNamesCount = namespaceNames.Length;

            if (namespaceNamesCount > 1)
            {
                var namePart = namespaceNames[namespaceNamesCount - 1];

                var name = namePart.ValueText;
                var betterName = FindBetterName(name);

                if (betterName != null && name != betterName)
                {
                    return new[] { Issue(name, namePart, betterName) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}
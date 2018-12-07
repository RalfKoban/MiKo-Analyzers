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

        private static readonly string[] AllowedSuffixes = { "s", "ing", "Security", "Maintainability", "Documentation", "System" };
        private static readonly char[] NamespaceDelimiters = { '.' };

        public MiKo_1400_NamespacesInPluralAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location)
        {
            var lastName = qualifiedName.Split(NamespaceDelimiters, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            return lastName.EndsWithAny(AllowedSuffixes)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(qualifiedName, location, lastName) };
        }
    }
}
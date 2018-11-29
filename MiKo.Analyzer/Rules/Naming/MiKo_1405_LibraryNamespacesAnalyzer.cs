using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1405_LibraryNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1405";

        private static readonly string[] LibraryNamespaces = { "Lib", "Library", "Libraries" };


        public MiKo_1405_LibraryNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location)
        {
            return qualifiedName.ContainsAny(LibraryNamespaces)
                       ? new[] { ReportIssue(qualifiedName, location, LibraryNamespaces.Last(_ => qualifiedName.Contains(_, StringComparison.OrdinalIgnoreCase))) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}
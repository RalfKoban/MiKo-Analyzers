using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1402_TestNamespaceAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1402";

        public MiKo_1402_TestNamespaceAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location) => qualifiedName.Contains("Test", StringComparison.OrdinalIgnoreCase)
                                                                                                                        ? new[] { ReportIssue(qualifiedName, location) }
                                                                                                                        : Enumerable.Empty<Diagnostic>();
    }
}
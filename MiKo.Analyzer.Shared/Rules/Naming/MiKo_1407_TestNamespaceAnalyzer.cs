using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1407_TestNamespaceAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1407";

        public MiKo_1407_TestNamespaceAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            foreach (var name in names)
            {
                if (name.ValueText.Contains("Test", StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(name);
                }
            }
        }
    }
}
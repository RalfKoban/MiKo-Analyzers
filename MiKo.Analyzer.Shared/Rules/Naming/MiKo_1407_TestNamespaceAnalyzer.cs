using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1407_TestNamespaceAnalyzer : NamespaceNamingAnalyzer
    {
        public const string Id = "MiKo_1407";

        public MiKo_1407_TestNamespaceAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames)
        {
            List<Diagnostic> issues = null;

            foreach (var name in namespaceNames)
            {
                if (name.ValueText.Contains("Test", StringComparison.OrdinalIgnoreCase))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(name));
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}
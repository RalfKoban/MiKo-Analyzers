using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1403_RedundantNamespaceAnalyzer : NamespaceNamingAnalyzer
    {
        public const string Id = "MiKo_1403";

        public MiKo_1403_RedundantNamespaceAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames)
        {
            if (namespaceNames.Length is 1)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> issues = null;

            var knownNamespaces = new HashSet<string>();

            foreach (var name in namespaceNames)
            {
                if (knownNamespaces.Add(name.ValueText))
                {
                    continue;
                }

                if (issues is null)
                {
                    issues = new List<Diagnostic>(1);
                }

                issues.Add(Issue(name));
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}
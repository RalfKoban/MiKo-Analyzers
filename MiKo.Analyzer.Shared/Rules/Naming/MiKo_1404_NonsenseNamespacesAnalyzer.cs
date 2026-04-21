using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1404_NonsenseNamespacesAnalyzer : NamespaceNamingAnalyzer
    {
        public const string Id = "MiKo_1404";

        private static readonly HashSet<string> NonsenseNamespaces = new HashSet<string> { "AsyncHelper", "AsyncHelpers", "Helper", "Helpers", "Util", "Utils", "Utility", "Utilities", "Misc", "Miscellaneous" };

        public MiKo_1404_NonsenseNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames)
        {
            List<Diagnostic> issues = null;

            foreach (var name in namespaceNames)
            {
                if (NonsenseNamespaces.Contains(name.ValueText))
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
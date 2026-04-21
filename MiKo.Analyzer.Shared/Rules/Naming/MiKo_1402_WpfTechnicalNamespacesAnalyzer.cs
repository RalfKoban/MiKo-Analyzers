using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1402_WpfTechnicalNamespacesAnalyzer : NamespaceNamingAnalyzer
    {
        public const string Id = "MiKo_1402";

        private static readonly HashSet<string> TechnicalWPFNamespaces = new HashSet<string> { "Command", "Commands", "Model", "Models", "View", "Views", "ViewModel", "ViewModels", "MVVM", };
        private static readonly HashSet<string> ModelNamespaces = new HashSet<string> { "ComponentModel", "ServiceModel" };

        public MiKo_1402_WpfTechnicalNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames)
        {
            List<Diagnostic> issues = null;

            foreach (var name in namespaceNames)
            {
                var namespaceName = name.ValueText;

                if (ModelNamespaces.Contains(namespaceName))
                {
                    continue;
                }

                if (TechnicalWPFNamespaces.Contains(namespaceName))
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
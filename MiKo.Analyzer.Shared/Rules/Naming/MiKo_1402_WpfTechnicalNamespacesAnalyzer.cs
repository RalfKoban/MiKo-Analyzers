using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1402_WpfTechnicalNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1402";

        private static readonly HashSet<string> TechnicalWPFNamespaces = new HashSet<string> { "Command", "Commands", "Model", "Models", "View", "Views", "ViewModel", "ViewModels", "MVVM", };
        private static readonly HashSet<string> ModelNamespaces = new HashSet<string> { "ComponentModel", "ServiceModel" };

        public MiKo_1402_WpfTechnicalNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            foreach (var name in names)
            {
                var namespaceName = name.ValueText;

                if (ModelNamespaces.Contains(namespaceName))
                {
                    continue;
                }

                if (TechnicalWPFNamespaces.Contains(namespaceName))
                {
                    yield return Issue(name);
                }
            }
        }
    }
}
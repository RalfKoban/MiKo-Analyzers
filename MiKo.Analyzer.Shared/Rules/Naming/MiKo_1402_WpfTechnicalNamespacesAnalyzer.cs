using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1402_WpfTechnicalNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1402";

        private static readonly string[] TechnicalWPFNamespaces = { "Command", "Commands", "Model", "Models", "View", "Views", "ViewModel", "ViewModels", };
        private static readonly string[] ModelNamespaces = { "ComponentModel", "ServiceModel" };

        public MiKo_1402_WpfTechnicalNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location)
        {
            var fullName = qualifiedName.Without(ModelNamespaces);

            if (fullName.ContainsAny(TechnicalWPFNamespaces))
            {
                var ns = TechnicalWPFNamespaces.Last(_ => fullName.Contains(_, StringComparison.OrdinalIgnoreCase));

                yield return Issue(qualifiedName, location, ns);
            }
        }
    }
}
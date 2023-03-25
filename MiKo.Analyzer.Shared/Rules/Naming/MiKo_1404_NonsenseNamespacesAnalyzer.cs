using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1404_NonsenseNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1404";

        private static readonly HashSet<string> NonsenseNamespaces = new HashSet<string> { "Helper", "Helpers", "Util", "Utils", "Utility", "Utilities", "Misc", "Miscellaneous" };

        public MiKo_1404_NonsenseNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            foreach (var name in names)
            {
                if (NonsenseNamespaces.Contains(name.ValueText))
                {
                    yield return Issue(name);
                }
            }
        }
    }
}
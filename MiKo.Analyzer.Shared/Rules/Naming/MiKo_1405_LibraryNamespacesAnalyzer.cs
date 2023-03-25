using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1405_LibraryNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1405";

        private static readonly HashSet<string> LibraryNamespaces = new HashSet<string> { "Lib", "Library", "Libraries" };

        public MiKo_1405_LibraryNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            foreach (var name in names)
            {
                if (LibraryNamespaces.Contains(name.ValueText))
                {
                    yield return Issue(name);
                }
            }
        }
    }
}
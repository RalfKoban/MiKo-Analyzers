using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1403_RedundantNamespaceAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1403";

        public MiKo_1403_RedundantNamespaceAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            var knownNamespaces = new HashSet<string>();

            foreach (var name in names.Where(_ => knownNamespaces.Add(_.ValueText) is false))
            {
                yield return Issue(name);
            }
        }
    }
}
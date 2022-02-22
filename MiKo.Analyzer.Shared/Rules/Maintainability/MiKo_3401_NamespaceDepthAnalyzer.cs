using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3401_NamespaceDepthAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3401";

        private const int MaxDepth = 7;

        public MiKo_3401_NamespaceDepthAnalyzer() : base(Id, SymbolKind.Namespace)
        {
        }

        protected override bool ShallAnalyze(INamespaceSymbol symbol) => symbol.IsGlobalNamespace is false;

        protected override IEnumerable<Diagnostic> Analyze(INamespaceSymbol symbol, Compilation compilation)
        {
            var depth = GetNamespaceDepth(symbol);

            return depth > MaxDepth
                       ? new[] { Issue(symbol, depth, MaxDepth) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static int GetNamespaceDepth(INamespaceSymbol symbol)
        {
            var depth = -1;

            var s = symbol;
            while (s != null)
            {
                depth++;

                s = s.ContainingNamespace;
            }

            return depth;
        }
    }
}
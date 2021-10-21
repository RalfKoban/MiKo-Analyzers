using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1059_ImplClassNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1059";

        internal const string WrongSuffixIndicator = "Indicator";
        private static readonly string[] WrongSuffixes = { "Impl", "Implementation", };

        public MiKo_1059_ImplClassNameAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            foreach (var wrongSuffix in WrongSuffixes)
            {
                if (symbolName.EndsWith(wrongSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    var issue = Issue(symbol, wrongSuffix, new Dictionary<string, string> { { WrongSuffixIndicator, wrongSuffix } });

                    return new[] { issue };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}
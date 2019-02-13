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

        private static readonly string[] WrongNames = { "Impl", "Implementation", };

        public MiKo_1059_ImplClassNameAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var symbolName = symbol.Name;
            foreach (var wrongName in WrongNames)
            {
                if (symbolName.EndsWith(wrongName, StringComparison.OrdinalIgnoreCase))
                {
                    return new[] { ReportIssue(symbol, wrongName) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}
using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1513_TypesWithExtendedSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1513";

        private static readonly string[] WrongSuffixes = { "Advanced", "Complex", "Enhanced", "Extended", "Simple", "Simplified" };

        public MiKo_1513_TypesWithExtendedSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            foreach (var suffix in WrongSuffixes)
            {
                var symbolName = symbol.Name.AsSpan();

                if (symbolName.EndsWith(suffix, StringComparison.Ordinal))
                {
                    var proposal = suffix.ConcatenatedWith(symbolName.WithoutSuffix(suffix));

                    return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}
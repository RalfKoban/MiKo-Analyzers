using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1004_EventNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1004";

        internal const string Suffix = "Event";

        public MiKo_1004_EventNameSuffixAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation)
        {
            if (symbol.Name.EndsWith(Suffix, StringComparison.Ordinal))
            {
                var proposal = symbol.Name.Without(Suffix);

                yield return Issue(symbol, proposal, CreateBetterNameProposal(proposal));
            }
        }
    }
}
using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1004_EventNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1004";

        internal const string Suffix = Constants.Names.Event;

        public MiKo_1004_EventNameSuffixAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation)
        {
            var eventName = symbol.Name;

            if (eventName.EndsWith(Suffix, StringComparison.Ordinal))
            {
                var proposal = NamesFinder.FindBetterEventName(eventName);

                return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}
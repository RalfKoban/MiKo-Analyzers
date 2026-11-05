using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1538_EventNamePrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1538";

        internal const string Prefix = Constants.Names.On;

        public MiKo_1538_EventNamePrefixAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.StartsWith(Prefix, StringComparison.Ordinal) && symbolName.Length > Prefix.Length && symbolName[Prefix.Length].IsUpperCase())
            {
                var proposal = NamesFinder.FindBetterEventName(symbolName);

                return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}
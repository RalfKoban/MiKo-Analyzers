using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1538_EventNamePrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1538";

        internal const string Prefix = "On";

        public MiKo_1538_EventNamePrefixAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name.AsSpan();

            if (symbolName.StartsWith(Prefix) && symbolName.Length > Prefix.Length && symbolName[Prefix.Length].IsUpperCase())
            {
                var proposal = symbolName.Slice(Prefix.Length).ToString();

                return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}
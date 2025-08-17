using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1013_NotifyMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1013";

        private const string CorrectStartingPhrase = "On";
        private const string StartingPhrase = "Notify";

        private static readonly string[] StartingPhrases = { StartingPhrase, CorrectStartingPhrase + StartingPhrase };

        public MiKo_1013_NotifyMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.StartsWithAny(StartingPhrases, StringComparison.OrdinalIgnoreCase))
            {
                // avoid situation that method has no name
                if (symbolName.Without(StartingPhrase).Length != 0)
                {
                    var proposal = symbolName.AsCachedBuilder()
                                             .ReplaceWithProbe(StartingPhrase, CorrectStartingPhrase)
                                             .ReplaceWithProbe(CorrectStartingPhrase + CorrectStartingPhrase, CorrectStartingPhrase)
                                             .ToStringAndRelease(); // may happen for "OnNotifyXyz"

                    return new[] { Issue(symbol, CreateBetterNameProposal(proposal)) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}
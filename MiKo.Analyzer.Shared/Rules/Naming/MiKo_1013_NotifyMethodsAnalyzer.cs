using System;
using System.Collections.Generic;
using System.Text;

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

        internal static string FindBetterName(IMethodSymbol method) => new StringBuilder(method.Name).ReplaceWithCheck(StartingPhrase, CorrectStartingPhrase)
                                                                                                     .ReplaceWithCheck(CorrectStartingPhrase + CorrectStartingPhrase, CorrectStartingPhrase) // may happen for "OnNotifyXyz"
                                                                                                     .ToString();

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.Name.StartsWithAny(StartingPhrases))
            {
                // avoid situation that method has no name
                if (symbol.Name.Without(StartingPhrase).Length != 0)
                {
                    yield return Issue(symbol);
                }
            }
        }
    }
}
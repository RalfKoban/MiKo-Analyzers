﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1067_PerformMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1067";

        private const string Phrase = "Perform";

        public MiKo_1067_PerformMethodsAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IMethodSymbol symbol)
        {
            var name = symbol.Name.Without(Phrase);

            return Verbalizer.TryMakeVerb(name, out var result) ? result : name;
        }

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (ContainsPhrase(methodName) && ContainsPhrase(methodName.Without("Performance").Without("Performed")))
            {
                yield return Issue(symbol);
            }
        }

        private static bool ContainsPhrase(string methodName) => methodName.Contains(Phrase, StringComparison.Ordinal);
    }
}
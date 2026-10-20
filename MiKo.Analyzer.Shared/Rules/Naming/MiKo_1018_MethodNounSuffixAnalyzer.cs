using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1018_MethodNounSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1018";

        public MiKo_1018_MethodNounSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol, Compilation compilation) => base.ShallAnalyze(symbol, compilation) && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => symbol.IsTestMethod() is false; // do not consider local functions inside tests

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol, Compilation compilation) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.StartsWith("Throw", StringComparison.Ordinal) is false && Verbalizer.TryMakeVerb(symbolName, out var betterName))
            {
                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}
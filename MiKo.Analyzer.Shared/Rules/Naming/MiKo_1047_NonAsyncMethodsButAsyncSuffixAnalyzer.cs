using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1047_NonAsyncMethodsButAsyncSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1047";

        public MiKo_1047_NonAsyncMethodsButAsyncSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsAsyncTaskBased() is false && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.IsAsyncTaskBased() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.AsyncSuffix, StringComparison.Ordinal))
            {
                var betterName = symbolName.WithoutSuffix(Constants.AsyncSuffix);

                yield return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
            }
            else if (symbolName.EndsWith(Constants.AsyncCoreSuffix, StringComparison.Ordinal))
            {
                var betterName = symbolName.WithoutSuffix(Constants.AsyncCoreSuffix) + Constants.Core;

                yield return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
            }
        }
    }
}
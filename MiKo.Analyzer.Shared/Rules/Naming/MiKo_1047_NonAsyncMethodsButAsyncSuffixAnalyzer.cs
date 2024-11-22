using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.IsAsyncTaskBased() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var betterName = FindBetterName(symbol.Name.AsSpan());

            if (betterName.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }

        private static string FindBetterName(ReadOnlySpan<char> symbolName)
        {
            if (symbolName.EndsWith(Constants.AsyncSuffix, StringComparison.Ordinal))
            {
                return symbolName.WithoutSuffix(Constants.AsyncSuffix).ToString();
            }

            if (symbolName.EndsWith(Constants.AsyncCoreSuffix, StringComparison.Ordinal))
            {
                return symbolName.WithoutSuffix(Constants.AsyncCoreSuffix).ConcatenatedWith(Constants.Core);
            }

            return null;
        }
    }
}
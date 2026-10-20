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

        protected override bool ShallAnalyze(IMethodSymbol symbol, Compilation compilation) => symbol?.IsAsyncTaskBased() is false && base.ShallAnalyze(symbol, compilation);

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol, Compilation compilation) => symbol?.IsAsyncTaskBased() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var betterName = FindBetterName(symbol.Name.AsSpan());

            if (betterName.IsNullOrWhiteSpace())
            {
                return Array.Empty<Diagnostic>();
            }

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }

        private static string FindBetterName(in ReadOnlySpan<char> symbolName)
        {
            if (symbolName.EndsWith(Constants.AsyncSuffix))
            {
                return symbolName.WithoutSuffix(Constants.AsyncSuffix).ToString();
            }

            if (symbolName.EndsWith(Constants.AsyncCoreSuffix))
            {
                return symbolName.WithoutSuffix(Constants.AsyncCoreSuffix).ConcatenatedWith(Constants.Core);
            }

            return null;
        }
    }
}
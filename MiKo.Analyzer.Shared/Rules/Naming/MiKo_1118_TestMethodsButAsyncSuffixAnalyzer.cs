using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1118_TestMethodsButAsyncSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1118";

        public MiKo_1118_TestMethodsButAsyncSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false;

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
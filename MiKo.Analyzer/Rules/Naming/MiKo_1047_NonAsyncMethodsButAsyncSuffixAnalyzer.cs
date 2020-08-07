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

        internal static string FindBetterName(IMethodSymbol symbol) => symbol.Name.WithoutSuffix(Constants.AsyncSuffix);

        protected override bool ShallAnalyze(IMethodSymbol method) => method.IsAsyncTaskBased() is false && base.ShallAnalyze(method);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            if (symbol.Name.EndsWith(Constants.AsyncSuffix, StringComparison.Ordinal))
            {
                var betterName = FindBetterName(symbol);
                return new[] { Issue(symbol, betterName) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}
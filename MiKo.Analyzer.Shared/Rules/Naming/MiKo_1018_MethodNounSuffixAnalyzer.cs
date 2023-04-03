using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1018_MethodNounSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1018";

        public const string BetterName = "BETTER_NAME";

        public MiKo_1018_MethodNounSuffixAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(Diagnostic diagnostic, ISymbol symbol)
        {
            var betterName = diagnostic.Properties[BetterName];

            return betterName.IsNullOrWhiteSpace() ? symbol.Name : betterName;
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => symbol.IsTestMethod()
                                                                                                                               ? Enumerable.Empty<Diagnostic>() // do not consider local functions inside tests
                                                                                                                               : base.AnalyzeLocalFunctions(symbol, compilation);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            if (Verbalizer.TryMakeVerb(symbol.Name, out var betterName))
            {
                yield return Issue(
                                   symbol,
                                   betterName,
                                   new Dictionary<string, string>
                                       {
                                           { BetterName, betterName },
                                       });
            }
        }
    }
}
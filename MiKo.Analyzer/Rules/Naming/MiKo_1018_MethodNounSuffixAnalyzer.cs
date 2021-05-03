using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1018_MethodNounSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1018";

        public MiKo_1018_MethodNounSuffixAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(ISymbol symbol) => NamesFinder.TryMakeVerb(symbol.Name, out var betterName) ? betterName : symbol.Name;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => NamesFinder.TryMakeVerb(symbol.Name, out var betterName)
                                                                                            ? new[] { Issue(symbol, betterName) }
                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1105_TestMethodsPascalCasingAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1105";

        private static readonly Regex PascalCasingRegex = new Regex("[a-z]+[A-Z]+");

        public MiKo_1105_TestMethodsPascalCasingAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => base.ShallAnalyze(method) && method.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => PascalCasingRegex.IsMatch(symbol.Name) && !symbol.Name.Contains("_")
                                                                                            ? new[] { Issue(symbol) }
                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}
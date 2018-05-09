using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1101_TestClassesSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1101";

        private const string Suffix = "Tests";

        public MiKo_1101_TestClassesSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => symbol.Name.EndsWith(Suffix, StringComparison.Ordinal)
                                                                                           ? Enumerable.Empty<Diagnostic>()
                                                                                           : new[] { ReportIssue(symbol, symbol.Name + Suffix) };
    }
}
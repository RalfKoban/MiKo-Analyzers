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

        public MiKo_1101_TestClassesSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            const string TestSuffix = "Tests";

            if (symbol.IsTestClass() && !symbol.Name.EndsWith(TestSuffix, StringComparison.Ordinal))
                return new[] { ReportIssue(symbol, symbol.Name + TestSuffix) };

            return Enumerable.Empty<Diagnostic>();
        }
    }
}
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

        protected override IEnumerable<Diagnostic> AnalyzeOrdinaryMethod(IMethodSymbol symbol) => symbol.Name.EndsWith(Constants.AsyncSuffix, StringComparison.Ordinal) && !symbol.IsAsyncTaskBased()
                                                                                                      ? new[] { ReportIssue(symbol, symbol.Name.WithoutSuffix(Constants.AsyncSuffix)) }
                                                                                                      : Enumerable.Empty<Diagnostic>();
    }
}
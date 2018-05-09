using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1046_AsyncMethodsSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1046";

        public MiKo_1046_AsyncMethodsSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => base.ShallAnalyze(method) && method.IsAsyncTaskBased();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => symbol.Name.EndsWith(Constants.AsyncSuffix, StringComparison.Ordinal)
                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                        : new[] { ReportIssue(symbol, symbol.Name + Constants.AsyncSuffix) };
    }
}
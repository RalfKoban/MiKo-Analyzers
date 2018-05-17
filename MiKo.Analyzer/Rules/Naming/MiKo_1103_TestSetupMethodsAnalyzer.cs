using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1103_TestSetupMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1103";

        private const string Marker = "PrepareTest";

        public MiKo_1103_TestSetupMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => base.ShallAnalyze(method) && method.IsTestSetupMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => symbol.Name == Marker
                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                        : new[] { ReportIssue(symbol, Marker) };
    }
}
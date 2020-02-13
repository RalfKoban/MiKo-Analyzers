using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1105_OneTimeTestSetupMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1105";

        private const string Marker = "PrepareTestEnvironment";

        public MiKo_1105_OneTimeTestSetupMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => base.ShallAnalyze(method) && method.IsTestOneTimeSetUpMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => symbol.Name == Marker
                                                                                            ? Enumerable.Empty<Diagnostic>()
                                                                                            : new[] { Issue(symbol, Marker) };
    }
}
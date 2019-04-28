using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1104_TestTeardownMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1104";

        private const string Marker = "CleanupTest";

        public MiKo_1104_TestTeardownMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => base.ShallAnalyze(method) && method.IsTestTeardownMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => symbol.Name == Marker
                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                        : new[] { Issue(symbol, Marker) };
    }
}
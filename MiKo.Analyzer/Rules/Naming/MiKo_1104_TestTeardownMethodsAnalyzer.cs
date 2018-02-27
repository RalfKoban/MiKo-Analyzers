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

        public MiKo_1104_TestTeardownMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol)
        {
            const string Marker = "CleanupTest";

            if (symbol.IsTestTeardownMethod() && symbol.Name != Marker)
                return new[] { ReportIssue(symbol, Marker) };

            return Enumerable.Empty<Diagnostic>();
        }
    }
}
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3101_TestClassesHaveTestsAnalyzer : TestsMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3101";

        public MiKo_3101_TestClassesHaveTestsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeTestType(INamedTypeSymbol symbol) => GetTestMethods(symbol).Any()
                                                                                               ? Enumerable.Empty<Diagnostic>()
                                                                                               : new[] { ReportIssue(symbol) };
    }
}
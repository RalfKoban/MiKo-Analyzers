using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3101_TestClassesHaveTestsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3101";

        public MiKo_3101_TestClassesHaveTestsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsPartial() is false && symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol) => GetTestMethods(symbol).Any()
                                                                                           ? Enumerable.Empty<Diagnostic>()
                                                                                           : new[] { Issue(symbol) };

        private static IEnumerable<IMethodSymbol> GetTestMethods(INamedTypeSymbol symbol)
        {
            var typeSymbols = symbol.IncludingAllBaseTypes().Concat(symbol.IncludingAllNestedTypes()).Distinct();

            return typeSymbols
                       .SelectMany(_ => _.GetMembers().OfType<IMethodSymbol>())
                       .Where(_ => _.MethodKind == MethodKind.Ordinary)
                       .Where(_ => _.IsTestMethod());
        }
    }
}
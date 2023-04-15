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

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsPartial() is false && symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (GetTestMethods(symbol).None())
            {
                return new[] { Issue(symbol) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static IEnumerable<IMethodSymbol> GetTestMethods(INamedTypeSymbol symbol)
        {
            var typeSymbols = symbol.IncludingAllBaseTypes().Concat(symbol.IncludingAllNestedTypes()).Where(_ => _.CanBeReferencedByName).Distinct(SymbolEqualityComparer.Default).Cast<ITypeSymbol>();

            return typeSymbols
                       .SelectMany(_ => _.GetMethods(MethodKind.Ordinary))
                       .Where(_ => _.IsTestMethod());
        }
    }
}
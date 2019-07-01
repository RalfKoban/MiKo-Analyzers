using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1100_TestClassesPrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1100";

        private const string TestsSuffix = "Tests";

        public MiKo_1100_TestClassesPrefixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var typeUnderTest = symbol.GetTypeUnderTestType();
            if (typeUnderTest is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var typeUnderTestName = GetTypeUnderTestName(symbol, typeUnderTest);
            if (typeUnderTestName is null)
            {
                return Enumerable.Empty<Diagnostic>(); // ignore generic class or struct constraint
            }

            var className = symbol.Name;
            return className.StartsWith(typeUnderTestName, StringComparison.Ordinal)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, typeUnderTestName + TestsSuffix) };
        }

        private static string GetTypeUnderTestName(INamedTypeSymbol testClass, ITypeSymbol typeUnderTest)
        {
            if (typeUnderTest.TypeKind != TypeKind.TypeParameter)
            {
                return typeUnderTest.Name;
            }

            var typeParameter = (ITypeParameterSymbol)testClass.TypeArguments[0];

            // for generic class or struct constraints there is no constraint type available
            var constraint = typeParameter.ConstraintTypes.FirstOrDefault();
            return constraint?.Name;
        }
    }
}
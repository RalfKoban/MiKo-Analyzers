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
            var classUnderTest = symbol.GetClassUnderTestType();
            if (classUnderTest is null)
                return Enumerable.Empty<Diagnostic>();

            var classUnderTestName = classUnderTest.Name;

            var className = symbol.Name;
            return className.StartsWith(classUnderTestName, StringComparison.Ordinal)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, classUnderTestName + TestsSuffix) };
        }
    }
}
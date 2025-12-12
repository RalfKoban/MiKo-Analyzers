using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4106_TestAssemblyWideSetUpMethodOrderingAnalyzer : TestMethodsOrderingAnalyzer
    {
        public const string Id = "MiKo_4106";

        public MiKo_4106_TestAssemblyWideSetUpMethodOrderingAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override IMethodSymbol GetMethod(INamedTypeSymbol symbol) => symbol.GetMethods(MethodKind.Ordinary).FirstOrDefault(_ => _.IsTestAssemblyWideSetUpMethod());

        protected override int GetExpectedMethodIndex(IEnumerable<IMethodSymbol> methods) => 0;
    }
}
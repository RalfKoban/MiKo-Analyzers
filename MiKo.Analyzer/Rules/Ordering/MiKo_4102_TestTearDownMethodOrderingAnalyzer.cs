using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4102_TestTearDownMethodOrderingAnalyzer : TestMethodsOrderingAnalyzer
    {
        public const string Id = "MiKo_4102";

        public MiKo_4102_TestTearDownMethodOrderingAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override IMethodSymbol GetMethod(INamedTypeSymbol symbol) => symbol.GetMembers()
                                                                                     .OfType<IMethodSymbol>()
                                                                                     .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                     .FirstOrDefault(_ => _.IsTestTearDownMethod());

        protected override int GetExpectedMethodIndex(IEnumerable<IMethodSymbol> methods)
        {
            var index = 0;

            if (methods.Any(_ => _.IsTestOneTimeSetUpMethod()))
            {
                index++;
            }

            if (methods.Any(_ => _.IsTestOneTimeTearDownMethod()))
            {
                index++;
            }

            if (methods.Any(_ => _.IsTestSetUpMethod()))
            {
                index++;
            }

            return index;
        }
    }
}
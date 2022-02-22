using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4104_TestOneTimeTearDownMethodOrderingAnalyzer : TestMethodsOrderingAnalyzer
    {
        public const string Id = "MiKo_4104";

        public MiKo_4104_TestOneTimeTearDownMethodOrderingAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override IMethodSymbol GetMethod(INamedTypeSymbol symbol) => symbol.GetMethods(MethodKind.Ordinary).FirstOrDefault(_ => _.IsTestOneTimeTearDownMethod());

        protected override int GetExpectedMethodIndex(IEnumerable<IMethodSymbol> methods)
        {
            var index = 0;

            if (methods.Any(_ => _.IsTestOneTimeSetUpMethod()))
            {
                index++;
            }

            return index;
        }
    }
}
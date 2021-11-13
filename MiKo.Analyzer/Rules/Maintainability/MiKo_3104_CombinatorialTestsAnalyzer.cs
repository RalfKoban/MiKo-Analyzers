using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3104_CombinatorialTestsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3104";

        private static readonly string[] AttributeNames = { "ValueSource", "ValueSourceAttribute", "Values", "ValuesAttribute", "Range", "RangeAttribute", "Random", "RandomAttribute", };

        public MiKo_3104_CombinatorialTestsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol)
        {
            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
            {
                if (IsSequential(method))
                {
                    // both attributes combined don't make sense
                    if (IsCombinatorial(method))
                    {
                        yield return Issue(method);
                    }
                }
                else if (IsPairwise(method))
                {
                    // both attributes combined don't make sense
                    if (IsCombinatorial(method))
                    {
                        yield return Issue(method);
                    }
                }
                else if (IsCombinatorial(method))
                {
                    var count = CountApplicableParameters(method);

                    // less than 2 ValueSource
                    if (count < 2)
                    {
                        yield return Issue(method);
                    }
                }
                else
                {
                    // attribute is optional, considered as default even if not applied, so nothing to report
                }
            }
        }

        private static bool IsCombinatorial(IMethodSymbol method)
        {
            foreach (var name in method.GetAttributeNames())
            {
                switch (name)
                {
                    case "Combinatorial":
                    case "CombinatorialAttribute":
                        return true;
                }
            }

            return false;
        }

        private static bool IsPairwise(IMethodSymbol method)
        {
            foreach (var name in method.GetAttributeNames())
            {
                switch (name)
                {
                    case "Pairwise":
                    case "PairwiseAttribute":
                        return true;
                }
            }

            return false;
        }

        private static bool IsSequential(IMethodSymbol method)
        {
            foreach (var name in method.GetAttributeNames())
            {
                switch (name)
                {
                    case "Sequential":
                    case "SequentialAttribute":
                        return true;
                }
            }

            return false;
        }

        private static int CountApplicableParameters(IMethodSymbol method)
        {
            return method.Parameters
                         .SelectMany(_ => _.GetAttributeNames())
                         .Count(_ => _.EqualsAny(AttributeNames));
        }
    }
}
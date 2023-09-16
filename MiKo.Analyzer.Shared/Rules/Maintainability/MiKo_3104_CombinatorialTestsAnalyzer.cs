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

        private static readonly HashSet<string> AttributeNames = new HashSet<string>
                                                                     {
                                                                         "ValueSource",
                                                                         "ValueSourceAttribute",
                                                                         "Values",
                                                                         "ValuesAttribute",
                                                                         "Range",
                                                                         "RangeAttribute",
                                                                         "Random",
                                                                         "RandomAttribute",
                                                                     };

        private static readonly HashSet<string> CombinatorialAttributes = new HashSet<string>
                                                                              {
                                                                                  "Combinatorial",
                                                                                  "CombinatorialAttribute",
                                                                              };

        private static readonly HashSet<string> PairwiseAttributes = new HashSet<string>
                                                                         {
                                                                             "Pairwise",
                                                                             "PairwiseAttribute",
                                                                         };

        private static readonly HashSet<string> SequentialAttributes = new HashSet<string>
                                                                           {
                                                                               "Sequential",
                                                                               "SequentialAttribute",
                                                                           };

        public MiKo_3104_CombinatorialTestsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            foreach (var method in symbol.GetNamedMethods().Where(_ => _.IsPubliclyVisible()))
            {
                if (IsSequential(method))
                {
                    // both attributes combined do not make sense
                    if (IsCombinatorial(method))
                    {
                        yield return Issue(method);
                    }
                }
                else if (IsPairwise(method))
                {
                    // both attributes combined do not make sense
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

        private static bool IsCombinatorial(IMethodSymbol method) => method.HasAttribute(CombinatorialAttributes);

        private static bool IsPairwise(IMethodSymbol method) => method.HasAttribute(PairwiseAttributes);

        private static bool IsSequential(IMethodSymbol method) => method.HasAttribute(SequentialAttributes);

        private static int CountApplicableParameters(IMethodSymbol method) => method.Parameters.SelectMany(_ => _.GetAttributeNames()).Count(AttributeNames.Contains);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3106_CombinatorialTestsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3106";

        public MiKo_3106_CombinatorialTestsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.IsTestClass()
                                                                                               ? AnalyzeTestType(symbol).ToList()
                                                                                               : Enumerable.Empty<Diagnostic>();

        private IEnumerable<Diagnostic> AnalyzeTestType(INamedTypeSymbol symbol)
        {
            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
            {
                var count = CountValueSources(method);

                if (IsCombinatorial(method))
                {
                    // less than 2 ValueSource
                    if (count < 2)
                        yield return ReportIssue(method);
                }
                else if (IsSequential(method))
                {
                    // ignore
                }
                else
                {
                    if (count > 1)
                        yield return ReportIssue(method);
                }
            }
        }

        private static bool IsCombinatorial(IMethodSymbol method)
        {
            foreach (var name in method.GetAttributes().Select(_ => _.AttributeClass.Name))
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

        private static bool IsSequential(IMethodSymbol method)
        {
            foreach (var name in method.GetAttributes().Select(_ => _.AttributeClass.Name))
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

        private static int CountValueSources(IMethodSymbol method)
        {
            return method.Parameters
                         .SelectMany(_ => _.GetAttributes().Select(__ => __.AttributeClass.Name))
                         .Count(_ => _.EqualsAny("ValueSource", "ValueSourceAttribute", "Values", "ValuesAttribute"));
        }
    }
}
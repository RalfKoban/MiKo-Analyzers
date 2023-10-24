using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3118_TestMethodsDoNotUseSpecificLinqMethodsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3118";

        private static readonly HashSet<string> ProblematicMethods = new HashSet<string>
                                                                         {
                                                                             nameof(Enumerable.Skip),
                                                                             "SkipLast",
                                                                             nameof(Enumerable.SkipWhile),
                                                                             nameof(Enumerable.Take),
                                                                             "TakeLast",
                                                                             nameof(Enumerable.TakeWhile),
                                                                             nameof(Enumerable.Single),
                                                                             nameof(Enumerable.SingleOrDefault),
                                                                             nameof(Enumerable.FirstOrDefault),
                                                                             nameof(Enumerable.LastOrDefault),
                                                                             nameof(Enumerable.Any),
                                                                             nameof(Enumerable.All),
                                                                         };

        public MiKo_3118_TestMethodsDoNotUseSpecificLinqMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var methodSyntax = symbol.GetSyntax();

            foreach (var maes in methodSyntax.DescendantNodes<MemberAccessExpressionSyntax>(_ => _.Parent is InvocationExpressionSyntax))
            {
                var node = maes.Name;
                var name = node.GetName();

                if (ProblematicMethods.Contains(name))
                {
                    yield return Issue(name, node);
                }
            }
        }
    }
}
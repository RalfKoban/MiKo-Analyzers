using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1046_AsyncMethodsSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1046";

        private static readonly HashSet<string> TaskFactoryMethods = typeof(TaskFactory).GetMethods().ToHashSet(_ => _.Name);

        public MiKo_1046_AsyncMethodsSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsAsyncTaskBased()
                                                                      && base.ShallAnalyze(symbol)
                                                                      && symbol.IsTestMethod() is false
                                                                      && symbol.IsTestSetUpMethod() is false
                                                                      && symbol.IsTestTearDownMethod() is false;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.IsAsyncTaskBased();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.EndsWith(Constants.AsyncSuffix, StringComparison.Ordinal) || TaskFactoryMethods.Contains(methodName))
            {
                // nothing to report here
            }
            else
            {
                var betterName = symbol.Name + Constants.AsyncSuffix;

                yield return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
            }
        }
    }
}
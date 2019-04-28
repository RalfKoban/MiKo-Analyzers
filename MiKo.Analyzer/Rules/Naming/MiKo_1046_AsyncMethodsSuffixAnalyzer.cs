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

        private static readonly HashSet<string> TaskFactoryMethods = typeof(TaskFactory).GetMethods().Select(_ => _.Name).ToHashSet();

        public MiKo_1046_AsyncMethodsSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => method.IsAsyncTaskBased() && base.ShallAnalyze(method) && !method.IsTestMethod() && !method.IsTestSetupMethod() && !method.IsTestTeardownMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            var methodName = symbol.Name;

            return methodName.EndsWith(Constants.AsyncSuffix, StringComparison.Ordinal) || TaskFactoryMethods.Contains(methodName)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, methodName + Constants.AsyncSuffix) };
        }
    }
}
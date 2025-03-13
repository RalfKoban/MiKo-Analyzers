﻿using System;
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

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.IsAsyncTaskBased();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.EndsWith(Constants.AsyncSuffix, StringComparison.Ordinal)
             || methodName.EndsWith(Constants.AsyncCoreSuffix, StringComparison.Ordinal)
             || TaskFactoryMethods.Contains(methodName))
            {
                // nothing to report here
                return Array.Empty<Diagnostic>();
            }

            if (symbol.IsStatic && methodName == "Main")
            {
                // nothing to report here for the main method as that is the entry point of an application and as to be named 'Main'
                return Array.Empty<Diagnostic>();
            }

            var betterName = symbol.Name + Constants.AsyncSuffix;

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}
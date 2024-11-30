﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1107_TestMethodsPascalCasingAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1107";

        public MiKo_1107_TestMethodsPascalCasingAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false; // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (HasIssue(symbolName))
            {
                var betterName = NamesFinder.FindBetterTestName(symbolName, symbol);

                return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static bool HasIssue(string symbolName)
        {
            if (symbolName.IsPascalCasing() is false)
            {
                return false;
            }

            if (symbolName.Contains(Constants.Underscore))
            {
                var underlinesNr = symbolName.Count(_ => _ is Constants.Underscore);
                var upperCasesNr = symbolName.Count(_ => _.IsUpperCase());

                var diff = underlinesNr - upperCasesNr;

                if (diff >= 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
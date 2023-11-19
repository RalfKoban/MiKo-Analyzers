﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1111_TestMethodsWithoutParametersNotSuffixedWithUnderscoreAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1111";

        public MiKo_1111_TestMethodsWithoutParametersNotSuffixedWithUnderscoreAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        internal static string FindBetterName(IMethodSymbol symbol) => symbol.Name.TrimEnd(Constants.Underscore);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.Parameters.Length == 0 && symbol.IsTestMethod();

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        protected override IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>(); // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var endsWithUnderscore = symbol.Name.EndsWith(Constants.Underscore);

            return endsWithUnderscore
                   ? new[] { Issue(symbol) }
                   : Enumerable.Empty<Diagnostic>();
        }
    }
}
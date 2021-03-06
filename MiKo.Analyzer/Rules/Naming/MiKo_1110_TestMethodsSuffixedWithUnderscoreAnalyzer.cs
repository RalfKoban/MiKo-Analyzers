﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1110_TestMethodsSuffixedWithUnderscoreAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1110";

        public MiKo_1110_TestMethodsSuffixedWithUnderscoreAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IMethodSymbol symbol) => symbol.Name + "_";

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.Parameters.Length > 0 && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            var endsWithUnderscore = symbol.Name.EndsWith("_", StringComparison.OrdinalIgnoreCase);

            return endsWithUnderscore
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol) };
        }
    }
}
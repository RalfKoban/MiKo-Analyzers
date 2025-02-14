﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1102_TestMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1102";

        private const string TestMarker = "Test";
        private const string TestCaseMarker = "TestCase";

        public MiKo_1102_TestMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.ContainingSymbol is IMethodSymbol method && ShallAnalyze(method);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.Contains(TestMarker))
            {
                var marker = GetTestMarker(symbolName);
                var betterName = FindBetterName(symbolName, marker);

                return new[] { Issue(symbol, marker, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string GetTestMarker(string symbolName)
        {
            var testCase = symbolName.Contains(TestCaseMarker, StringComparison.OrdinalIgnoreCase);

            return testCase ? TestCaseMarker : TestMarker;
        }

        private static string FindBetterName(string symbolName, string marker)
        {
            var phrases = new[]
                              {
                                  marker.SurroundedWith(Constants.Underscore),
                                  Constants.Underscore + marker,
                                  marker + Constants.Underscore,
                                  marker,
                              };

            return symbolName.Without(phrases);
        }
    }
}
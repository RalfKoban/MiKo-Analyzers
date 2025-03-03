﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1101_TestClassesSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1101";

        private const string TestSuffix = "Test";

        public MiKo_1101_TestClassesSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var className = symbol.Name;

            if (className.EndsWith(Constants.TestsSuffix, StringComparison.Ordinal))
            {
                return Array.Empty<Diagnostic>();
            }

            var name = FindBetterName(className);

            return new[] { Issue(symbol, name, CreateBetterNameProposal(name)) };
        }

        private static string FindBetterName(string className)
        {
            var suffix = className.EndsWith(TestSuffix, StringComparison.Ordinal) ? "s" : Constants.TestsSuffix;

            return className + suffix;
        }
    }
}
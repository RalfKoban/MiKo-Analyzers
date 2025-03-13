﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1106_OneTimeTestTeardownMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1106";

        private const string ExpectedName = "CleanupTestEnvironment";

        public MiKo_1106_OneTimeTestTeardownMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestOneTimeTearDownMethod();

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false; // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => symbol.Name == ExpectedName
                                                                                                                 ? Array.Empty<Diagnostic>()
                                                                                                                 : new[] { Issue(symbol, ExpectedName, CreateBetterNameProposal(ExpectedName)) };
    }
}
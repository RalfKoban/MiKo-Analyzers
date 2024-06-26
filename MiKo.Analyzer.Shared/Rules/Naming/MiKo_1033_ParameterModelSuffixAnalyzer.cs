﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1033_ParameterModelSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1033";

        public MiKo_1033_ParameterModelSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => base.ShallAnalyze(symbol) && symbol.GetEnclosingMethod().IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => AnalyzeEntityMarkers(symbol);
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1055_DependencyPropertyFieldSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1055";

        public MiKo_1055_DependencyPropertyFieldSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyProperty();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => symbol.Name.EndsWith(Constants.DependencyProperty.FieldSuffix, StringComparison.Ordinal)
                                                                                                                ? Enumerable.Empty<Diagnostic>()
                                                                                                                : new[] { Issue(symbol, symbol.Name + Constants.DependencyProperty.FieldSuffix) };
    }
}
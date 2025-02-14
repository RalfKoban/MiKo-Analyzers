﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1041_FieldCollectionSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1041";

        public MiKo_1041_FieldCollectionSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.ContainingType?.IsEnum() is false; // ignore enum definitions

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var diagnostic = AnalyzeCollectionSuffix(symbol);

            return diagnostic != null
                   ? new[] { diagnostic }
                   : Array.Empty<Diagnostic>();
        }
    }
}
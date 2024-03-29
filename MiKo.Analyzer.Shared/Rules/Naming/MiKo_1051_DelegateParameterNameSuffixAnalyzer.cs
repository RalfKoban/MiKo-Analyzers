﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1051_DelegateParameterNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1051";

        private static readonly string[] WrongNames = { "Action", "Delegate", "Func" };

        public MiKo_1051_DelegateParameterNameSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => symbol.Type.TypeKind == TypeKind.Delegate || (symbol.Type.TypeKind == TypeKind.Class && symbol.Type.ToString() == TypeNames.Delegate);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            if (symbol.Name.EndsWithAny(WrongNames))
            {
                yield return Issue(symbol, CreateBetterNameProposal("callback"));
            }
        }
    }
}
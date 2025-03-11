﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1503_MethodsWithCounterSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1503";

        public MiKo_1503_MethodsWithCounterSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.TypeKind == TypeKind.Struct && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => symbol.ReturnType.TypeKind == TypeKind.Struct;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.ReturnType.TypeKind == TypeKind.Struct;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.Names.Counter, StringComparison.OrdinalIgnoreCase))
            {
                var betterName = FindBetterName(symbolName);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string symbolName)
        {
            var nameSpan = symbolName.AsSpan();

            if (nameSpan.Contains("Increment") || nameSpan.Contains("Decrement"))
            {
                return symbolName.WithoutSuffix(Constants.Names.Counter);
            }

            return symbolName.WithoutSuffix("er");
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1044_CommandSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1044";

        private const string Suffix = "Command";

        private static readonly string[] SingleSuffix = { Suffix };
        private static readonly string[] Suffixes = { "_command", Suffix };

        public MiKo_1044_CommandSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Field);

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsCommand();

        protected override bool ShallAnalyze(IMethodSymbol method) => ShallAnalyze(method.ReturnType);

        protected override bool ShallAnalyze(IPropertySymbol symbol) => ShallAnalyze(symbol.Type);

        protected override bool ShallAnalyze(IFieldSymbol symbol) => ShallAnalyze(symbol.Type);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => AnalyzeNames(symbol, Suffixes);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol) => AnalyzeNames(symbol, SingleSuffix);

        private IEnumerable<Diagnostic> AnalyzeNames(ISymbol symbol, IEnumerable<string> suffixes) => suffixes.Any(_ => symbol.Name.EndsWith(_, StringComparison.Ordinal))
                                                                                                          ? Enumerable.Empty<Diagnostic>()
                                                                                                          : new[] { Issue(symbol, Suffix) };
    }
}
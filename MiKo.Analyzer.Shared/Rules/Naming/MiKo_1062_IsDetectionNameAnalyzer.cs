﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    //// <seealso cref="MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer"/>
    //// <seealso cref="MiKo_1073_BooleanFieldNamedAsQuestionAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1062_IsDetectionNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1062";

        private static readonly string[] Prefixes = { "Can", "Has", "Contains" };

        public MiKo_1062_IsDetectionNameAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property, SymbolKind.Field);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsBoolean();

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetReturnType()?.IsBoolean() is true;

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsBoolean();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeCamelCase(symbol, symbol.Name.AsSpan(), 4);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeCamelCase(symbol, symbol.Name.AsSpan(), 3);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name.AsSpan();

            foreach (var prefix in Constants.Markers.FieldPrefixes)
            {
                if (prefix.Length > 0 && symbolName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    symbolName = symbolName.Slice(prefix.Length);

                    break;
                }
            }

            return AnalyzeCamelCase(symbol, symbolName, 2);
        }

        private static bool ViolatesLimit(in ReadOnlySpan<char> name, in ushort limit) => name.StartsWithAny(Prefixes, StringComparison.OrdinalIgnoreCase) && name.HasUpperCaseLettersAbove(limit);

        private Diagnostic[] AnalyzeCamelCase(ISymbol symbol, in ReadOnlySpan<char> symbolName, in ushort limit) => ViolatesLimit(symbolName, limit)
                                                                                                                    ? new[] { Issue(symbol, limit) }
                                                                                                                    : Array.Empty<Diagnostic>();
    }
}
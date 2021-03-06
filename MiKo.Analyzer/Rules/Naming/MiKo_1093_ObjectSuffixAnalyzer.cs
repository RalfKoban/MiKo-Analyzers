﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1093_ObjectSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1093";

        internal static readonly string[] WrongSuffixes =
            {
                "Object",
                "Struct",
            };

        public MiKo_1093_ObjectSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static string FindBetterName(ISymbol symbol)
        {
            var newName = symbol.Name;
            foreach (var suffix in WrongSuffixes.Where(_ => newName.EndsWith(_, StringComparison.OrdinalIgnoreCase)))
            {
                newName = newName.WithoutSuffix(suffix);
            }

            // do it twice as maybe the name is a combination of both words
            foreach (var suffix in WrongSuffixes.Where(_ => newName.EndsWith(_, StringComparison.OrdinalIgnoreCase)))
            {
                newName = newName.WithoutSuffix(suffix);
            }

            return newName;
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Namespace, SymbolKind.NamedType, SymbolKind.Property, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => AnalyzeName(symbol);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            foreach (var suffix in WrongSuffixes)
            {
                if (symbolName.EndsWith(suffix, StringComparison.Ordinal))
                {
                    yield return Issue(symbol, suffix);
                }
            }
        }
    }
}
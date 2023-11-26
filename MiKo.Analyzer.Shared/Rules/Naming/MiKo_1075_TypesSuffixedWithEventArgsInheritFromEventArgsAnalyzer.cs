﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1075";

        public MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static string FindBetterName(ISymbol symbol, Diagnostic diagnostic) => FindBetterName(symbol);

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Class && symbol.Name.EndsWith(nameof(EventArgs), StringComparison.Ordinal);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => symbol.IsEventArgs()
                                                                                                                    ? Enumerable.Empty<Diagnostic>()
                                                                                                                    : new[] { Issue(symbol, FindBetterName(symbol)) };

        private static string FindBetterName(ISymbol symbol) => symbol.Name.Without(nameof(EventArgs));
    }
}
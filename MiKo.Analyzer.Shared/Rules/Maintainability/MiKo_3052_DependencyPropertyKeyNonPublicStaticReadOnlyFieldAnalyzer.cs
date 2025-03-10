﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3052_DependencyPropertyKeyNonPublicStaticReadOnlyFieldAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3052";

        public MiKo_3052_DependencyPropertyKeyNonPublicStaticReadOnlyFieldAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyPropertyKey();

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation) => symbol.IsStatic && symbol.IsReadOnly && symbol.DeclaredAccessibility != Accessibility.Public
                                                                                                            ? Array.Empty<Diagnostic>()
                                                                                                            : new[] { Issue(symbol) };
    }
}
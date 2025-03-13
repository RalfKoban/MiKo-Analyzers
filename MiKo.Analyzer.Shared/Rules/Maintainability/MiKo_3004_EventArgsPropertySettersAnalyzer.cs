﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3004_EventArgsPropertySettersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3004";

        public MiKo_3004_EventArgsPropertySettersAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.ContainingType?.IsEventArgs() is true;

        protected override IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation)
        {
            if (symbol.IsReadOnly)
            {
                return Array.Empty<Diagnostic>();
            }

            var setter = symbol.SetMethod;

            if (setter is null)
            {
                return Array.Empty<Diagnostic>();
            }

            return setter.DeclaredAccessibility != Accessibility.Private
                   ? new[] { Issue(setter) }
                   : Array.Empty<Diagnostic>();
        }
    }
}
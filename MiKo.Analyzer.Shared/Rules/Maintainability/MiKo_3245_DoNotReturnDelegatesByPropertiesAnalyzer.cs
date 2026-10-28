using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3245_DoNotReturnDelegatesByPropertiesAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3245";

        public MiKo_3245_DoNotReturnDelegatesByPropertiesAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol, Compilation compilation) => base.ShallAnalyze(symbol, compilation) && symbol.IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation) => symbol.GetReturnType()?.TypeKind is TypeKind.Delegate
                                                                                                               ? new[] { Issue(symbol) }
                                                                                                               : Array.Empty<Diagnostic>();
    }
}
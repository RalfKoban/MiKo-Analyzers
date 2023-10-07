using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3055_ViewModelImplementsINotifyPropertyChangedAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3055";

        public MiKo_3055_ViewModelImplementsINotifyPropertyChangedAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Class
                                                                      && symbol.Name.EndsWithAny(Constants.Markers.ViewModels, StringComparison.Ordinal)
                                                                      && symbol.IsTestClass() is false;

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.Implements<INotifyPropertyChanged>() is false)
            {
                yield return Issue(symbol);
            }
        }
    }
}
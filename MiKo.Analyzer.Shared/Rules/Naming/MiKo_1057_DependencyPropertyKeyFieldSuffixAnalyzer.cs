using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1057";

        public MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyPropertyKey();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.DependencyPropertyKey.FieldSuffix, StringComparison.Ordinal))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var propertyNames = NamesFinder.FindPropertyNames(symbol, Constants.DependencyPropertyKey.FieldSuffix, Constants.DependencyProperty.RegisterReadOnly);
            var name = propertyNames.Any() ? propertyNames.First() : symbolName;
            var betterName = name + Constants.DependencyPropertyKey.FieldSuffix;

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}
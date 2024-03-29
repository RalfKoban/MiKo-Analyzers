using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1055_DependencyPropertyFieldSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1055";

        private const string Suffix = Constants.DependencyProperty.FieldSuffix;

        public MiKo_1055_DependencyPropertyFieldSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyProperty();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Suffix, StringComparison.Ordinal) is false)
            {
                var propertyNames = NamesFinder.FindPropertyNames(symbol, Suffix, Constants.DependencyProperty.Register);
                var name = propertyNames.Any() ? propertyNames.First() : symbolName;
                var betterName = name + Suffix;

                yield return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
            }
        }
    }
}
using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1504_PropertiesWithCounterSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1504";

        public MiKo_1504_PropertiesWithCounterSuffixAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => base.ShallAnalyze(symbol) && symbol.Type.TypeKind is TypeKind.Struct;

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.Names.Counter, StringComparison.OrdinalIgnoreCase))
            {
                var betterName = FindBetterName(symbolName);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name)
        {
            if (name.Equals(Constants.Names.Counter, StringComparison.OrdinalIgnoreCase))
            {
                return "Count";
            }

            var length = Constants.Names.Counter.Length;

            return "Counted" + Pluralizer.MakePluralName(name.AsCachedBuilder().Remove(name.Length - length, length).ToUpperCaseAt(0).ToString());
        }
    }
}
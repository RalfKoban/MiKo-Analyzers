using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1507_ParametersWithCounterSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1507";

        public MiKo_1507_ParametersWithCounterSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => symbol.Type.TypeKind is TypeKind.Struct && symbol.Name.EndsWith(Constants.Names.Counter, StringComparison.OrdinalIgnoreCase);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var betterName = FindBetterName(symbol.Name);

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }

        private static string FindBetterName(string symbolName)
        {
            if (symbolName.Equals(Constants.Names.Counter, StringComparison.OrdinalIgnoreCase))
            {
                return "count";
            }

            var length = Constants.Names.Counter.Length;

            return "counted" + Pluralizer.MakePluralName(symbolName.AsCachedBuilder().Remove(symbolName.Length - length, length).ToUpperCaseAt(0).ToString());
        }
    }
}
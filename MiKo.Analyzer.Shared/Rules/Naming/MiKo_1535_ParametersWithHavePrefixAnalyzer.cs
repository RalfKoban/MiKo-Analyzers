using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1535_ParametersWithHavePrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1535";

        public MiKo_1535_ParametersWithHavePrefixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => base.ShallAnalyze(symbol) && symbol.Name.StartsWith(Constants.Markers.Have, StringComparison.OrdinalIgnoreCase);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var betterName = NamesFinder.FindBetterNameForHavePrefix(symbol.Name);

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}
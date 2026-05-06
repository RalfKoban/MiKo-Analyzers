using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1532_ParametersWithShouldPrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1532";

        public MiKo_1532_ParametersWithShouldPrefixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => base.ShallAnalyze(symbol) && symbol.Name.StartsWithAny(Constants.Names.IntentPrefixes, StringComparison.OrdinalIgnoreCase);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var betterName = FindBetterNameForShouldPrefix(symbol.Name);

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}
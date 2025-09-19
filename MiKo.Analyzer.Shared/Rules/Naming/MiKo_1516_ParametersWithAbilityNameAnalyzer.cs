using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1516_ParametersWithAbilityNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1516";

        public MiKo_1516_ParametersWithAbilityNameAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => symbol.Type.IsBoolean() && symbol.Name.EndsWith('y') && symbol.Name.HasUpperCaseLettersAbove(1) is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var betterName = AdjectiveFinder.GetAdjectiveForNoun(symbol.Name.AsSpan(), FirstWordAdjustment.StartLowerCase);

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}
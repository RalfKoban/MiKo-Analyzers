using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1515_PropertiesWithAbilityNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1515";

        public MiKo_1515_PropertiesWithAbilityNameAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.Type.IsBoolean() && symbol.Name.EndsWith('y') && symbol.Name.HasUpperCaseLettersAbove(1) is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation)
        {
            var betterName = AdjectiveFinder.GetAdjectiveForNoun(symbol.Name.AsSpan());

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}
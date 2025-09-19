using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1517_FieldsWithAbilityNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1517";

        public MiKo_1517_FieldsWithAbilityNameAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsBoolean() && symbol.Name.EndsWith('y') && symbol.Name.HasUpperCaseLettersAbove(1) is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var name = symbol.Name;
            var prefix = GetFieldPrefix(name);
            var betterName = prefix + AdjectiveFinder.GetAdjectiveForNoun(name.AsSpan(prefix.Length), FirstWordAdjustment.StartLowerCase);

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}
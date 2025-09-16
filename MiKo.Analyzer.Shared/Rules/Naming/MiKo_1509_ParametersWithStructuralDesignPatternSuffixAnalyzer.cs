using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1509_ParametersWithStructuralDesignPatternSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1509";

        public MiKo_1509_ParametersWithStructuralDesignPatternSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => IsNameForStructuralDesignPattern(symbol.Name);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var betterName = FindBetterNameForStructuralDesignPattern(symbol.Name);

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}
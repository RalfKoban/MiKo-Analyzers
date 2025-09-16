using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1514_FieldsWithStructuralDesignPatternSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1514";

        public MiKo_1514_FieldsWithStructuralDesignPatternSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => IsNameForStructuralDesignPattern(symbol.Name);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var name = symbol.Name;
            var prefix = GetFieldPrefix(name);
            var betterName = FindBetterNameForStructuralDesignPattern(name, prefix);

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}
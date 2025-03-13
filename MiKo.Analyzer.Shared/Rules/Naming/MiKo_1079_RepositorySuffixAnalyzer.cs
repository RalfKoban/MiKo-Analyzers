using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1079_RepositorySuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1079";

        private const string Repository = nameof(Repository);

        public MiKo_1079_RepositorySuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => base.ShallAnalyze(symbol) && symbol.Name.Length > Repository.Length;

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            return symbolName.EndsWith(Repository, StringComparison.Ordinal)
                   ? new[] { Issue(symbol, CreateBetterNameProposal(FindBetterName(symbolName))) }
                   : Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name) => Pluralizer.MakePluralName(name.WithoutSuffix(Repository));
    }
}
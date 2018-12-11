using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1057";

        public MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyPropertyKey();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => symbol.Name.EndsWith(Constants.DependencyPropertyKeyFieldSuffix, StringComparison.Ordinal)
                                                                                           ? Enumerable.Empty<Diagnostic>()
                                                                                           : new[] { ReportIssue(symbol, symbol.Name + Constants.DependencyPropertyKeyFieldSuffix) };
    }
}
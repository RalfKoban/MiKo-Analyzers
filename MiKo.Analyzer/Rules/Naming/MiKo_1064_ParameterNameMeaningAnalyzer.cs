using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1064_ParameterNameMeaningAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1064";

        public MiKo_1064_ParameterNameMeaningAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol) => string.Equals(symbol.Name, symbol.Type.Name, StringComparison.OrdinalIgnoreCase)
                                                                                               ? new[] { Issue(symbol) }
                                                                                               : Enumerable.Empty<Diagnostic>();
    }
}
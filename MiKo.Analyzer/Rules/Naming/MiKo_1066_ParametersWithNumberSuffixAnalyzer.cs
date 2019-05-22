using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1066_ParametersWithNumberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1066";

        public MiKo_1066_ParametersWithNumberSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol) => symbol.Name.Last().IsNumber()
                                                                                               ? new[] { Issue(symbol) }
                                                                                               : Enumerable.Empty<Diagnostic>();
    }
}
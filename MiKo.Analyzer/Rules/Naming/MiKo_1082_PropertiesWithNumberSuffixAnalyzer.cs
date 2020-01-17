using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1082_PropertiesWithNumberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1082";

        public MiKo_1082_PropertiesWithNumberSuffixAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => base.ShallAnalyze(symbol) && symbol.GetReturnType()?.Name.EndsWithNumber() is true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => symbol.Name.EndsWithNumber() && symbol.Name.EndsWithAny(Constants.Markers.OSBitNumbers) is false
                                                                                              ? new[] { Issue(symbol) }
                                                                                              : Enumerable.Empty<Diagnostic>();
    }
}
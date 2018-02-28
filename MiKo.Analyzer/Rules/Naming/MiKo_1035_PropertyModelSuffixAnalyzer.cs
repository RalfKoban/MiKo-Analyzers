using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1035_PropertyModelSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1035";

        public MiKo_1035_PropertyModelSuffixAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => symbol.ExplicitInterfaceImplementations.Length != 0 ? Enumerable.Empty<Diagnostic>() : AnalyzeEntityMarkers(symbol);
    }
}
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1020_TypeNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1020";

        public MiKo_1020_TypeNameLengthAnalyzer() : base(Id, SymbolKind.NamedType, 40)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.IsTestClass() ? Enumerable.Empty<Diagnostic>() : Analyze(symbol);
    }
}
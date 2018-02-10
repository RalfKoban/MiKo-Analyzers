using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1024_PropertyNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1024";

        public MiKo_1024_PropertyNameLengthAnalyzer() : base(Id, SymbolKind.Property, 25)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => Analyze(symbol);
    }
}
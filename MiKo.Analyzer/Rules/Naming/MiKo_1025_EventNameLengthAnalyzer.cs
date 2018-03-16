using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1025_EventNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1025";

        public MiKo_1025_EventNameLengthAnalyzer() : base(Id, SymbolKind.Event, Constants.MaxNamingLengths.Events)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => Analyze(symbol);
    }
}
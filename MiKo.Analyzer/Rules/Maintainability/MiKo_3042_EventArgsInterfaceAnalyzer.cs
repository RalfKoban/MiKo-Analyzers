using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3042_EventArgsInterfaceAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3042";

        public MiKo_3042_EventArgsInterfaceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEventArgs();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol) => symbol.Interfaces.Select(_ => Issue(symbol.Name, _));
    }
}
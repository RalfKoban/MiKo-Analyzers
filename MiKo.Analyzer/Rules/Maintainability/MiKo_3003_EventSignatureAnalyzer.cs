using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3003_EventSignatureAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3003";

        public MiKo_3003_EventSignatureAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => symbol.Type.Name == nameof(EventHandler)
                                                                                            ? Enumerable.Empty<Diagnostic>()
                                                                                            : new []{ ReportIssue(symbol) };
    }
}
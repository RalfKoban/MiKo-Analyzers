using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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

        protected override bool ShallAnalyze(IEventSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> Analyze(IEventSymbol symbol, Compilation compilation)
        {
            switch (symbol.Type.Name)
            {
                case nameof(EventHandler):
                case nameof(NotifyCollectionChangedEventHandler):
                case nameof(PropertyChangedEventHandler):
                case nameof(PropertyChangingEventHandler):
                case nameof(CancelEventHandler):
                    return Enumerable.Empty<Diagnostic>();

                default:
                    return new[] { Issue(symbol) };
            }
        }
    }
}
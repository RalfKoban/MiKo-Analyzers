using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1075";

        private const string EventArg = "EventArg";
        private const string EventArgs = nameof(System.EventArgs);
        private const string EventsArg = nameof(EventsArg);
        private const string EventsArgs = nameof(EventsArgs);

        private static readonly ISet<string> EventArgSuffixes = new HashSet<string>
                                                                    {
                                                                        EventArgs,
                                                                        EventArg,
                                                                        EventsArgs,
                                                                        EventsArg,
                                                                    };

        public MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Class && symbol.Name.EndsWithAny(EventArgSuffixes, StringComparison.Ordinal);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.IsEventArgs() is false)
            {
                var betterName = FindBetterName(symbol);

                yield return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
            }
        }

        private static string FindBetterName(INamedTypeSymbol symbol)
        {
            var betterName = symbol.Name
                                   .Without(EventArgs)
                                   .Without(EventArg)
                                   .Without(EventsArgs)
                                   .Without(EventsArg);

            return symbol.IsPrismEvent()
                   ? betterName + "Event" // prism events should be suffixed with 'Event'
                   : betterName;
        }
    }
}
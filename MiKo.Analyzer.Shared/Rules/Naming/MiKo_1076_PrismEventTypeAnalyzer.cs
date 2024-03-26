using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1076_PrismEventTypeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1076";

        private const string CorrectSuffix = "Event";

        private const string Arg = nameof(Arg);
        private const string Args = nameof(Args);
        private const string Base = nameof(Base);
        private const string BaseArg = nameof(BaseArg);
        private const string BaseArgs = nameof(BaseArgs);
        private const string EventBaseArg = nameof(EventBaseArg);
        private const string EventsBaseArg = nameof(EventsBaseArg);
        private const string EventBaseArgs = nameof(EventBaseArgs);
        private const string EventsBaseArgs = nameof(EventsBaseArgs);
        private const string EventArgs = nameof(EventArgs);
        private const string EventsArgs = nameof(EventsArgs);
        private const string EventArg = nameof(EventArg);
        private const string EventsArg = nameof(EventsArg);
        private const string EventArgBase = nameof(EventArgBase);
        private const string EventArgsBase = nameof(EventArgsBase);
        private const string EventsArgBase = nameof(EventsArgBase);
        private const string EventsArgsBase = nameof(EventsArgsBase);
        private const string Events = nameof(Events);

        public MiKo_1076_PrismEventTypeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsPrismEvent();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (IsProperlyNamed(symbol) is false)
            {
                var proposal = GetNameWithoutSuffix(symbol.Name) + CorrectSuffix;

                yield return Issue(symbol, proposal, CreateBetterNameProposal(proposal));
            }
        }

        private static bool IsProperlyNamed(ISymbol symbol) => symbol.Name.EndsWith(CorrectSuffix, StringComparison.Ordinal);

        private static string GetNameWithoutSuffix(string name)
        {
            if (name.EndsWith(Arg, StringComparison.Ordinal))
            {
                return GetNameWithoutSuffixes(name, EventArg, EventsArg, EventBaseArg, EventsBaseArg, BaseArg, Arg);
            }

            if (name.EndsWith(Args, StringComparison.Ordinal))
            {
                return GetNameWithoutSuffixes(name, EventArgs, EventsArgs, EventBaseArgs, EventsBaseArgs, BaseArgs, Args);
            }

            if (name.EndsWith(Base, StringComparison.Ordinal))
            {
                return GetNameWithoutSuffixes(name, EventArgBase, EventsArgBase, EventArgsBase, EventsArgsBase, Base);
            }

            if (name.EndsWith(Events, StringComparison.Ordinal))
            {
                return name.WithoutSuffix(Events);
            }

            return name;
        }

        private static string GetNameWithoutSuffixes(string name, params string[] phrases)
        {
            foreach (var phrase in phrases)
            {
                if (name.EndsWith(phrase, StringComparison.Ordinal))
                {
                    return name.WithoutSuffix(phrase);
                }
            }

            return name;
        }
    }
}
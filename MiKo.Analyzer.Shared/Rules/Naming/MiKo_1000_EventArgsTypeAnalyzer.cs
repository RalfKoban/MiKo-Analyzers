using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1000_EventArgsTypeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1000";

        private const string CorrectSuffix = nameof(EventArgs);

        private const string Arg = nameof(Arg);
        private const string Args = nameof(Args);
        private const string Base = nameof(Base);
        private const string BaseArg = nameof(BaseArg);
        private const string BaseArgs = nameof(BaseArgs);
        private const string EventBaseArg = nameof(EventBaseArg);
        private const string EventsBaseArg = nameof(EventsBaseArg);
        private const string EventBaseArgs = nameof(EventBaseArgs);
        private const string EventsBaseArgs = nameof(EventsBaseArgs);
        private const string EventsArgs = nameof(EventsArgs);
        private const string EventArg = nameof(EventArg);
        private const string EventsArg = nameof(EventsArg);
        private const string EventArgBase = nameof(EventArgBase);
        private const string EventArgsBase = nameof(EventArgsBase);
        private const string EventsArgBase = nameof(EventsArgBase);
        private const string EventsArgsBase = nameof(EventsArgsBase);

        public MiKo_1000_EventArgsTypeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsEventArgs();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (IsProperlyNamed(symbol))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var proposal = GetNameWithoutSuffix(symbol.Name.AsSpan()).ConcatenatedWith(CorrectSuffix);

            return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
        }

        private static bool IsProperlyNamed(ISymbol symbol) => symbol.Name.EndsWith(CorrectSuffix, StringComparison.Ordinal);

        private static ReadOnlySpan<char> GetNameWithoutSuffix(ReadOnlySpan<char> name)
        {
            if (name.EndsWith(Arg, StringComparison.Ordinal))
            {
                return GetNameWithoutSuffixes(name, EventArg, EventsArg, EventBaseArg, EventsBaseArg, BaseArg, Arg);
            }

            if (name.EndsWith(Args, StringComparison.Ordinal))
            {
                return GetNameWithoutSuffixes(name, EventsArgs, EventBaseArgs, EventsBaseArgs, BaseArgs, Args);
            }

            if (name.EndsWith(Base, StringComparison.Ordinal))
            {
                return GetNameWithoutSuffixes(name, EventArgBase, EventsArgBase, EventArgsBase, EventsArgsBase, Base);
            }

            return name;
        }

        private static ReadOnlySpan<char> GetNameWithoutSuffixes(ReadOnlySpan<char> name, params string[] phrases)
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
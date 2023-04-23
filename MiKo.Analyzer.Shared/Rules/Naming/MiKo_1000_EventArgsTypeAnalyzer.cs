using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1000_EventArgsTypeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1000";

        private const string Proposal = "BetterName";

        private const string CorrectSuffix = nameof(EventArgs);

        private const string Arg = "Arg";
        private const string Args = "Args";
        private const string EventsArgs = "EventsArgs";
        private const string EventArg = "EventArg";
        private const string EventsArg = "EventsArg";

        public MiKo_1000_EventArgsTypeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static string FindBetterName(ISymbol symbol, Diagnostic diagnostic) => diagnostic.Properties[Proposal];

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsEventArgs();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (IsProperlyNamed(symbol) is false)
            {
                var proposal = GetNameWithoutSuffix(symbol.Name) + CorrectSuffix;

                yield return Issue(symbol, proposal, new Dictionary<string, string> { { Proposal, proposal }});
            }
        }

        private static bool IsProperlyNamed(ISymbol symbol) => symbol.Name.EndsWith(CorrectSuffix, StringComparison.Ordinal);

        private static string GetNameWithoutSuffix(string name)
        {
            if (name.EndsWith(Args, StringComparison.Ordinal))
            {
                if (name.EndsWith(EventsArgs, StringComparison.Ordinal))
                {
                    return name.WithoutSuffix(EventsArgs);
                }

                return name.WithoutSuffix(Args);
            }

            if (name.EndsWith(Arg, StringComparison.Ordinal))
            {
                if (name.EndsWith(EventArg, StringComparison.Ordinal))
                {
                    return name.WithoutSuffix(EventArg);
                }

                if (name.EndsWith(EventsArg, StringComparison.Ordinal))
                {
                    return name.WithoutSuffix(EventsArg);
                }
            }

            return name;
        }
    }
}
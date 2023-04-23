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

        private const string Suffix = nameof(EventArgs);
        private const string BadSuffix1 = "Args";
        private const string BadSuffix2 = "EventArg";

        public MiKo_1000_EventArgsTypeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static string FindBetterName(ISymbol symbol)
        {
            var name = symbol.Name;

            if (name.EndsWith(BadSuffix1, StringComparison.Ordinal))
            {
                return name.WithoutSuffix(BadSuffix1) + Suffix;
            }

            if (name.EndsWith(BadSuffix2, StringComparison.Ordinal))
            {
                return name.WithoutSuffix(BadSuffix2) + Suffix;
            }

            return name + Suffix;
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsEventArgs();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (IsProperlyNamed(symbol) is false)
            {
                yield return Issue(symbol, FindBetterName(symbol));
            }
        }

        private static bool IsProperlyNamed(ISymbol symbol) => symbol.Name.EndsWith(Suffix, StringComparison.Ordinal);
    }
}
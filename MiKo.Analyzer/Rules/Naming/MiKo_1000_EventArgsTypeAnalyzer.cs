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

        private const string Suffix = nameof(EventArgs);

        public MiKo_1000_EventArgsTypeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static string FindBetterName(ISymbol symbol) => symbol.Name + Suffix;

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsEventArgs();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (IsProperlyNamed(symbol))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return new[] { Issue(symbol, FindBetterName(symbol)) };
        }

        private static bool IsProperlyNamed(ISymbol symbol) => symbol.Name.EndsWith(Suffix, StringComparison.Ordinal);
    }
}
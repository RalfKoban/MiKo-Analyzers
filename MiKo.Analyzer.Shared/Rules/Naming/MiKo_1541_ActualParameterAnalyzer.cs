using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1541_ActualParameterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1541";

        private const string Prefix = "actual";

        public MiKo_1541_ActualParameterAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.StartsWith(Prefix, StringComparison.Ordinal))
            {
                var betterName = FindBetterName(symbolName);

                if (symbolName != betterName)
                {
                    return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name) => name.Length > Prefix.Length
                                                             ? name.AsSpan(Prefix.Length).ToLowerCaseAt(0)
                                                             : name;
    }
}
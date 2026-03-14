using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1053_DelegateFieldNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1053";

        private static readonly string[] WrongNames = { "Action", "Delegate", "Func" };

        private static readonly string[] WrongSuffixes =
                                                         {
                                                             "action",
                                                             "delegate",
                                                             "func",
                                                             "Action",
                                                             "Delegate",
                                                             "Func",
                                                         };

        public MiKo_1053_DelegateFieldNameSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            if (symbol.Type.IsDelegate())
            {
                var symbolName = symbol.Name.AsSpan();

                if (symbolName.EndsWithAny(WrongNames, StringComparison.OrdinalIgnoreCase))
                {
                    var betterName = FindBetterName(symbolName);

                    return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(ReadOnlySpan<char> symbolName)
        {
            var nameWithoutSuffix = symbolName.WithoutSuffixes(WrongSuffixes);

            var suffix = symbolName.Length > nameWithoutSuffix.Length && nameWithoutSuffix.Length > 0 && symbolName[nameWithoutSuffix.Length].IsUpperCase()
                         ? Constants.Names.Callback
                         : Constants.Names.callback;

            return nameWithoutSuffix.ConcatenatedWith(suffix);
        }
    }
}
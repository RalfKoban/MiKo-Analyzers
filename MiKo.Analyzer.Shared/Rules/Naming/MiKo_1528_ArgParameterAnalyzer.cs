using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1528_ArgParameterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1528";

        private static readonly string[] Starts = { "arguments", "argument", "args", "arg" };

        public MiKo_1528_ArgParameterAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol)
        {
            switch (symbol.GetEnclosingMethod()?.Name)
            {
                case "Format":
                case "FormatWith":
                case "Main":
                    return false;

                default:
                    return base.ShallAnalyze(symbol);
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.StartsWith("arg", StringComparison.Ordinal))
            {
                var betterName = FindBetterName(symbolName);

                yield return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
            }
        }

        private static string FindBetterName(string symbolName)
        {
            int startIndex = FindStartIndex(symbolName);

            if (startIndex > 0 && symbolName.Length > startIndex)
            {
                return symbolName.AsSpan(startIndex).ToLowerCaseAt(0);
            }

            return symbolName;
        }

        private static int FindStartIndex(string symbolName)
        {
            foreach (var start in Starts)
            {
                if (symbolName.StartsWith(start, StringComparison.Ordinal))
                {
                    return start.Length;
                }
            }

            return 0;
        }
    }
}
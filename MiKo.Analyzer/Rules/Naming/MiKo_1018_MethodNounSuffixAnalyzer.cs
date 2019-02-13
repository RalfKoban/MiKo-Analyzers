using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1018_MethodNounSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1018";

        private static readonly KeyValuePair<string, string>[] Endings =
            {
                new KeyValuePair<string, string>("Caption", "Caption"),
                new KeyValuePair<string, string>(nameof(Exception), nameof(Exception)),
                new KeyValuePair<string, string>("estination", "estination"),
                new KeyValuePair<string, string>("mentation", "ment"),
                new KeyValuePair<string, string>("unction", "unction"),
                new KeyValuePair<string, string>("ptation", "pt"),
                new KeyValuePair<string, string>("rmation", "rm"),
                new KeyValuePair<string, string>("llation", "ll"),
                new KeyValuePair<string, string>("stration", "ster"),
                new KeyValuePair<string, string>("ration", "re"),
                new KeyValuePair<string, string>("isation", "ise"),
                new KeyValuePair<string, string>("ization", "ize"),
                new KeyValuePair<string, string>("ation", "ate"),
                new KeyValuePair<string, string>("ction", "ct"),
                new KeyValuePair<string, string>("ption", "pt"),
                new KeyValuePair<string, string>("rison", "re"),
            };

        private static readonly string[] StartingPhrases =
            {
                "Undo",
                "Redo",
                "To",
                "Verify",
                "Ensure",
                "Get",
                "get_",
                "Set",
                "set_",
                "Refresh",
                "Reset",
                "Trace",
                "Read",
                "Write",
                "Load",
                "Save",
                "Store",
                "Restore",
                "Update",
                "Add",
                "Remove",
                "Clear",
                "Create",
                "Delete",
                "Query",
                "Analyze",
                "Start",
                "Stop",
                "Restart",
                "Try",
                "Translate",
                "Find",
                "Push",
                "Pop",
            };

        public MiKo_1018_MethodNounSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && !symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method) => TryFindBetterName(method.Name, out var betterName)
                                                                                            ? new[] { ReportIssue(method, betterName) }
                                                                                            : Enumerable.Empty<Diagnostic>();

        public static bool TryFindBetterName(string name, out string betterName)
        {
            betterName = name;

            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.StartsWithAny(StartingPhrases))
                return false;

            foreach (var pair in Endings.Where(_ => name.EndsWith(_.Key, StringComparison.Ordinal)))
            {
                betterName = name.Substring(0, name.Length - pair.Key.Length) + pair.Value;
                return !string.Equals(betterName, name, StringComparison.Ordinal);
            }

            return false;
        }
    }
}
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
                new KeyValuePair<string, string>(nameof(Action), nameof(Action)),
                new KeyValuePair<string, string>("Caption", "Caption"),
                new KeyValuePair<string, string>(nameof(Exception), nameof(Exception)),
                new KeyValuePair<string, string>("Func", "Function"),
                new KeyValuePair<string, string>("Function", "Function"),
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

        private static readonly string[] StartingPhrases = new[]
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
                                                                   "Subscribe",
                                                                   "Unsubscribe",
                                                                   "Register",
                                                                   "Unregister",
                                                                   "Validate",
                                                                   "CompileTimeValidate",
                                                                   "Is",
                                                                   "Can",
                                                                   "Has",
                                                                   "Cancel",
                                                                   "Prepare",
                                                                   "Parse",
                                                                   "Wrap",
                                                                   "Select",
                                                                   "Deselect",
                                                                   "Rollback",
                                                                   "Invert",
                                                                   "Handle",
                                                                   "Free",
                                                                   "Calculate",
                                                                   "Open",
                                                                   "Close",
                                                                   "Clone",
                                                               }.OrderBy(_ => _.Length)
                                                                .ThenBy(_ => _)
                                                                .ToArray();

        public MiKo_1018_MethodNounSuffixAnalyzer() : base(Id)
        {
        }

        public static bool TryFindBetterName(string name, out string result)
        {
            result = name;

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (HasAcceptableStartingPhrase(name))
            {
                return false;
            }

            foreach (var pair in Endings.Where(_ => name.EndsWith(_.Key, StringComparison.Ordinal)))
            {
                result = name.Substring(0, name.Length - pair.Key.Length) + pair.Value;

                return string.Equals(result, name, StringComparison.Ordinal) is false;
            }

            return false;
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method) => TryFindBetterName(method.Name, out var betterName)
                                                                                            ? new[] { Issue(method, betterName) }
                                                                                            : Enumerable.Empty<Diagnostic>();

        private static bool HasAcceptableStartingPhrase(string name)
        {
            foreach (var phrase in StartingPhrases.Where(_ => name.StartsWith(_, StringComparison.Ordinal)))
            {
                var remainingName = name.Substring(phrase.Length);

                if (remainingName.Length == 0 || remainingName[0].IsUpperCase())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
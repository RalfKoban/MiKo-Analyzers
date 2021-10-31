using System;
using System.Collections.Generic;
using System.Linq;

namespace MiKoSolutions.Analyzers.Linguistics
{
    public static class Verbalizer
    {
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
                new KeyValuePair<string, string>("iption", "ibe"),
                new KeyValuePair<string, string>("rmation", "rm"),
                new KeyValuePair<string, string>("llation", "ll"),
                new KeyValuePair<string, string>("stration", "ster"),
                new KeyValuePair<string, string>("ration", "re"),
                new KeyValuePair<string, string>("isition", "ire"),
                new KeyValuePair<string, string>("isation", "ise"),
                new KeyValuePair<string, string>("ization", "ize"),
                new KeyValuePair<string, string>("ation", "ate"),
                new KeyValuePair<string, string>("ction", "ct"),
                new KeyValuePair<string, string>("ption", "pt"),
                new KeyValuePair<string, string>("rison", "re"),
                new KeyValuePair<string, string>("sis", "ze"),
            };

        private static readonly string[] StartingPhrases = new[]
                                                               {
                                                                   "Add",
                                                                   "Analyze",
                                                                   "Calculate",
                                                                   "Can",
                                                                   "Cancel",
                                                                   "Clear",
                                                                   "Clone",
                                                                   "Close",
                                                                   "CompileTimeValidate",
                                                                   "Continue",
                                                                   "Create",
                                                                   "Delay",
                                                                   "Delete",
                                                                   "Deregister",
                                                                   "Deselect",
                                                                   "Ensure",
                                                                   "Find",
                                                                   "Free",
                                                                   "Get",
                                                                   "get_",
                                                                   "Handle",
                                                                   "Has",
                                                                   "Invert",
                                                                   "Is",
                                                                   "Load",
                                                                   "Log",
                                                                   "Open",
                                                                   "Parse",
                                                                   "Pause",
                                                                   "Pop",
                                                                   "Prepare",
                                                                   "Push",
                                                                   "Query",
                                                                   "Read",
                                                                   "Redo",
                                                                   "Refresh",
                                                                   "Register",
                                                                   "Remove",
                                                                   "Request",
                                                                   "Reset",
                                                                   "Restart",
                                                                   "Restore",
                                                                   "Resume",
                                                                   "Rollback",
                                                                   "Save",
                                                                   "Select",
                                                                   "Set",
                                                                   "set_",
                                                                   "Start",
                                                                   "Stop",
                                                                   "Store",
                                                                   "Subscribe",
                                                                   "Suspend",
                                                                   "To",
                                                                   "Trace",
                                                                   "Translate",
                                                                   "Try",
                                                                   "Undo",
                                                                   "Unregister",
                                                                   "Unsubscribe",
                                                                   "Update",
                                                                   "Validate",
                                                                   "Verify",
                                                                   "With",
                                                                   "Wrap",
                                                                   "Write",
                                                               }.OrderBy(_ => _.Length)
                                                                .ThenBy(_ => _)
                                                                .ToArray();

        public static string MakeInfiniteVerb(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            if (name.EndsWith("s", StringComparison.Ordinal))
            {
                if (name.EndsWith("oes", StringComparison.Ordinal) || name.EndsWith("shes", StringComparison.Ordinal))
                {
                    return name.WithoutSuffix("es");
                }

                return name.WithoutSuffix("s");
            }

            return name;
        }

        public static bool TryMakeVerb(string name, out string result)
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
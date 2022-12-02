using System;
using System.Collections.Generic;
using System.Linq;

namespace MiKoSolutions.Analyzers.Linguistics
{
    public static class Verbalizer
    {
        private static readonly HashSet<char> CharsForTwoCharacterEndingsWithS = new HashSet<char> { 'a', 'h', 'i', 'o', 's', 'u', 'x', 'z' };

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

        private static readonly string[] MiddlePhrases = new[]
                                                               {
                                                                   "InformAbout",
                                                                   "InformsAbout",
                                                                   "InformedAbout",
                                                                   "BelongsTo",
                                                               }.OrderBy(_ => _.Length)
                                                                .ThenBy(_ => _)
                                                                .ToArray();

        public static string MakeInfiniteVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            if (value.EndsWith('s'))
            {
                if (value.EndsWith("oes", StringComparison.Ordinal) || value.EndsWith("shes", StringComparison.Ordinal))
                {
                    return value.WithoutSuffix("es");
                }

                return value.WithoutSuffix("s");
            }

            return value;
        }

        public static string MakeThirdPersonSingularVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            if (value.EndsWith('y'))
            {
                if (value.EndsWith("ay", StringComparison.Ordinal) || value.EndsWith("ey", StringComparison.Ordinal))
                {
                    return value + 's';
                }

                return value.Substring(0, value.Length - 1) + "ies";
            }

            if (value.EndsWith('s'))
            {
                if (value.EndsWith("ss", StringComparison.Ordinal))
                {
                    return value + "es";
                }

                if (value.EndsWith("oes", StringComparison.Ordinal) || value.EndsWith("shes", StringComparison.Ordinal))
                {
                    return value;
                }

                if (IsThirdPersonSingularVerb(value))
                {
                    return value;
                }
            }

            var result = value + 's';
            if (IsTwoCharacterEndingsWithS(result))
            {
                return value + "es";
            }

            return result;
        }

        public static bool IsThirdPersonSingularVerb(string value)
        {
            var length = value?.Length;
            if (length >= 2)
            {
                return value[length.Value - 1] == 's' && CharsForTwoCharacterEndingsWithS.Contains(value[length.Value - 2]) is false;
            }

            return false;
        }

        public static bool IsTwoCharacterEndingsWithS(string value)
        {
            var length = value?.Length;
            if (length >= 2)
            {
                return value[length.Value - 1] == 's' && CharsForTwoCharacterEndingsWithS.Contains(value[length.Value - 2]);
            }

            return false;
        }

        public static string MakeGerundVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            if (value.EndsWith("ing", StringComparison.Ordinal))
            {
                return value;
            }

            return (value + "ing").Replace("ping", "pping").Replace("eing", "ing");
        }

        public static bool TryMakeVerb(string value, out string result)
        {
            result = value;

            if (value.IsNullOrWhiteSpace())
            {
                return false;
            }

            if (HasAcceptableStartingPhrase(value))
            {
                return false;
            }

            if (HasAcceptableMiddlePhrase(value))
            {
                return false;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var pair in Endings)
            {
                if (value.EndsWith(pair.Key, StringComparison.Ordinal))
                {
                    result = value.Substring(0, value.Length - pair.Key.Length) + pair.Value;

                    return string.Equals(result, value, StringComparison.Ordinal) is false;
                }
            }

            return false;
        }

        private static bool HasAcceptableStartingPhrase(string value)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var phrase in StartingPhrases)
            {
                if (value.StartsWith(phrase, StringComparison.Ordinal))
                {
                    var remaining = value.AsSpan(phrase.Length);

                    if (remaining.Length == 0 || remaining[0].IsUpperCase())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool HasAcceptableMiddlePhrase(string value)
        {
            return value.ContainsAny(MiddlePhrases);
        }
    }
}
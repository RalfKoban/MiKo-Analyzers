using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiKoSolutions.Analyzers.Linguistics
{
    public static class Verbalizer
    {
        private static readonly HashSet<char> CharsForTwoCharacterEndingsWithS = new HashSet<char> { 'a', 'h', 'i', 'o', 's', 'u', 'x', 'z' };

        private static readonly ConcurrentDictionary<string, string> GerundVerbs = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> InfiniteVerbs = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> ThirdPersonSingularVerbs = new ConcurrentDictionary<string, string>();

        private static readonly KeyValuePair<string, string>[] Endings =
                                                                         {
                                                                             new KeyValuePair<string, string>(nameof(Action), nameof(Action)),
                                                                             new KeyValuePair<string, string>("Caption", "Caption"),
                                                                             new KeyValuePair<string, string>("cution", "cute"),
                                                                             new KeyValuePair<string, string>(nameof(Exception), nameof(Exception)),
                                                                             new KeyValuePair<string, string>("Func", "Function"),
                                                                             new KeyValuePair<string, string>("Function", "Function"),
                                                                             new KeyValuePair<string, string>("estination", "estination"),
                                                                             new KeyValuePair<string, string>("mentation", "ment"),
                                                                             new KeyValuePair<string, string>("unction", "unction"),
                                                                             new KeyValuePair<string, string>("ptation", "pt"),
                                                                             new KeyValuePair<string, string>("iption", "ibe"),
                                                                             new KeyValuePair<string, string>("rmation", "rm"),
                                                                             new KeyValuePair<string, string>("allation", "all"),
                                                                             new KeyValuePair<string, string>("ellation", "el"),
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
                                                               "PromptFor",
                                                               "Push",
                                                               "Query",
                                                               "Read",
                                                               "Record",
                                                               "Recover",
                                                               "Redo",
                                                               "Refresh",
                                                               "Register",
                                                               "Reload",
                                                               "Release",
                                                               "Remove",
                                                               "Replace",
                                                               "Report",
                                                               "Request",
                                                               "Reset",
                                                               "Resolve",
                                                               "Restart",
                                                               "Restore",
                                                               "Resume",
                                                               "Retrieve",
                                                               "Rollback",
                                                               "Save",
                                                               "Select",
                                                               "Send",
                                                               "Set",
                                                               "set_",
                                                               "Setup",
                                                               "Simulate",
                                                               "Sort",
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
                                                               "Unlock",
                                                               "Unregister",
                                                               "Unsubscribe",
                                                               "Update",
                                                               "Validate",
                                                               "Verify",
                                                               "With",
                                                               "Wrap",
                                                               "Write",
                                                               "Zoom",
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

        public static bool IsThirdPersonSingularVerb(string value)
        {
            var length = value?.Length;

            if (length == 4 && value.Equals("will", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

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

            return GerundVerbs.GetOrAdd(value, CreateGerundVerb);

            string CreateGerundVerb(string word)
            {
                if (word.EndsWith("ing", StringComparison.Ordinal))
                {
                    return word;
                }

                var sb = new StringBuilder(word + "ing").ReplaceWithCheck("ping", "pping").ReplaceWithCheck("eing", "ing");

                return sb.ToString();
            }
        }

        public static string MakeInfiniteVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            return InfiniteVerbs.GetOrAdd(value, CreateInfiniteVerb);

            string CreateInfiniteVerb(string word)
            {
                if (word.EndsWith('s'))
                {
                    if (word.EndsWith("oes", StringComparison.Ordinal) || word.EndsWith("shes", StringComparison.Ordinal))
                    {
                        return word.WithoutSuffix("es");
                    }

                    return word.WithoutSuffix("s");
                }

                return word;
            }
        }

        public static string MakeThirdPersonSingularVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            if (value.EndsWithAny(Constants.SentenceMarkers))
            {
                return ThirdPersonSingularVerbs.GetOrAdd(value, CreateThirdPersonSingularVerbForSentence);
            }

            return ThirdPersonSingularVerbs.GetOrAdd(value, CreateThirdPersonSingularVerb);

            string CreateThirdPersonSingularVerbForSentence(string sentenceEnding)
            {
                var word = sentenceEnding.Substring(0, value.Length - 1);

                return CreateThirdPersonSingularVerb(word) + value.Last();
            }

            string CreateThirdPersonSingularVerb(string word)
            {
                if (word.EndsWith('y'))
                {
                    if (word.EndsWith("ay", StringComparison.Ordinal) || word.EndsWith("ey", StringComparison.Ordinal))
                    {
                        return word + 's';
                    }

                    return word.Substring(0, word.Length - 1) + "ies";
                }

                if (word.EndsWith('s'))
                {
                    if (word.EndsWith("ss", StringComparison.Ordinal))
                    {
                        return word + "es";
                    }

                    if (word.EndsWith("oes", StringComparison.Ordinal) || word.EndsWith("shes", StringComparison.Ordinal))
                    {
                        return word;
                    }

                    if (IsThirdPersonSingularVerb(word))
                    {
                        return word;
                    }
                }

                if (word.Equals("will", StringComparison.OrdinalIgnoreCase))
                {
                    return word;
                }

                var result = word + 's';

                if (IsTwoCharacterEndingsWithS(result))
                {
                    return word + "es";
                }

                return result;
            }
        }

        public static bool TryMakeVerb(string value, out string result)
        {
            result = value;

            if (value.IsNullOrWhiteSpace())
            {
                return false;
            }

            var word = value.AsSpan();

            if (HasAcceptableStartingPhrase(word))
            {
                return false;
            }

            if (HasAcceptableMiddlePhrase(word))
            {
                return false;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var pair in Endings)
            {
                if (word.EndsWith(pair.Key, StringComparison.Ordinal))
                {
                    result = word.Slice(0, word.Length - pair.Key.Length).ToString() + pair.Value;

                    return result.Equals(value, StringComparison.Ordinal) is false;
                }
            }

            return false;
        }

        private static bool HasAcceptableStartingPhrase(ReadOnlySpan<char> value)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var phrase in StartingPhrases)
            {
                if (value.StartsWith(phrase, StringComparison.Ordinal))
                {
                    var remaining = value.Slice(phrase.Length);

                    if (remaining.Length == 0 || remaining[0].IsUpperCase())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool HasAcceptableMiddlePhrase(ReadOnlySpan<char> value)
        {
            return value.ContainsAny(MiddlePhrases);
        }
    }
}
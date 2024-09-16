using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class Verbalizer
    {
        private static readonly HashSet<char> CharsForTwoCharacterEndingsWithS = new HashSet<char> { 'a', 'h', 'i', 'o', 's', 'u', 'x', 'z' };

        private static readonly string[] PluralEndings = { "gers", "tchers", "pters", "stors", "ptors" };

        private static readonly string[] PastEndings = { "ated", "dled", "ced", "ged", "ied", "red", "sed", "ved" };

        private static readonly string[] FourCharacterGerundEndings = { "pping", "rring", "tting" };

        private static readonly string[] ThreeCharacterGerundEndings = { "anging", "inging", "ssing", "cting", "pting" };

        private static readonly string[] ThreeCharacterGerundEndingsWithE = { "bling", "kling", "ging", "sing", "ting", "uing", "ving", "zing" };

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
                                                                             new KeyValuePair<string, string>("vocation", "voke"),
                                                                             new KeyValuePair<string, string>("ation", "ate"),
                                                                             new KeyValuePair<string, string>("ction", "ct"),
                                                                             new KeyValuePair<string, string>("ption", "pt"),
                                                                             new KeyValuePair<string, string>("rison", "re"),
                                                                             new KeyValuePair<string, string>("sis", "ze"),
                                                                         };

        private static readonly string[] IsAre = { "is", "are" };

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
                                                                   "Collect",
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
                                                                   "JumpTo",
                                                                   "Load",
                                                                   "Log",
                                                                   "NavigateTo",
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
                                                               }.ToArray(_ => _, AscendingStringComparer.Default);

        private static readonly string[] MiddlePhrases = new[]
                                                             {
                                                                 "InformAbout",
                                                                 "InformsAbout",
                                                                 "InformedAbout",
                                                                 "BelongsTo",
                                                             }.ToArray(_ => _, AscendingStringComparer.Default);

        private static readonly string[] EndingPhrases = new[]
                                                             {
                                                                 "Position",
                                                                 "Parenthesis",
                                                                 "Situation",
                                                             }.ToArray(_ => _, AscendingStringComparer.Default);

        private static readonly char[] SentenceEndingMarkers = ".?!;:,)".ToCharArray();

        private static readonly string[] AdjectivesOrAdverbs =
                                                               {
                                                                   "about",
                                                                   "afterwards",
                                                                   "also",
                                                                   "already",
                                                                   "always",
                                                                   "at",
                                                                   "before",
                                                                   "either",
                                                                   "first",
                                                                   "however",
                                                                   "in",
                                                                   "just",
                                                                   "later",
                                                                   "longer",
                                                                   "on",
                                                                   "off",
                                                                   "out",
                                                                   "no",
                                                                   "not",
                                                                   "now",
                                                                   "than",
                                                                   "then",
                                                                   "therefore",
                                                                   "to",
                                                                   "turn",
                                                               };

        public static bool IsAdjectiveOrAdverb(ReadOnlySpan<char> value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.EndsWith("ly", comparison))
            {
                if (value.EndsWith("ply", comparison))
                {
                    return value.Equals("simply", comparison);
                }

                return true;
            }

            return value.EqualsAny(AdjectivesOrAdverbs, comparison);
        }

        public static bool IsPlural(ReadOnlySpan<char> value) => value.EndsWith('s') && IsThirdPersonSingularVerb(value) is false;

        public static bool IsThirdPersonSingularVerb(ReadOnlySpan<char> value)
        {
            var length = value.Length;

            if (length < 2)
            {
                return false;
            }

            if (length == 4 && value.Equals("will", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (value[length - 1] != 's')
            {
                return false;
            }

            var previous = value[length - 2];

            if (previous == 'r')
            {
                return value.EndsWithAny(PluralEndings, StringComparison.OrdinalIgnoreCase) is false;
            }

            return CharsForTwoCharacterEndingsWithS.Contains(previous) is false;
        }

        public static bool IsThirdPersonSingularVerb(string value) => value != null && IsThirdPersonSingularVerb(value.AsSpan());

        public static bool IsTwoCharacterEndingsWithS(string value)
        {
            var length = value?.Length;

            if (length >= 2)
            {
                return value[length.Value - 1] == 's' && CharsForTwoCharacterEndingsWithS.Contains(value[length.Value - 2]);
            }

            return false;
        }

        public static bool IsGerundVerb(string value) => value != null && IsGerundVerb(value.AsSpan());

        public static bool IsGerundVerb(ReadOnlySpan<char> value)
        {
            if (value.Length <= 4)
            {
                return false;
            }

            if (value.EndsWith("ing", StringComparison.Ordinal))
            {
                if (value.EndsWith("thing", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return true;
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
                if (word.EqualsAny(IsAre, StringComparison.OrdinalIgnoreCase))
                {
                    return word;
                }

                if (word.Equals("has", StringComparison.OrdinalIgnoreCase))
                {
                    return word[0].IsUpperCaseLetter() ? "Having" : "having";
                }

                if (word.Equals("shutdown", StringComparison.OrdinalIgnoreCase))
                {
                    return word[0].IsUpperCaseLetter() ? "Shutting down" : "shutting down";
                }

                if (word.EndsWith("ing", StringComparison.Ordinal))
                {
                    if (word.Length > 4)
                    {
                        return word;
                    }

                    // here we have only 4 character words
                    if (word.Equals("bing", StringComparison.OrdinalIgnoreCase)
                     || word.Equals("king", StringComparison.OrdinalIgnoreCase)
                     || word.Equals("ming", StringComparison.OrdinalIgnoreCase))
                    {
                        return word;
                    }

                    return word + "ing";
                }

                var gerundVerb = new StringBuilder(word).Append("ing")
                                                        .ReplaceWithCheck("ping", "pping")
                                                        .ReplaceWithCheck("eing", "ing")
                                                        .ReplaceWithCheck("uring", "urring")
                                                        .ReplaceWithCheck("uting", "utting")
                                                        .ToString();

                return gerundVerb;
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
                    if (word.Equals("is", StringComparison.OrdinalIgnoreCase))
                    {
                        return word[0].IsUpperCaseLetter() ? "Be" : "be";
                    }

                    if (word.Equals("has", StringComparison.OrdinalIgnoreCase))
                    {
                        return word[0].IsUpperCaseLetter() ? "Have" : "have";
                    }

                    if (word.EndsWith("ss", StringComparison.Ordinal))
                    {
                        return word;
                    }

                    if (word.EndsWith("oes", StringComparison.Ordinal) || word.EndsWith("shes", StringComparison.Ordinal))
                    {
                        return word.WithoutSuffix("es");
                    }

                    return word.WithoutSuffix("s");
                }

                if (word.Equals("are", StringComparison.OrdinalIgnoreCase))
                {
                    return word[0].IsUpperCaseLetter() ? "Be" : "be";
                }

                if (word.EndsWith("ing", StringComparison.OrdinalIgnoreCase))
                {
                    var wordLength = word.Length;

                    if (wordLength == 4)
                    {
                        // ignore short word such as "ping" or "thing"
                        return word;
                    }

                    if (word.EndsWith("thing", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return word;
                    }

                    if (word.EndsWithAny(FourCharacterGerundEndings, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.Substring(0, wordLength - 4);
                    }

                    if (word.EndsWithAny(ThreeCharacterGerundEndings, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.Substring(0, wordLength - 3);
                    }

                    if (word.EndsWithAny(ThreeCharacterGerundEndingsWithE, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.AsSpan(0, wordLength - 3).ConcatenatedWith('e');
                    }

                    return word.Substring(0, wordLength - 3);
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

            if (value.EndsWithAny(SentenceEndingMarkers))
            {
                return ThirdPersonSingularVerbs.GetOrAdd(value, CreateThirdPersonSingularVerbForSentence);
            }

            return ThirdPersonSingularVerbs.GetOrAdd(value, CreateThirdPersonSingularVerb);

            string CreateThirdPersonSingularVerbForSentence(string sentenceEnding)
            {
                var word = sentenceEnding.TrimEnd(SentenceEndingMarkers);

                return CreateThirdPersonSingularVerb(word).ConcatenatedWith(sentenceEnding.AsSpan(word.Length));
            }

            string CreateThirdPersonSingularVerb(string word)
            {
                if (word.Length > 0)
                {
                    const char Apostrophe = '\'';
                    const char DoubleQuote = '\"';

                    var firstChar = word.First();
                    var lastChar = word.Last();

                    switch (firstChar)
                    {
                        case Apostrophe when lastChar == Apostrophe:
                            return CreateThirdPersonSingularVerb(word.Trim(Apostrophe)).SurroundedWithApostrophe();

                        case DoubleQuote when lastChar == DoubleQuote:
                            return CreateThirdPersonSingularVerb(word.Trim(DoubleQuote)).SurroundedWithDoubleQuote();
                    }
                }

                if (word.EndsWith('y'))
                {
                    if (word.EndsWith("ay", StringComparison.Ordinal) || word.EndsWith("ey", StringComparison.Ordinal))
                    {
                        return word.AsSpan().ConcatenatedWith('s');
                    }

                    return word.AsSpan(0, word.Length - 1).ConcatenatedWith("ies");
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

                if (word.EndsWith("ed", StringComparison.Ordinal))
                {
                    return CreateFromPast(word);
                }

                if (word.EndsWith("ing", StringComparison.Ordinal))
                {
                    return CreateFromGerund(word);
                }

                if (word.Equals("be", StringComparison.OrdinalIgnoreCase) || word.Equals("are", StringComparison.OrdinalIgnoreCase))
                {
                    return word[0].IsUpperCaseLetter() ? "Is" : "is";
                }

                if (word.Equals("have", StringComparison.OrdinalIgnoreCase))
                {
                    return word[0].IsUpperCaseLetter() ? "Has" : "has";
                }

                if (word.Equals("will", StringComparison.OrdinalIgnoreCase))
                {
                    return word;
                }

                if (word.Equals("shutdown", StringComparison.OrdinalIgnoreCase))
                {
                    return word[0].IsUpperCaseLetter() ? "Shuts down" : "shuts down";
                }

                return AppendEndingS(word);
            }

            string CreateFromPast(string word)
            {
                if (word.EndsWith("dded", StringComparison.Ordinal))
                {
                    return word.AsSpan(0, word.Length - 2).ConcatenatedWith('s');
                }

                if (word.EndsWith("tted", StringComparison.Ordinal))
                {
                    return word.AsSpan(0, word.Length - 3).ConcatenatedWith('s');
                }

                if (word.EndsWith("eed", StringComparison.Ordinal))
                {
                    return word.AsSpan().ConcatenatedWith('s');
                }

                if (word.EndsWithAny(PastEndings, StringComparison.Ordinal))
                {
                    return word.AsSpan(0, word.Length - 1).ConcatenatedWith('s');
                }

                return word.AsSpan(0, word.Length - 2).ConcatenatedWith('s');
            }

            string CreateFromGerund(string word)
            {
                var infiniteVerb = MakeInfiniteVerb(word);

                if (infiniteVerb == word)
                {
                    // did not work, so just use this to avoid infinite recursion
                    return AppendEndingS(word);
                }

                return CreateThirdPersonSingularVerb(infiniteVerb);
            }

            string AppendEndingS(string word)
            {
                var result = word.AsSpan().ConcatenatedWith('s');

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

            var span = value.AsSpan();

            if (HasAcceptableStartingPhrase(span))
            {
                return false;
            }

            if (HasAcceptableMiddlePhrase(value))
            {
                return false;
            }

            if (HasAcceptableEndingPhrase(span))
            {
                return false;
            }

            var length = Endings.Length;

            for (var index = 0; index < length; index++)
            {
                var pair = Endings[index];
                var key = pair.Key;

                if (span.EndsWith(key, StringComparison.Ordinal))
                {
                    result = span.Slice(0, span.Length - key.Length).ConcatenatedWith(pair.Value);

                    return result.Equals(value, StringComparison.Ordinal) is false;
                }
            }

            return false;
        }

        private static bool HasAcceptableStartingPhrase(ReadOnlySpan<char> value)
        {
            var length = StartingPhrases.Length;

            for (var index = 0; index < length; index++)
            {
                var phrase = StartingPhrases[index];

                if (value.StartsWith(phrase, StringComparison.Ordinal))
                {
                    var remaining = value.Slice(phrase.Length);

                    return remaining.Length == 0 || remaining[0].IsUpperCase();
                }
            }

            return false;
        }

        private static bool HasAcceptableMiddlePhrase(string value) => value.ContainsAny(MiddlePhrases);

        private static bool HasAcceptableEndingPhrase(ReadOnlySpan<char> value) => value.EndsWithAny(EndingPhrases);
    }
}
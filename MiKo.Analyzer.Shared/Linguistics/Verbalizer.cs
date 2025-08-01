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

        private static readonly string[] NonThirdPersonSingularEndingsWithS = { "pters", "tors", "gers", "chers" };

        private static readonly string[] SpecialPastEndings = { "ated", "dled", "ced", "ged", "ied", "red", "rized", "sed", "ved" };

        private static readonly string[] PastEndings = SpecialPastEndings.Concat(new[] { "led", "eed", "ted", "ded" }).ToArray();

        private static readonly string[] FourCharacterGerundEndings = { "pping", "rring", "tting" };

        private static readonly string[] ThreeCharacterGerundEndings = { "anging", "inging", "ssing", "cting", "pting", "enting" };

        private static readonly string[] ThreeCharacterGerundEndingsWithE = { "bling", "kling", "ging", "sing", "ting", "uing", "ving", "zing", "mining" };

        private static readonly ConcurrentDictionary<string, string> GerundVerbs = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> InfiniteVerbs = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> ThirdPersonSingularVerbs = new ConcurrentDictionary<string, string>(new[]
                                                                                                                                             {
                                                                                                                                                 new KeyValuePair<string, string>("Are", "Is"),
                                                                                                                                                 new KeyValuePair<string, string>("are", "is"),
                                                                                                                                                 new KeyValuePair<string, string>("got", "gets"),
                                                                                                                                                 new KeyValuePair<string, string>("Got", "Gets"),
                                                                                                                                                 new KeyValuePair<string, string>("Had", "Has"),
                                                                                                                                                 new KeyValuePair<string, string>("had", "has"),
                                                                                                                                                 new KeyValuePair<string, string>("Has", "Has"),
                                                                                                                                                 new KeyValuePair<string, string>("has", "has"),
                                                                                                                                                 new KeyValuePair<string, string>("Is", "Is"),
                                                                                                                                                 new KeyValuePair<string, string>("is", "is"),
                                                                                                                                                 new KeyValuePair<string, string>("frozen", "freezes"),
                                                                                                                                                 new KeyValuePair<string, string>("Frozen", "Freezes"),
                                                                                                                                                 new KeyValuePair<string, string>("Were", "Is"),
                                                                                                                                                 new KeyValuePair<string, string>("were", "is"),
                                                                                                                                                 new KeyValuePair<string, string>("Was", "Is"),
                                                                                                                                                 new KeyValuePair<string, string>("was", "is"),
                                                                                                                                             });

        private static readonly string[] ThirdPersonalSingularVerbExceptions =
                                                                               {
                                                                                   "argument", "Argument",
                                                                                   "exception", "Exception",
                                                                                   "false", "False",
                                                                                   "guid", "Guid",
                                                                                   "int", "Int",
                                                                                   "invalid", "Invalid",
                                                                                   "json", "Json",
                                                                                   "key", "Key",
                                                                                   "no", "No",
                                                                                   "not", "Not",
                                                                                   "null", "Null",
                                                                                   "object", "Object",
                                                                                   "Operation",
                                                                                   "should", "Should",
                                                                                   "string", "String",
                                                                                   "task", "Task",
                                                                                   "true", "True",
                                                                                   "unauthorized", "Unauthorized",
                                                                                   "valid", "Valid",
                                                                                   "validation", "Validation",
                                                                               };

        private static readonly string[] NounsWithGerundEnding =
                                                                 {
                                                                     "awakening", "awning",
                                                                     "blessing", "booking", "briefing", "building",
                                                                     "ceiling",
                                                                     "darling", "dealing", "drawing", "duckling",
                                                                     "evening",
                                                                     "feeling", "finding", "fledgling",
                                                                     "gathering", "guttering",
                                                                     "hireling",
                                                                     "inkling",
                                                                     "leaning",
                                                                     "meeting", "misgiving", "misunderstanding", "morning",
                                                                     "offering", "outing",
                                                                     "painting",
                                                                     "quisling",
                                                                     "reasoning", "recording", "restructuring", "rising", "roofing",
                                                                     "sapling", "seasoning", "seating", "setting", "shooting", "shopping", "sibling", "sitting", "standing",
                                                                     "tiding", "timing", "training",
                                                                     "underling", "understanding", "undertaking", "upbringing", "uprising",
                                                                     "warning", "wedding", "well-being", "winning", "wording",
                                                                 };

        private static readonly Pair[] Endings =
                                                 {
                                                     new Pair(nameof(Action), nameof(Action)),
                                                     new Pair("Caption", "Caption"),
                                                     new Pair("cution", "cute"),
                                                     new Pair(nameof(Exception), nameof(Exception)),
                                                     new Pair("Func", "Function"),
                                                     new Pair("Function", "Function"),
                                                     new Pair("estination", "estination"),
                                                     new Pair("mentation", "ment"),
                                                     new Pair("unction", "unction"),
                                                     new Pair("ptation", "pt"),
                                                     new Pair("iption", "ibe"),
                                                     new Pair("rmation", "rm"),
                                                     new Pair("allation", "all"),
                                                     new Pair("ellation", "el"),
                                                     new Pair("stration", "ster"),
                                                     new Pair("eration", "erate"),
                                                     new Pair("iration", "ire"),
                                                     new Pair("uration", "ure"),
                                                     new Pair("isition", "ire"),
                                                     new Pair("osition", "ose"),
                                                     new Pair("isation", "ise"),
                                                     new Pair("ization", "ize"),
                                                     new Pair("vocation", "voke"),
                                                     new Pair("ation", "ate"),
                                                     new Pair("ction", "ct"),
                                                     new Pair("ption", "pt"),
                                                     new Pair("rison", "re"),
                                                     new Pair("sis", "ze"),
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

        public static bool IsAdjectiveOrAdverb(in ReadOnlySpan<char> value, in StringComparison comparison = StringComparison.OrdinalIgnoreCase)
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

        public static bool IsThirdPersonSingularVerb(in ReadOnlySpan<char> value)
        {
            var length = value.Length;

            if (length < 2)
            {
                return false;
            }

            if (length is 4 && value.Equals("will", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (value[length - 1] != 's')
            {
                return false;
            }

            if (CharsForTwoCharacterEndingsWithS.Contains(value[length - 2]))
            {
                return false;
            }

            if (Pluralizer.IsPlural(value))
            {
                if (value.EndsWithAny(NonThirdPersonSingularEndingsWithS))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsThirdPersonSingularVerb(string value) => value != null && IsThirdPersonSingularVerb(value.AsSpan());

        public static bool IsTwoCharacterEndingsWithS(string value)
        {
            var length = value?.Length;

            if (length >= 2)
            {
                return value[length.Value - 1] is 's' && CharsForTwoCharacterEndingsWithS.Contains(value[length.Value - 2]);
            }

            return false;
        }

        public static bool IsPastTense(string value) => value != null && IsPastTense(value.AsSpan());

        public static bool IsPastTense(in ReadOnlySpan<char> value) => value.EndsWithAny(PastEndings, StringComparison.Ordinal);

        public static bool IsGerundVerb(string value) => value != null && IsGerundVerb(value.AsSpan());

        public static bool IsGerundVerb(in ReadOnlySpan<char> value)
        {
            if (value.Length <= 4)
            {
                return false;
            }

            if (value.EndsWith("ing", StringComparison.Ordinal))
            {
                if (value.EndsWith("ling", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (value.EndsWith("thing", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                for (int index = 0, length = NounsWithGerundEnding.Length; index < length; index++)
                {
                    var noun = NounsWithGerundEnding[index];

                    if (value.Equals(noun, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
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

                var gerundVerb = word.AsCachedBuilder()
                                     .Append("ing")
                                     .ReplaceWithProbe("ping", "pping")
                                     .ReplaceWithProbe("eing", "ing")
                                     .ReplaceWithProbe("uring", "urring")
                                     .ReplaceWithProbe("uting", "utting")
                                     .ToStringAndRelease();

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

                    if (wordLength is 4)
                    {
                        // ignore short word such as "ping" or "thing"
                        return word;
                    }

                    if (word.EndsWith("thing", StringComparison.OrdinalIgnoreCase))
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

                if (IsPastTense(word))
                {
                    return CreateFromPast(word);
                }

                return word;
            }

            string CreateFromPast(string word)
            {
                if (word.EndsWith("dded", StringComparison.Ordinal))
                {
                    return word.AsSpan(0, word.Length - 2).ToString();
                }

                if (word.EndsWith("tted", StringComparison.Ordinal))
                {
                    return word.AsSpan(0, word.Length - 3).ToString();
                }

                if (word.EndsWith("eed", StringComparison.Ordinal))
                {
                    return word;
                }

                if (word.EndsWithAny(SpecialPastEndings, StringComparison.Ordinal))
                {
                    return word.AsSpan(0, word.Length - 1).ToString();
                }

                return word.AsSpan(0, word.Length - 2).ToString();
            }
        }

        public static string MakeFirstWordInfiniteVerb(string text, in FirstWordAdjustment adjustment = FirstWordAdjustment.None)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return text;
            }

            return MakeFirstWordInfiniteVerb(text.AsSpan(), adjustment);
        }

        public static string MakeFirstWordInfiniteVerb(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var valueText = text.TrimStart(Constants.SentenceMarkers);

            if (valueText.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // first word
            var firstWord = GetFirstWord(valueText, firstWordAdjustment);

            var infiniteVerb = MakeInfiniteVerb(firstWord);

            if (firstWord != infiniteVerb)
            {
                return infiniteVerb.ConcatenatedWith(valueText.WithoutFirstWord());
            }

            return text.ToString();

            string GetFirstWord(in ReadOnlySpan<char> span, in FirstWordAdjustment adjustment)
            {
                var word = span.FirstWord();

                switch (adjustment)
                {
                    case FirstWordAdjustment.StartLowerCase: return word.ToLowerCaseAt(0);
                    case FirstWordAdjustment.StartUpperCase: return word.ToUpperCaseAt(0);
                    default:
                        return word.ToString();
                }
            }
        }

        public static string MakeThirdPersonSingularVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            if (value.EqualsAny(ThirdPersonalSingularVerbExceptions))
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

                if (word.EndsWithAny(SpecialPastEndings, StringComparison.Ordinal))
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

            for (int index = 0, length = Endings.Length; index < length; index++)
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

        private static bool HasAcceptableStartingPhrase(in ReadOnlySpan<char> value)
        {
            for (int index = 0, length = StartingPhrases.Length; index < length; index++)
            {
                var phrase = StartingPhrases[index];

                if (value.StartsWith(phrase, StringComparison.Ordinal))
                {
                    var remaining = value.Slice(phrase.Length);

                    return remaining.Length is 0 || remaining[0].IsUpperCase();
                }
            }

            return false;
        }

        private static bool HasAcceptableMiddlePhrase(string value) => value.ContainsAny(MiddlePhrases, StringComparison.Ordinal);

        private static bool HasAcceptableEndingPhrase(in ReadOnlySpan<char> value) => value.EndsWithAny(EndingPhrases, StringComparison.Ordinal);
    }
}
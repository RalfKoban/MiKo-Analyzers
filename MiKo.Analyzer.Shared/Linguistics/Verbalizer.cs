using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MiKoSolutions.Analyzers.Linguistics
{
    /// <summary>
    /// Provides functionality to convert words and phrases between different verb forms.
    /// </summary>
    internal static class Verbalizer
    {
        private static readonly HashSet<char> CharsForTwoCharacterEndingsWithS = new HashSet<char> { 'a', 'h', 'i', 'o', 's', 'u', 'x', 'z' };

        private static readonly string[] NonThirdPersonSingularEndingsWithS = { "pters", "tors", "gers", "chers", "asses" };

        private static readonly string[] ExceptionsToNonThirdPersonSingularEndingsWithS = { "factors", "monitors", "triggers", "passes" };

        private static readonly string[] SpecialPastEndings = { "ated", "dled", "ced", "ged", "ied", "red", "rized", "sed", "ved", "oked" };

        private static readonly string[] PastEndings = SpecialPastEndings.ConcatenatedWith("led", "eed", "ted", "ded").ToArray();

        private static readonly string[] FourCharacterGerundEndings = { "pping", "rring", "tting" };

        private static readonly string[] ThreeCharacterGerundEndings = { "anging", "inging", "ssing", "cting", "pting", "rting", "enting" };

        private static readonly string[] ThreeCharacterGerundEndingsWithE = { "bling", "kling", "ging", "sing", "ting", "uing", "ving", "zing", "mining" };

        private static readonly ConcurrentDictionary<string, string> GerundVerbs = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        private static readonly ConcurrentDictionary<string, string> InfiniteVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                          new[]
                                                                                                                              {
                                                                                                                                  new KeyValuePair<string, string>("broken", "break"),
                                                                                                                                  new KeyValuePair<string, string>("Broken", "Break"),
                                                                                                                                  new KeyValuePair<string, string>("chosen", "choose"),
                                                                                                                                  new KeyValuePair<string, string>("Chosen", "Choose"),
                                                                                                                                  new KeyValuePair<string, string>("done", "do"),
                                                                                                                                  new KeyValuePair<string, string>("Done", "Do"),
                                                                                                                                  new KeyValuePair<string, string>("driven", "drive"),
                                                                                                                                  new KeyValuePair<string, string>("Driven", "Drive"),
                                                                                                                                  new KeyValuePair<string, string>("drove", "drive"),
                                                                                                                                  new KeyValuePair<string, string>("Drove", "Drive"),
                                                                                                                                  new KeyValuePair<string, string>("felt", "feel"),
                                                                                                                                  new KeyValuePair<string, string>("Felt", "Feel"),
                                                                                                                                  new KeyValuePair<string, string>("frozen", "freeze"),
                                                                                                                                  new KeyValuePair<string, string>("Frozen", "Freeze"),
                                                                                                                                  new KeyValuePair<string, string>("hid", "hide"),
                                                                                                                                  new KeyValuePair<string, string>("Hid", "Hide"),
                                                                                                                                  new KeyValuePair<string, string>("hidden", "hide"),
                                                                                                                                  new KeyValuePair<string, string>("Hidden", "Hide"),
                                                                                                                                  new KeyValuePair<string, string>("woke", "wake"),
                                                                                                                                  new KeyValuePair<string, string>("Woke", "Wake"),
                                                                                                                                  new KeyValuePair<string, string>("bent", "bend"),
                                                                                                                                  new KeyValuePair<string, string>("Bent", "Bend"),
                                                                                                                                  new KeyValuePair<string, string>("lent", "lend"),
                                                                                                                                  new KeyValuePair<string, string>("Lent", "Lend"),
                                                                                                                                  new KeyValuePair<string, string>("sent", "send"),
                                                                                                                                  new KeyValuePair<string, string>("Sent", "Send"),
                                                                                                                                  new KeyValuePair<string, string>("spent", "spend"),
                                                                                                                                  new KeyValuePair<string, string>("Spent", "Spend"),
                                                                                                                                  new KeyValuePair<string, string>("spoken", "speak"),
                                                                                                                                  new KeyValuePair<string, string>("Spoken", "Speak"),
                                                                                                                                  new KeyValuePair<string, string>("woken", "wake"),
                                                                                                                                  new KeyValuePair<string, string>("Woken", "Wake"),
                                                                                                                                  new KeyValuePair<string, string>("written", "write"),
                                                                                                                                  new KeyValuePair<string, string>("Written", "Write"),
                                                                                                                                  new KeyValuePair<string, string>("wrote", "write"),
                                                                                                                                  new KeyValuePair<string, string>("Wrote", "Write"),
                                                                                                                              },
                                                                                                                          StringComparer.Ordinal);

        private static readonly ConcurrentDictionary<string, string> ThirdPersonSingularVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                                     new[]
                                                                                                                                         {
                                                                                                                                             new KeyValuePair<string, string>("are", "is"),
                                                                                                                                             new KeyValuePair<string, string>("Are", "Is"),
                                                                                                                                             new KeyValuePair<string, string>("frozen", "freezes"),
                                                                                                                                             new KeyValuePair<string, string>("Frozen", "Freezes"),
                                                                                                                                             new KeyValuePair<string, string>("got", "gets"),
                                                                                                                                             new KeyValuePair<string, string>("Got", "Gets"),
                                                                                                                                             new KeyValuePair<string, string>("had", "has"),
                                                                                                                                             new KeyValuePair<string, string>("Had", "Has"),
                                                                                                                                             new KeyValuePair<string, string>("has", "has"),
                                                                                                                                             new KeyValuePair<string, string>("Has", "Has"),
                                                                                                                                             new KeyValuePair<string, string>("implementation", "implements"),
                                                                                                                                             new KeyValuePair<string, string>("Implementation", "Implements"),
                                                                                                                                             new KeyValuePair<string, string>("is", "is"),
                                                                                                                                             new KeyValuePair<string, string>("Is", "Is"),
                                                                                                                                             new KeyValuePair<string, string>("maintenance", "maintains"),
                                                                                                                                             new KeyValuePair<string, string>("Maintenance", "Maintains"),
                                                                                                                                             new KeyValuePair<string, string>("was", "is"),
                                                                                                                                             new KeyValuePair<string, string>("Was", "Is"),
                                                                                                                                             new KeyValuePair<string, string>("were", "is"),
                                                                                                                                             new KeyValuePair<string, string>("Were", "Is"),
                                                                                                                                             new KeyValuePair<string, string>("trigger", "triggers"),
                                                                                                                                             new KeyValuePair<string, string>("triggered", "triggers"),
                                                                                                                                             new KeyValuePair<string, string>("Trigger", "Triggers"),
                                                                                                                                             new KeyValuePair<string, string>("Triggered", "Triggers"),
                                                                                                                                         },
                                                                                                                                     StringComparer.Ordinal);

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

        /// <summary>
        /// Determines whether the specified word is a third-person singular verb.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is a third-person singular verb; otherwise, <see langword="false"/>.
        /// </returns>
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
                if (value.EndsWithAny(NonThirdPersonSingularEndingsWithS, StringComparison.OrdinalIgnoreCase))
                {
                    return value.EndsWithAny(ExceptionsToNonThirdPersonSingularEndingsWithS, StringComparison.OrdinalIgnoreCase);
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified word is a third-person singular verb.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is a third-person singular verb; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsThirdPersonSingularVerb(string value) => value != null && IsThirdPersonSingularVerb(value.AsSpan());

        /// <summary>
        /// Determines whether the specified word ends with a two-character ending followed by 's'.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word ends with a two-character ending followed by 's'; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsTwoCharacterEndingsWithS(string value)
        {
            var length = value?.Length;

            if (length >= 2)
            {
                return value[length.Value - 1] is 's' && CharsForTwoCharacterEndingsWithS.Contains(value[length.Value - 2]);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified word is in past tense.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is in past tense; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsPastTense(string value) => value != null && IsPastTense(value.AsSpan());

        /// <summary>
        /// Determines whether the specified word is in past tense.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is in past tense; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsPastTense(in ReadOnlySpan<char> value) => value.EndsWithAny(PastEndings);

        /// <summary>
        /// Determines whether the specified word is a gerund verb.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is a gerund verb; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsGerundVerb(string value) => value != null && IsGerundVerb(value.AsSpan());

        /// <summary>
        /// Determines whether the specified word is a gerund verb.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is a gerund verb; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsGerundVerb(in ReadOnlySpan<char> value)
        {
            if (value.Length <= 4)
            {
                return false;
            }

            if (value.EndsWith("ing"))
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

        /// <summary>
        /// Converts the specified word to its gerund form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the gerund form of the word; or the original word if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
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

        /// <summary>
        /// Converts the specified word to its infinitive form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the infinitive form of the word; or the original word if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
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

                    if (word.Equals("was", StringComparison.OrdinalIgnoreCase))
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
                        return word.Substring(0, word.Length - 2);
                    }

                    return word.Substring(0, word.Length - 1);
                }

                if (word.EndsWith('t'))
                {
                    if (word.EndsWith("et", StringComparison.OrdinalIgnoreCase))
                    {
                        return word;
                    }

                    if (word.EndsWith("nt", StringComparison.OrdinalIgnoreCase))
                    {
                        if (word.EndsWith("ant", StringComparison.OrdinalIgnoreCase))
                        {
                            return word.Substring(0, word.Length - 1);
                        }

                        if (word.EndsWith("rnt", StringComparison.OrdinalIgnoreCase))
                        {
                            return word.Substring(0, word.Length - 1);
                        }

                        return word;
                    }

                    if (word.EndsWith("pt", StringComparison.OrdinalIgnoreCase))
                    {
                        if (word.EndsWith("ept", StringComparison.OrdinalIgnoreCase))
                        {
                            if (word.EndsWith("cept", StringComparison.OrdinalIgnoreCase))
                            {
                                return word;
                            }

                            return word.AsSpan(0, word.Length - 2).ConcatenatedWith("ep");
                        }

                        if (word.EndsWith("eapt", StringComparison.OrdinalIgnoreCase))
                        {
                            return word.Substring(0, word.Length - 1);
                        }

                        return word;
                    }

                    if (word.EndsWith("lt", StringComparison.OrdinalIgnoreCase))
                    {
                        if (word.EndsWith("elt", StringComparison.OrdinalIgnoreCase))
                        {
                            return word.AsSpan(0, word.Length - 1).ConcatenatedWith('l');
                        }

                        if (word.EndsWith("alt", StringComparison.OrdinalIgnoreCase))
                        {
                            return word.Substring(0, word.Length - 1);
                        }

                        if (word.EndsWith("ilt", StringComparison.OrdinalIgnoreCase))
                        {
                            return word.AsSpan(0, word.Length - 1).ConcatenatedWith('d');
                        }
                    }

                    if (word.EndsWith("amt", StringComparison.OrdinalIgnoreCase))
                    {
                        return word.Substring(0, word.Length - 1);
                    }

                    return word;
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
                    return word.Substring(0, word.Length - 2);
                }

                if (word.EndsWith("tted", StringComparison.Ordinal))
                {
                    return word.Substring(0, word.Length - 3);
                }

                if (word.EndsWith("eed", StringComparison.Ordinal))
                {
                    return word;
                }

                if (word.EndsWithAny(SpecialPastEndings))
                {
                    return word.Substring(0, word.Length - 1);
                }

                return word.Substring(0, word.Length - 2);
            }
        }

        /// <summary>
        /// Converts the first word of the specified text to its infinitive form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its infinitive form; or the original text if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordInfiniteVerb(string text, in FirstWordAdjustment adjustment = FirstWordAdjustment.None)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return text;
            }

            return MakeFirstWordInfiniteVerb(text.AsSpan(), adjustment);
        }

        /// <summary>
        /// Converts the first word of the specified text to its infinitive form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its infinitive form; or the <see cref="string.Empty"/> string ("") if the text is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
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

        /// <summary>
        /// Converts the specified word to its third-person singular form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the third-person singular form of the word; or the original word if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeThirdPersonSingularVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            if (value.EqualsAny(ThirdPersonalSingularVerbExceptions, StringComparison.OrdinalIgnoreCase))
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
                if (word.EndsWith("eed", StringComparison.Ordinal))
                {
                    return word.AsSpan().ConcatenatedWith('s');
                }

                var difference = 2;

                if (word.EndsWith("tted", StringComparison.Ordinal))
                {
                    difference = 3;
                }
                else if (word.EndsWithAny(SpecialPastEndings) && word.EndsWith("tored", StringComparison.Ordinal) is false)
                {
                    difference = 1;
                }

                return word.AsSpan(0, word.Length - difference).ConcatenatedWith('s');
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

        /// <summary>
        /// Attempts to convert the specified word to its verb form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <param name="result">
        /// On successful return, contains the verb form of the word; otherwise the original word.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word was successfully converted to a verb form; otherwise, <see langword="false"/>.
        /// </returns>
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

                if (span.EndsWith(key))
                {
                    result = span.Slice(0, span.Length - key.Length).ConcatenatedWith(pair.Value);

                    return result.Equals(value, StringComparison.Ordinal) is false;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified word starts with an acceptable phrase.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word starts with an acceptable phrase; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasAcceptableStartingPhrase(in ReadOnlySpan<char> value)
        {
            for (int index = 0, length = StartingPhrases.Length; index < length; index++)
            {
                var phrase = StartingPhrases[index];

                if (value.StartsWith(phrase))
                {
                    var remaining = value.Slice(phrase.Length);

                    return remaining.Length is 0 || remaining[0].IsUpperCase();
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified word contains an acceptable phrase.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word contains an acceptable phrase; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasAcceptableMiddlePhrase(string value) => value.ContainsAny(MiddlePhrases);

        /// <summary>
        /// Determines whether the specified word ends with an acceptable phrase.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word ends with an acceptable phrase; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasAcceptableEndingPhrase(in ReadOnlySpan<char> value) => value.EndsWithAny(EndingPhrases);
    }
}
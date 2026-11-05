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

        private static readonly string[] NonThirdPersonSingularEndingsWithS = { "pters", "tors", "gers", "chers", "asses", "blings", "klings" };

        private static readonly string[] ExceptionsToNonThirdPersonSingularEndingsWithS = { "factors", "monitors", "triggers", "passes" };

        private static readonly string[] SpecialPastEndings = { "ated", "dled", "ced", "ged", "ied", "red", "rized", "sed", "ved", "oked", "ined", "ched", "loned", "poned" };

        private static readonly string[] PastEndings = SpecialPastEndings.ConcatenatedWith("led", "eed", "ted", "ded", "ned", "hed").ToArray();

        private static readonly string[] PastEndingsWithoutConsonantDoubling = { "get", "ait", "dit", "sit", "fit", "xit", "imit", "ffer", "ain", "elop", "eat", "oot" };

        private static readonly string[] PastEndingsWithConsonantDoubling = { "lan", "fer", "ur", "ab", "pel", "trol" };

        private static readonly string[] FourCharacterGerundEndings = { "nning", "pping", "rring", "tting" };

        private static readonly string[] ThreeCharacterGerundEndings = { "anging", "inging", "ssing", "cting", "pting", "rting", "enting", "rmating", "ioning", "aining" };

        private static readonly string[] ThreeCharacterGerundEndingsWithE = { "bling", "dling", "kling", "ging", "sing", "ting", "uing", "ving", "zing", "mining", "reating", "oning", "ining", "ching" };

        private static readonly string[] EndingsWithConsonantDoublingForGerund = { "et", "in", "it", "un" };

        private static readonly string[] EndingsWithoutConsonantDoublingForGerund = { "ain", "oin", "ait", "dit", "sit", "fit", "xit", };

        private static readonly ConcurrentDictionary<string, string> GerundVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                        new[]
                                                                                                                            {
                                                                                                                                Paired("shutdown", "shuttingDown"),
                                                                                                                                Paired("Shutdown", "ShuttingDown"),
                                                                                                                                Paired("shutDown", "shuttingDown"),
                                                                                                                                Paired("ShutDown", "ShuttingDown"),
                                                                                                                            },
                                                                                                                        StringComparer.Ordinal);

        private static readonly ConcurrentDictionary<string, string> InfiniteVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                          new[]
                                                                                                                              {
                                                                                                                                  Paired("broken", "break"),
                                                                                                                                  Paired("Broken", "Break"),
                                                                                                                                  Paired("chosen", "choose"),
                                                                                                                                  Paired("Chosen", "Choose"),
                                                                                                                                  Paired("done", "do"),
                                                                                                                                  Paired("Done", "Do"),
                                                                                                                                  Paired("driven", "drive"),
                                                                                                                                  Paired("Driven", "Drive"),
                                                                                                                                  Paired("drove", "drive"),
                                                                                                                                  Paired("Drove", "Drive"),
                                                                                                                                  Paired("felt", "feel"),
                                                                                                                                  Paired("Felt", "Feel"),
                                                                                                                                  Paired("frozen", "freeze"),
                                                                                                                                  Paired("Frozen", "Freeze"),
                                                                                                                                  Paired("hid", "hide"),
                                                                                                                                  Paired("Hid", "Hide"),
                                                                                                                                  Paired("hidden", "hide"),
                                                                                                                                  Paired("Hidden", "Hide"),
                                                                                                                                  Paired("woke", "wake"),
                                                                                                                                  Paired("Woke", "Wake"),
                                                                                                                                  Paired("bent", "bend"),
                                                                                                                                  Paired("Bent", "Bend"),
                                                                                                                                  Paired("lent", "lend"),
                                                                                                                                  Paired("Lent", "Lend"),
                                                                                                                                  Paired("sent", "send"),
                                                                                                                                  Paired("Sent", "Send"),
                                                                                                                                  Paired("spent", "spend"),
                                                                                                                                  Paired("Spent", "Spend"),
                                                                                                                                  Paired("spoken", "speak"),
                                                                                                                                  Paired("Spoken", "Speak"),
                                                                                                                                  Paired("woken", "wake"),
                                                                                                                                  Paired("Woken", "Wake"),
                                                                                                                                  Paired("written", "write"),
                                                                                                                                  Paired("Written", "Write"),
                                                                                                                                  Paired("wrote", "write"),
                                                                                                                                  Paired("Wrote", "Write"),
                                                                                                                                  Paired("Began", "Begin"),
                                                                                                                                  Paired("began", "begin"),
                                                                                                                                  Paired("Ran", "Run"),
                                                                                                                                  Paired("ran", "run"),
                                                                                                                                  Paired("Flew", "Fly"),
                                                                                                                                  Paired("flew", "fly"),
                                                                                                                              },
                                                                                                                          StringComparer.Ordinal);

        private static readonly ConcurrentDictionary<string, string> PastVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                      new[]
                                                                                                                          {
                                                                                                                              Paired("are", "were"),
                                                                                                                              Paired("Are", "Were"),
                                                                                                                              Paired("ban", "banned"),
                                                                                                                              Paired("Ban", "Banned"),
                                                                                                                              Paired("begin", "began"),
                                                                                                                              Paired("Begin", "Began"),
                                                                                                                              Paired("cut", "cut"),
                                                                                                                              Paired("Cut", "Cut"),
                                                                                                                              Paired("fit", "fitted"),
                                                                                                                              Paired("Fit", "Fitted"),
                                                                                                                              Paired("flew", "flew"),
                                                                                                                              Paired("Flew", "Flew"),
                                                                                                                              Paired("fly", "flew"),
                                                                                                                              Paired("Fly", "Flew"),
                                                                                                                              Paired("flying", "flew"),
                                                                                                                              Paired("Flying", "Flew"),
                                                                                                                              Paired("forget", "forgot"),
                                                                                                                              Paired("Forget", "Forgot"),
                                                                                                                              Paired("freeze", "frozen"),
                                                                                                                              Paired("Freeze", "Frozen"),
                                                                                                                              Paired("freezes", "frozen"),
                                                                                                                              Paired("Freezes", "Frozen"),
                                                                                                                              Paired("freezing", "frozen"),
                                                                                                                              Paired("Freezing", "Frozen"),
                                                                                                                              Paired("get", "got"),
                                                                                                                              Paired("Get", "Got"),
                                                                                                                              Paired("had", "had"),
                                                                                                                              Paired("Had", "Had"),
                                                                                                                              Paired("has", "had"),
                                                                                                                              Paired("Has", "Had"),
                                                                                                                              Paired("have", "had"),
                                                                                                                              Paired("Have", "Had"),
                                                                                                                              Paired("hit", "hit"),
                                                                                                                              Paired("Hit", "Hit"),
                                                                                                                              Paired("input", "input"),
                                                                                                                              Paired("Input", "Input"),
                                                                                                                              Paired("is", "was"),
                                                                                                                              Paired("Is", "Was"),
                                                                                                                              Paired("let", "let"),
                                                                                                                              Paired("Let", "Let"),
                                                                                                                              Paired("offset", "offset"),
                                                                                                                              Paired("Offset", "Offset"),
                                                                                                                              Paired("output", "output"),
                                                                                                                              Paired("Output", "Output"),
                                                                                                                              Paired("pin", "pinned"),
                                                                                                                              Paired("Pin", "Pinned"),
                                                                                                                              Paired("put", "put"),
                                                                                                                              Paired("Put", "Put"),
                                                                                                                              Paired("quit", "quit"),
                                                                                                                              Paired("Quit", "Quit"),
                                                                                                                              Paired("reset", "reset"),
                                                                                                                              Paired("Reset", "Reset"),
                                                                                                                              Paired("run", "ran"),
                                                                                                                              Paired("Run", "Ran"),
                                                                                                                              Paired("set", "set"),
                                                                                                                              Paired("Set", "Set"),
                                                                                                                              Paired("upset", "upset"),
                                                                                                                              Paired("Upset", "Upset"),
                                                                                                                              Paired("write", "wrote"),
                                                                                                                              Paired("Write", "Wrote"),
                                                                                                                              Paired("wrote", "wrote"),
                                                                                                                              Paired("Wrote", "Wrote"),
                                                                                                                          },
                                                                                                                      StringComparer.Ordinal);

        private static readonly HashSet<string> PastVerbsOnly = PastVerbs.Values.ToHashSet();

        private static readonly ConcurrentDictionary<string, string> ThirdPersonSingularVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                                     new[]
                                                                                                                                         {
                                                                                                                                             Paired("are", "is"),
                                                                                                                                             Paired("Are", "Is"),
                                                                                                                                             Paired("frozen", "freezes"),
                                                                                                                                             Paired("Frozen", "Freezes"),
                                                                                                                                             Paired("got", "gets"),
                                                                                                                                             Paired("Got", "Gets"),
                                                                                                                                             Paired("had", "has"),
                                                                                                                                             Paired("Had", "Has"),
                                                                                                                                             Paired("has", "has"),
                                                                                                                                             Paired("Has", "Has"),
                                                                                                                                             Paired("is", "is"),
                                                                                                                                             Paired("Is", "Is"),
                                                                                                                                             Paired("maintenance", "maintains"),
                                                                                                                                             Paired("Maintenance", "Maintains"),
                                                                                                                                             Paired("was", "is"),
                                                                                                                                             Paired("Was", "Is"),
                                                                                                                                             Paired("were", "is"),
                                                                                                                                             Paired("Were", "Is"),
                                                                                                                                             Paired("trigger", "triggers"),
                                                                                                                                             Paired("triggered", "triggers"),
                                                                                                                                             Paired("Trigger", "Triggers"),
                                                                                                                                             Paired("Triggered", "Triggers"),
                                                                                                                                             Paired("calc", "calculates"),
                                                                                                                                             Paired("Calc", "Calculates"),
                                                                                                                                             Paired("recalc", "recalculates"),
                                                                                                                                             Paired("Recalc", "Recalculates"),
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
                                                                                   "should", "Should",
                                                                                   "string", "String",
                                                                                   "task", "Task",
                                                                                   "true", "True",
                                                                                   "unauthorized", "Unauthorized",
                                                                                   "valid", "Valid",
                                                                               };

        private static readonly string[] Nouns =
                                                 {
                                                     "File",
                                                     "Name",
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
                                                     new Pair("istration", "ister"),
                                                     new Pair("eration", "erate"),
                                                     new Pair("iration", "ire"),
                                                     new Pair("uration", "ure"),
                                                     new Pair("isition", "ire"),
                                                     new Pair("osition", "ose"),
                                                     new Pair("isation", "ise"),
                                                     new Pair("ization", "ize"),
                                                     new Pair("vocation", "voke"),
                                                     new Pair("fication", "fy"),
                                                     new Pair("ation", "ate"),
                                                     new Pair("duction", "duce"),
                                                     new Pair("ction", "ct"),
                                                     new Pair("ption", "pt"),
                                                     new Pair("rison", "re"),
                                                     new Pair("sis", "ze"),
                                                 };

        private static readonly string[] IsAre = { "is", "are" };

        private static readonly string[] StartingPhrases = new[]
                                                               {
                                                                   "Abort",
                                                                   "Activate",
                                                                   "Add",
                                                                   "Analyze",
                                                                   "Apply",
                                                                   "Arrange",
                                                                   "Assert",
                                                                   "Begin",
                                                                   "Build",
                                                                   "Calculate",
                                                                   "Can",
                                                                   "Cancel",
                                                                   "Change",
                                                                   "Clear",
                                                                   "Clone",
                                                                   "Close",
                                                                   "Collect",
                                                                   "CompileTimeValidate",
                                                                   "Continue",
                                                                   "Create",
                                                                   "Deactivate",
                                                                   "Debug",
                                                                   "Delay",
                                                                   "Delete",
                                                                   "Deregister",
                                                                   "Deselect",
                                                                   "End",
                                                                   "Ensure",
                                                                   "Find",
                                                                   "Free",
                                                                   "Generate",
                                                                   "Get",
                                                                   "get_",
                                                                   "Handle",
                                                                   "Has",
                                                                   "Init",
                                                                   "Initialize",
                                                                   "Invert",
                                                                   "Is",
                                                                   "JumpTo",
                                                                   "Load",
                                                                   "Log",
                                                                   "Mirror",
                                                                   "Modify",
                                                                   "Move",
                                                                   "NavigateTo",
                                                                   "On",
                                                                   "Open",
                                                                   "Parse",
                                                                   "Pause",
                                                                   "Pop",
                                                                   "Prepare",
                                                                   "PromptFor",
                                                                   "Propagate",
                                                                   "Push",
                                                                   "Query",
                                                                   "Read",
                                                                   "Rebuild",
                                                                   "Record",
                                                                   "Recover",
                                                                   "Redo",
                                                                   "Refresh",
                                                                   "Register",
                                                                   "Release",
                                                                   "Reload",
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
                                                                   "Subtract",
                                                                   "Suspend",
                                                                   "To",
                                                                   "Trace",
                                                                   "Translate",
                                                                   "Trigger",
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
        /// Determines whether the first word of the specified text is a gerund form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the first word is a gerund form; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsFirstWordGerundVerb(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return false;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var firstWord = text.AsSpan().TrimStart(Constants.SentenceMarkers).FirstWord();

            return IsGerundVerb(firstWord);
        }

        /// <summary>
        /// Determines whether the first word of the specified text is an infinite form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the first word is an infinite form; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsFirstWordInfiniteVerb(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return false;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var trimmed = text.AsSpan().TrimStart(Constants.SentenceMarkers);
            var firstWordSpan = trimmed.FirstWord();
            var firstWord = firstWordSpan.Length == text.Length ? text : firstWordSpan.ToString();

            if (StartingPhrases.Contains(firstWord, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            if (firstWord.EqualsAny(Nouns, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (NounsWithGerundEnding.Contains(firstWord, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            if (IsThirdPersonSingularVerb(firstWord))
            {
                return false;
            }

            if (firstWord.EndsWithAny("eh"))
            {
                return true;
            }

            if (firstWord.EndsWith("at", StringComparison.Ordinal) || firstWord.EndsWith("nd", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the first word of the specified text is a past form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the first word is a past form; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsFirstWordPastTenseVerb(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return false;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var firstWord = text.AsSpan().TrimStart(Constants.SentenceMarkers).FirstWord();

            return IsPastTenseVerb(firstWord);
        }

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
        /// Determines whether the specified word is in past tense.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is in past tense; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsPastTenseVerb(string value)
        {
            if (value is null)
            {
                return false;
            }

            if (PastVerbsOnly.Contains(value))
            {
                return true;
            }

            return IsPastTenseVerb(value.AsSpan());
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
        public static bool IsPastTenseVerb(in ReadOnlySpan<char> value) => value.EndsWithAny(PastEndings) || value.EqualsAny(PastVerbsOnly);

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
        /// Converts the first word of the specified text to its gerund form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its gerund form; or the original text if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordGerundVerb(string text, in FirstWordAdjustment adjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return text;
            }

            return MakeFirstWordGerundVerb(text.AsSpan(), adjustment, wordBoundary);
        }

        /// <summary>
        /// Converts the first word of the specified text to its gerund form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its gerund form; or the <see cref="string.Empty"/> string ("") if the text is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordGerundVerb(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
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

            var verb = MakeGerundVerb(firstWord);

            if (firstWord != verb)
            {
                return verb.ConcatenatedWith(valueText.WithoutFirstWord(wordBoundary));
            }

            return text.ToString();
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
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its infinitive form; or the original text if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordInfiniteVerb(string text, in FirstWordAdjustment adjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return text;
            }

            return MakeFirstWordInfiniteVerb(text.AsSpan(), adjustment, wordBoundary);
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
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its infinitive form; or the <see cref="string.Empty"/> string ("") if the text is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordInfiniteVerb(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
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

            var verb = MakeInfiniteVerb(firstWord);

            if (firstWord != verb)
            {
                return verb.ConcatenatedWith(valueText.WithoutFirstWord(wordBoundary));
            }

            return text.ToString();
        }

        /// <summary>
        /// Converts the first word of the specified text to its past form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its past form; or the original text if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordPastTenseVerb(string text, in FirstWordAdjustment adjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return text;
            }

            return MakeFirstWordPastTenseVerb(text.AsSpan(), adjustment, wordBoundary);
        }

        /// <summary>
        /// Converts the first word of the specified text to its past form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its past form; or the <see cref="string.Empty"/> string ("") if the text is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordPastTenseVerb(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
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

            var verb = MakePastTenseVerb(firstWord);

            if (firstWord != verb)
            {
                return verb.ConcatenatedWith(valueText.WithoutFirstWord(wordBoundary));
            }

            return text.ToString();
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
                if (IsPastTenseVerb(word))
                {
                    word = MakeInfiniteVerb(word);
                }

                if (word.EqualsAny(IsAre, StringComparison.OrdinalIgnoreCase))
                {
                    return word;
                }

                if (word.Equals("has", StringComparison.OrdinalIgnoreCase))
                {
                    return word[0].IsUpperCaseLetter() ? "Having" : "having";
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

                if (WantsDoubleConstants(word))
                {
                    // duplicate word's last consonant
                    word += word[word.Length - 1];
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

            bool WantsDoubleConstants(string word)
            {
                if (word.EndsWithAny(EndingsWithoutConsonantDoublingForGerund))
                {
                    return false;
                }

                return word.EndsWithAny(EndingsWithConsonantDoublingForGerund);
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

                    if (word.EndsWith("oes", StringComparison.Ordinal)
                     || word.EndsWith("shes", StringComparison.Ordinal)
                     || word.EndsWith("sses", StringComparison.Ordinal)
                     || word.EndsWith("tches", StringComparison.Ordinal))
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
                        if (word.EndsWith("ant", StringComparison.OrdinalIgnoreCase) || word.EndsWith("rnt", StringComparison.OrdinalIgnoreCase))
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
                    if (word.Length is 4)
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
                        return word.Substring(0, word.Length - 4);
                    }

                    if (word.EndsWithAny(ThreeCharacterGerundEndings, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.Substring(0, word.Length - 3);
                    }

                    if (word.EndsWithAny(ThreeCharacterGerundEndingsWithE, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.AsSpan(0, word.Length - 3).ConcatenatedWith('e');
                    }

                    return word.Substring(0, word.Length - 3);
                }

                if (IsPastTenseVerb(word))
                {
                    return CreateFromPast(word);
                }

                if (word.EndsWith("ion", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (word.EndsWith("ndation", StringComparison.OrdinalIgnoreCase)
                     || word.EndsWith("tation", StringComparison.OrdinalIgnoreCase))
                    {
                        return word.Substring(0, word.Length - 5);
                    }

                    if (word.EndsWith("idation", StringComparison.OrdinalIgnoreCase)
                     || word.EndsWith("mation", StringComparison.OrdinalIgnoreCase)
                     || word.EndsWith("ration", StringComparison.OrdinalIgnoreCase))
                    {
                        return word.AsSpan(0, word.Length - 3).ConcatenatedWith('e');
                    }

                    if (word.EndsWith("nsion", StringComparison.OrdinalIgnoreCase))
                    {
                        return word.AsSpan(0, word.Length - 4).ConcatenatedWith('d');
                    }

                    return word.Substring(0, word.Length - 3);
                }

                return word;
            }

            string CreateFromPast(string word)
            {
                if (word.Equals("were", StringComparison.OrdinalIgnoreCase))
                {
                    return word[0].IsUpperCaseLetter() ? "Be" : "be";
                }

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
        /// Converts the specified word to its past form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the past form of the word; or the original word if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakePastTenseVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            return PastVerbs.GetOrAdd(value, CreatePastVerbWithAdd);

            string CreatePastVerbWithAdd(string word)
            {
                var pastVerb = CreatePastVerb(word);

                PastVerbsOnly.Add(pastVerb);

                return pastVerb;
            }

            string CreatePastVerb(string word)
            {
                if (word.EndsWith("eed", StringComparison.Ordinal))
                {
                    return word + "ed";
                }

                if (word.EndsWith("ed", StringComparison.Ordinal))
                {
                    return word;
                }

                if (IsGerundVerb(word) || IsThirdPersonSingularVerb(word))
                {
                    word = MakeInfiniteVerb(word);

                    if (PastVerbs.TryGetValue(word, out var pastTense))
                    {
                        return pastTense;
                    }
                }

                if (word.EndsWith('e'))
                {
                    return word + "d";
                }

                if (word.EndsWith('y'))
                {
                    if (word.EndsWith("ay", StringComparison.Ordinal) || word.EndsWith("ey", StringComparison.Ordinal))
                    {
                        return word.AsSpan().ConcatenatedWith("ed");
                    }

                    return word.AsSpan(0, word.Length - 1).ConcatenatedWith("ied");
                }

                if (word.EndsWith("ing", StringComparison.Ordinal))
                {
                    return word.AsSpan(0, word.Length - 3).ConcatenatedWith("ed");
                }

                if (WantsDoubleConstants(word))
                {
                    // duplicate word's last consonant
                    return word.AsSpan().ConcatenatedWith(word[word.Length - 1], "ed");
                }

                return word + "ed";
            }

            bool WantsDoubleConstants(string word)
            {
                if (word.EndsWithAny(PastEndingsWithoutConsonantDoubling))
                {
                    return false;
                }

                if (word.EndsWithAny(PastEndingsWithConsonantDoubling))
                {
                    return true;
                }

                return word.EndsWithAny("gmpt") && word.Length > 1 && word[word.Length - 2].EqualsAny("aeiou");
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

                if (word.EndsWith("ion", StringComparison.Ordinal))
                {
                    var infinite = MakeInfiniteVerb(word);

                    if (word != infinite)
                    {
                        return MakeThirdPersonSingularVerb(infinite);
                    }
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

                if (span.EndsWith(pair.Key))
                {
                    result = span.Slice(0, span.Length - pair.Key.Length).ConcatenatedWith(pair.Value);

                    return result.Equals(value, StringComparison.Ordinal) is false;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the first word of the specified text and adjusts it based on the given adjustment.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to return.
        /// </param>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the adjusted first word; or the <see cref="string.Empty"/> string ("") if the text is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        private static string GetFirstWord(in ReadOnlySpan<char> text, in FirstWordAdjustment adjustment = FirstWordAdjustment.None)
        {
            var word = text.FirstWord();

            switch (adjustment)
            {
                case FirstWordAdjustment.StartLowerCase: return word.ToLowerCaseAt(0);
                case FirstWordAdjustment.StartUpperCase: return word.ToUpperCaseAt(0);
                default:
                    return word.ToString();
            }
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

        /// <summary>
        /// Creates a <see cref="KeyValuePair{TKey,TValue}"/> for the given key and value.
        /// </summary>
        /// <param name="key">
        /// The word to use as <see cref="KeyValuePair{TKey,TValue}.Key"/>.
        /// </param>
        /// <param name="value">
        /// The word to use as <see cref="KeyValuePair{TKey,TValue}.Value"/>.
        /// </param>
        /// <returns>
        /// The <see cref="KeyValuePair{TKey,TValue}"/> for the given key and value.
        /// </returns>
        private static KeyValuePair<string, string> Paired(string key, string value) => new KeyValuePair<string, string>(key, value);
    }
}
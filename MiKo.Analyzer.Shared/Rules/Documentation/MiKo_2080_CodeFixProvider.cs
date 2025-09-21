using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2080_CodeFixProvider)), Shared]
    public sealed class MiKo_2080_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly Lazy<MapData> MappedData = new Lazy<MapData>();

#if !NCRUNCH // do not define a static ctor to speed up tests in NCrunch
        static MiKo_2080_CodeFixProvider() => LoadData(); // ensure that we have the object available
#endif

        public override string FixableDiagnosticId => "MiKo_2080";

        public static void LoadData() => GC.KeepAlive(MappedData.Value);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;
            var phrase = GetStartingPhraseProposal(issue);

            var data = MappedData.Value;

            var preparedComment = Comment(comment, data.PreparationMapKeys, data.PreparationMap);
            var preparedComment1 = Comment(preparedComment, data.TypeGuidReplacementMapKeys, data.TypeGuidReplacementMap);
            var preparedComment2 = Comment(preparedComment1, data.ReplacementMapKeys, data.ReplacementMap);

            var fixedComment = CommentStartingWith(preparedComment2, phrase);

            return Comment(fixedComment, data.CleanupMapKeys, data.CleanupMap);
        }

//// ncrunch: rdi off
//// ncrunch: no coverage start

        private sealed class MapData
        {
#pragma warning disable SA1401 // Fields should be private
            public readonly Pair[] ReplacementMap;
            public readonly string[] ReplacementMapKeys;
            public readonly string[] TypeGuidReplacementMapKeys;
            public readonly Pair[] TypeGuidReplacementMap;
            public readonly string[] PreparationMapKeys;
            public readonly Pair[] PreparationMap;
            public readonly string[] CleanupMapKeys;
            public readonly Pair[] CleanupMap;
#pragma warning restore SA1401 // Fields should be private

#pragma warning disable CA1861
            public MapData()
            {
                ReplacementMap = CreateReplacementMapPairs().OrderByDescending(_ => _.Key.Length) // get longest items first as shorter items may be part of the longer ones and would cause problems
                                                            .ThenBy(_ => _.Key)
                                                            .Concat(new[] { new Pair("Factory ", "a factory ") })
                                                            .ToArray();

                var keys = ReplacementMap.ToArray(_ => _.Key);

                ReplacementMapKeys = GetTermsForQuickLookup(keys);

                TypeGuidReplacementMapKeys = new[]
                                                 {
                                                     "A Type GUID for ",
                                                     "A Type GUID of ",
                                                     "A Type Guid for ",
                                                     "A Type Guid of ",
                                                     "A TypeGuid for ",
                                                     "A TypeGuid of ",
                                                     "The Type GUID for ",
                                                     "The Type GUID of ",
                                                     "The Type Guid for ",
                                                     "The Type Guid of ",
                                                     "The TypeGuid for ",
                                                     "The TypeGuid of ",
                                                     "Type GUID for ",
                                                     "Type GUID of ",
                                                     "Type Guid for ",
                                                     "Type Guid of ",
                                                     "TypeGuid for ",
                                                     "TypeGuid of ",
                                                     "TypeGuids for ", // typo
                                                     "TypeGuids of ", // typo
                                                 }.ToArray(AscendingStringComparer.Default);

                TypeGuidReplacementMap = TypeGuidReplacementMapKeys.OrderByDescending(_ => _.Length)
                                                                   .ThenBy(_ => _)
                                                                   .ToArray(_ => new Pair(_, "The unique identifier for the type of "));

                PreparationMap = new[]
                                     {
                                         new Pair("uperset", "#1#"), // prepare 'superset' as the 'set' would get replaced
                                         new Pair("uper-set", "#2#"), // prepare 'super-set' as the 'set' would get replaced
                                         new Pair("ubset", "#3#"), // prepare 'subset' as the 'set' would get replaced
                                         new Pair("ub-set", "#4#"), // prepare 'sub-set' as the 'set' would get replaced
                                     };

                PreparationMapKeys = PreparationMap.ToArray(_ => _.Key);

                CleanupMap = new[]
                                 {
                                     new Pair("#1#", "uperset"), // restore 'superset' as the 'set' would get replaced
                                     new Pair("#2#", "uper-set"), // restore 'super-set' as the 'set' would get replaced
                                     new Pair("#3#", "ubset"), // restore 'subset' as the 'set' would get replaced
                                     new Pair("#4#", "ub-set"), // restore 'sub-set' as the 'set' would get replaced

                                     new Pair(" a the ", " the "),
                                     new Pair(" an the ", " the "),
                                     new Pair(" the the ", " the "),
                                     new Pair("The a ", "The "),
                                     new Pair("The an ", "The "),
                                     new Pair("The the ", "The "),
                                     new Pair(" from from ", " from "),
                                     new Pair(" whether if ", " whether "),
                                     new Pair(" whether when ", " whether "),
                                     new Pair(" whether whether ", " whether "),
                                 };

                CleanupMapKeys = CleanupMap.ToArray(_ => _.Key);
            }
#pragma warning restore CA1861

            private static List<Pair> CreateReplacementMapPairs()
            {
                var keys = new List<string>(6_144);

                FillBooleanReplacementMapKeys(keys);
                FillGuidReplacementMapKeys(keys);
                FillCollectionReplacementMapKeys(keys);
                FillGetReplacementMapKeys(keys);
                FillOtherReplacementMapKeys(keys);

                var hashSet = new HashSet<string>(); // avoid duplicates

                foreach (var key in keys)
                {
                    hashSet.Add(key);
                    hashSet.Add(key.ToLowerCaseAt(0));
                }

                var results = new List<Pair>(8_192);
                results.AddRange(hashSet.Select(_ => new Pair(_)));

                FillSpecialCollectionReplacementPairs(results);

                return results;
            }

            private static void FillBooleanReplacementMapKeys(List<string> keys)
            {
                string[] booleans =
                                    {
                                        "A bool value",
                                        "A bool",
                                        "A boolean value",
                                        "A Boolean value",
                                        "A boolean",
                                        "A Boolean",
                                        "A flag",
                                        "A value",
                                        "The bool value",
                                        "The bool",
                                        "The boolean value",
                                        "The Boolean value",
                                        "The boolean",
                                        "The Boolean",
                                        "The flag",
                                        "The value",
                                    };

                var starts = new List<string>(booleans)
                                 {
                                     "Boolean value",
                                     "Bool value",
                                     "Boolean",
                                     "Bool",
                                     "Flag",
                                     "Value",
                                 };

                // build up the "contains xyz" terms
                foreach (var text in booleans)
                {
                    var lowerText = text.ToLowerCaseAt(0);

                    starts.Add("Contains " + lowerText);
                    starts.Add("Specifies " + lowerText);
                }

                string[] verbs = { "to indicate", "that indicates", "which indicates", "indicating", "to control", "that controls", "which controls", "controlling", "controling" }; // typo
                string[] continuations = { " if", " whether or not", " whether", " that", string.Empty };

                var results = new HashSet<string>();

                foreach (var start in starts)
                {
                    foreach (var verb in verbs)
                    {
                        var begin = start + " " + verb;

                        foreach (var continuation in continuations)
                        {
                            results.Add(begin + continuation + " ");
                        }
                    }
                }

                foreach (var start in new[] { "To", "Shall", "Should", "Will", "Would" })
                {
                    foreach (var continuation in continuations)
                    {
                        var end = continuation + " ";

                        results.Add(start + " control" + end);
                        results.Add(start + " control," + end);
                        results.Add(start + " indicate" + end);
                        results.Add(start + " indicate," + end);
                    }
                }

                foreach (var start in new[] { "Indicating", "Indicates", "Indicate", "Controlling", "Controling", "Controls", "Control" })
                {
                    foreach (var continuation in continuations)
                    {
                        if (continuation.IsNullOrEmpty() is false)
                        {
                            var end = continuation + " ";

                            results.Add(start + end);
                            results.Add(start + "," + end);
                        }
                    }
                }

                keys.AddRange(results);
            }

            private static void FillGuidReplacementMapKeys(List<string> keys)
            {
                string[] starts = { string.Empty, "A ", "The " };
                string[] types = { "GUID", "Guid", "guid", "TypeGuid", "Guids" };
                string[] continuations = { "of", "for" };

                foreach (var start in starts)
                {
                    foreach (var type in types)
                    {
                        var begin = start + type + " ";

                        foreach (var continuation in continuations)
                        {
                            keys.Add(begin + continuation + " ");
                        }
                    }
                }

                keys.Add("A unique identifier for "); // needed to avoid duplicate text in type guid comments
                keys.Add("An unique identifier for "); // needed to avoid duplicate text in type guid comments
                keys.Add("The unique identifier for "); // needed to avoid duplicate text in type guid comments
            }

            private static void FillCollectionReplacementMapKeys(List<string> keys)
            {
                keys.Add("Containing ");
                keys.Add("Storing ");
                keys.Add("Stores ");
                keys.Add("Holding ");
                keys.Add("Holds ");
                keys.Add("This is ");

                foreach (var begin in CollectionStartupPhrases())
                {
                    keys.Add(begin + " of all ");
                    keys.Add(begin + " of ");
                    keys.Add(begin + " for all ");
                    keys.Add(begin + " for ");

                    keys.Add(begin + " that holds ");
                    keys.Add(begin + " which holds ");
                    keys.Add(begin + " holds ");
                    keys.Add(begin + " holding ");
                    keys.Add(begin + " is holding ");
                    keys.Add(begin + " that is holding ");
                    keys.Add(begin + " which is holding ");

                    keys.Add(begin + " that contains ");
                    keys.Add(begin + " which contains ");
                    keys.Add(begin + " contains ");
                    keys.Add(begin + " containing ");
                    keys.Add(begin + " is containing ");
                    keys.Add(begin + " that is containing ");
                    keys.Add(begin + " which is containing ");

                    keys.Add(begin + " that stores ");
                    keys.Add(begin + " which stores ");
                    keys.Add(begin + " stores ");
                    keys.Add(begin + " storing ");
                    keys.Add(begin + " is storing ");
                    keys.Add(begin + " that is storing ");
                    keys.Add(begin + " which is storing ");
                }
            }

            private static void FillSpecialCollectionReplacementPairs(List<Pair> results)
            {
                const string Replacement = "mapping information from ";

                foreach (var begin in CollectionStartupPhrases())
                {
                    results.Add(new Pair(begin + " mapping ", Replacement));
                    results.Add(new Pair(begin + " mapping info ", Replacement));
                    results.Add(new Pair(begin + " mapping infos ", Replacement));
                    results.Add(new Pair(begin + " mapping information ", Replacement));
                    results.Add(new Pair(begin + " mapping informations ", Replacement));
                    results.Add(new Pair(begin + " mappings ", Replacement));
                    results.Add(new Pair(begin + " mappings info ", Replacement));
                    results.Add(new Pair(begin + " mappings information ", Replacement));
                }
            }

            private static void FillGetReplacementMapKeys(List<string> keys)
            {
                keys.Add("Gets ");
                keys.Add("Get ");
                keys.Add("Sets ");
                keys.Add("Set ");
                keys.Add("Gets or sets ");
                keys.Add("Gets or Sets ");
                keys.Add("Get or set ");
                keys.Add("Get or Set ");
                keys.Add("Return ");
                keys.Add("Returns ");

                // typos
                keys.Add("/Return ");
                keys.Add("/Returns ");
            }

            private static void FillOtherReplacementMapKeys(List<string> keys)
            {
                keys.Add("Defines ");
                keys.Add("Specifies ");
                keys.Add("Use this ");
                keys.Add("This ");
            }

            private static IEnumerable<string> CollectionStartupPhrases()
            {
                var articles = new[] { string.Empty, "A ", "An ", "The ", "This ", "a ", "an ", "the ", "this " };
                var starts = new[]
                                 {
                                     "Array", "array",
                                     "Collection", "collection",
                                     "List", "list",
                                     "Dictionary", "dictionary",
                                     "Cache", "cache",
                                     "Stack", "stack",
                                     "Queue", "queue",
                                     "Enumerable", "enumerable",
                                 };

                foreach (var article in articles)
                {
                    foreach (var start in starts)
                    {
                        yield return article + start;
                    }
                }
            }
        }

//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}
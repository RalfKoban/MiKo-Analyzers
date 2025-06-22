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
                ReplacementMap = CreateReplacementMapKeys().OrderByDescending(_ => _.Length) // get longest items first as shorter items may be part of the longer ones and would cause problems
                                                           .ThenBy(_ => _)
                                                           .Select(_ => new Pair(_))
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
                                 };

                CleanupMapKeys = CleanupMap.ToArray(_ => _.Key);
            }
#pragma warning restore CA1861

            private static HashSet<string> CreateReplacementMapKeys()
            {
                var keys = Enumerable.Empty<string>()
                                     .Concat(CreateBooleanReplacementMapKeys())
                                     .Concat(CreateGuidReplacementMapKeys())
                                     .Concat(CreateCollectionReplacementMapKeys())
                                     .Concat(CreateGetReplacementMapKeys())
                                     .Concat(CreateOtherReplacementMapKeys());

                var results = new HashSet<string>(); // avoid duplicates

                foreach (var key in keys)
                {
                    results.Add(key);
                    results.Add(key.ToLowerCaseAt(0));
                }

                return results;
            }

            private static HashSet<string> CreateBooleanReplacementMapKeys()
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

                foreach (var start in new[] { "Indicating", "Indicates", "Controlling", "Controling", "Controls" })
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

                return results;
            }

            private static List<string> CreateGuidReplacementMapKeys()
            {
                string[] starts = { string.Empty, "A ", "The " };
                string[] types = { "GUID", "Guid", "guid", "TypeGuid", "Guids" };
                string[] continuations = { "of", "for" };

                var results = new List<string>(3 + (starts.Length * types.Length * continuations.Length));

                foreach (var start in starts)
                {
                    foreach (var type in types)
                    {
                        var begin = start + type + " ";

                        foreach (var continuation in continuations)
                        {
                            results.Add(begin + continuation + " ");
                        }
                    }
                }

                results.Add("A unique identifier for "); // needed to avoid duplicate text in type guid comments
                results.Add("An unique identifier for "); // needed to avoid duplicate text in type guid comments
                results.Add("The unique identifier for "); // needed to avoid duplicate text in type guid comments

                return results;
            }

            private static IEnumerable<string> CreateCollectionReplacementMapKeys()
            {
                yield return "Containing ";
                yield return "Storing ";
                yield return "Stores ";
                yield return "Holding ";
                yield return "Holds ";
                yield return "This is ";

                var articles = new[] { string.Empty, "A ", "An ", "The ", "This ", "a ", "an ", "the ", "this " };
                var starts = new[]
                                 {
                                     "Array", "array",
                                     "Collection", "collection",
                                     "List", "list",
                                     "Dictionary", "dictionary",
                                     "Cache", "cache",
                                 };

                foreach (var article in articles)
                {
                    foreach (var start in starts)
                    {
                        var begin = article + start;

                        yield return begin + " of all ";
                        yield return begin + " of ";
                        yield return begin + " for all ";
                        yield return begin + " for ";

                        yield return begin + " that holds ";
                        yield return begin + " which holds ";
                        yield return begin + " holds ";
                        yield return begin + " holding ";
                        yield return begin + " is holding ";
                        yield return begin + " that is holding ";
                        yield return begin + " which is holding ";

                        yield return begin + " that contains ";
                        yield return begin + " which contains ";
                        yield return begin + " contains ";
                        yield return begin + " containing ";
                        yield return begin + " is containing ";
                        yield return begin + " that is containing ";
                        yield return begin + " which is containing ";

                        yield return begin + " that stores ";
                        yield return begin + " which stores ";
                        yield return begin + " stores ";
                        yield return begin + " storing ";
                        yield return begin + " is storing ";
                        yield return begin + " that is storing ";
                        yield return begin + " which is storing ";
                    }
                }
            }

            private static string[] CreateGetReplacementMapKeys() => new[]
                                                                         {
                                                                             "Gets ", "Get ",
                                                                             "Sets ", "Set ",
                                                                             "Gets or sets ", "Gets or Sets ", "Get or set ", "Get or Set ",
                                                                             "Return ", "Returns ",
                                                                             "/Return ", "/Returns ", // typos
                                                                         };

            private static string[] CreateOtherReplacementMapKeys() => new[] { "Defines ", "Specifies ", "Use this ", "This " };
        }
//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}
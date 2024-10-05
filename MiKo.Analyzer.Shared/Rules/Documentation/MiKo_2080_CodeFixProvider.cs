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

            var preparedComment = Comment(comment, MappedData.Value.TypeGuidReplacementMapKeys, MappedData.Value.TypeGuidReplacementMap);
            var preparedComment2 = Comment(preparedComment, MappedData.Value.ReplacementMapKeys, MappedData.Value.ReplacementMap);

            var fixedComment = CommentStartingWith(preparedComment2, phrase);

            return Comment(fixedComment, MappedData.Value.CleanupMapKeys, MappedData.Value.CleanupMap);
        }

//// ncrunch: rdi off

        private sealed class MapData
        {
            public MapData()
            {
                var keys = CreateReplacementMapKeys().OrderByDescending(_ => _.Length) // get longest items first as shorter items may be part of the longer ones and would cause problems
                                                     .ThenBy(_ => _)
                                                     .ToArray();

                ReplacementMap = keys.ToArray(_ => new Pair(_, string.Empty));

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

                CleanupMapKeys = new[] { " a the ", " an the ", " the the " };

                CleanupMap = CleanupMapKeys.ToArray(_ => new Pair(_, " the "));
            }

            public Pair[] ReplacementMap { get; }

            public string[] ReplacementMapKeys { get; }

            public string[] TypeGuidReplacementMapKeys { get; }

            public Pair[] TypeGuidReplacementMap { get; }

            public string[] CleanupMapKeys { get; }

            public Pair[] CleanupMap { get; }

            // ReSharper disable once ReturnTypeCanBeEnumerable.Local Violates CA1859
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

            private static IEnumerable<string> CreateBooleanReplacementMapKeys()
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

                string[] verbs = { "to indicate", "that indicates", "which indicates", "indicating", "to control", "that controls", "which controls", "controlling" };
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
                        results.Add(start + " indicate" + end);
                    }
                }

                results.Add("Indicating if ");
                results.Add("Indicating that ");
                results.Add("Indicating whether or not");
                results.Add("Indicating whether ");

                results.Add("Indicates if ");
                results.Add("Indicates that ");

                results.Add("Controlling if ");
                results.Add("Controlling that ");
                results.Add("Controlling whether or not");
                results.Add("Controlling whether ");

                results.Add("Controls if ");
                results.Add("Controls that ");
                results.Add("Controls whether or not ");
                results.Add("Controls whether ");

                return results;
            }

            private static IEnumerable<string> CreateGuidReplacementMapKeys()
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

            private static IEnumerable<string> CreateCollectionReplacementMapKeys() => new[] { "A list of ", "List of ", "A cache for ", "A Cache for ", "Cache for ", "Stores " };

            private static IEnumerable<string> CreateGetReplacementMapKeys() => new[] { "Gets ", "Get " };

            private static IEnumerable<string> CreateOtherReplacementMapKeys() => new[] { "Specifies the ", "Specifies an ", "Specifies a " };
        }

        //// ncrunch: rdi default
    }
}
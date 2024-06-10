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

        public override string FixableDiagnosticId => "MiKo_2080";

        protected override string Title => Resources.MiKo_2080_CodeFixTitle;

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
                ReplacementMap = CreateReplacementMapKeys().ToHashSet() // avoid duplicates
                                                           .OrderByDescending(_ => _.Length) // get longest items first as shorter items may be part of the longer ones and would cause problems
                                                           .ThenBy(_ => _)
                                                           .Select(_ => new KeyValuePair<string, string>(_, string.Empty))
                                                           .ToArray();

                ReplacementMapKeys = GetTermsForQuickLookup(ReplacementMap.Select(_ => _.Key));

                TypeGuidReplacementMapKeys = new[]
                                                 {
                                                     "A Type Guid for ",
                                                     "A Type GUID for ",
                                                     "A Type Guid of ",
                                                     "A Type GUID of ",
                                                     "A TypeGuid for ",
                                                     "A TypeGuid of ",
                                                     "The Type Guid for ",
                                                     "The Type GUID for ",
                                                     "The Type Guid of ",
                                                     "The Type GUID of ",
                                                     "The TypeGuid for ",
                                                     "The TypeGuid of ",
                                                     "Type Guid for ",
                                                     "Type Guid for ",
                                                     "Type GUID for ",
                                                     "Type GUID for ",
                                                     "Type Guid of ",
                                                     "Type GUID of ",
                                                     "TypeGuid for ",
                                                     "TypeGuid of ",
                                                     "TypeGuids for ", // typo
                                                     "TypeGuids of ", // typo
                                                 }.ToHashSet() // avoid duplicates
                                                  .ToArray(AscendingStringComparer.Default);

                TypeGuidReplacementMap = TypeGuidReplacementMapKeys.OrderByDescending(_ => _.Length)
                                                                   .ThenBy(_ => _)
                                                                   .Select(_ => new KeyValuePair<string, string>(_, "The unique identifier for the type of "))
                                                                   .ToArray();

                CleanupMapKeys = new[] { " a the ", " an the ", " the the " };

                CleanupMap = CleanupMapKeys.Select(_ => new KeyValuePair<string, string>(_, " the ")).ToArray();
            }

            public IReadOnlyCollection<KeyValuePair<string, string>> ReplacementMap { get; }

            public string[] ReplacementMapKeys { get; }

            public IReadOnlyCollection<string> TypeGuidReplacementMapKeys { get; }

            public IReadOnlyCollection<KeyValuePair<string, string>> TypeGuidReplacementMap { get; }

            public IReadOnlyCollection<string> CleanupMapKeys { get; }

            public IReadOnlyCollection<KeyValuePair<string, string>> CleanupMap { get; }

            private static IEnumerable<string> CreateReplacementMapKeys()
            {
                var keys = Enumerable.Empty<string>()
                                     .Concat(CreateBooleanReplacementMapKeys())
                                     .Concat(CreateGuidReplacementMapKeys())
                                     .Concat(CreateCollectionReplacementMapKeys())
                                     .Concat(CreateGetReplacementMapKeys())
                                     .Concat(CreateOtherReplacementMapKeys());

                foreach (var key in keys)
                {
                    yield return key;
                    yield return key.ToLowerCaseAt(0);
                }
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

                foreach (var start in starts)
                {
                    foreach (var verb in verbs)
                    {
                        foreach (var continuation in continuations)
                        {
                            yield return $"{start} {verb}{continuation} ";
                        }
                    }
                }

                foreach (var start in new[] { "To", "Shall", "Should", "Will", "Would" })
                {
                    foreach (var continuation in continuations)
                    {
                        yield return $"{start} control{continuation} ";
                        yield return $"{start} indicate{continuation} ";
                    }
                }

                yield return "Indicating if ";
                yield return "Indicating that ";
                yield return "Indicating whether or not";
                yield return "Indicating whether ";

                yield return "Indicates if ";
                yield return "Indicates that ";

                yield return "Controlling if ";
                yield return "Controlling that ";
                yield return "Controlling whether or not";
                yield return "Controlling whether ";

                yield return "Controls if ";
                yield return "Controls that ";
                yield return "Controls whether or not ";
                yield return "Controls whether ";
            }

            private static IEnumerable<string> CreateGuidReplacementMapKeys()
            {
                string[] starts = { string.Empty, "A ", "The " };
                string[] types = { "GUID", "Guid", "guid", "TypeGuid", "Guids" };
                string[] continuations = { "of", "for" };

                foreach (var start in starts)
                {
                    foreach (var type in types)
                    {
                        foreach (var continuation in continuations)
                        {
                            yield return $"{start}{type} {continuation} ";
                        }
                    }
                }

                yield return "A unique identifier for "; // needed to avoid duplicate text in type guid comments
                yield return "An unique identifier for "; // needed to avoid duplicate text in type guid comments
                yield return "The unique identifier for "; // needed to avoid duplicate text in type guid comments
            }

            private static IEnumerable<string> CreateCollectionReplacementMapKeys()
            {
                yield return "A list of ";
                yield return "List of ";
                yield return "A cache for ";
                yield return "A Cache for ";
                yield return "Cache for ";
                yield return "Stores ";
            }

            private static IEnumerable<string> CreateGetReplacementMapKeys()
            {
                yield return "Gets ";
                yield return "Get ";
            }

            private static IEnumerable<string> CreateOtherReplacementMapKeys()
            {
                yield return "Specifies the ";
                yield return "Specifies an ";
                yield return "Specifies a ";
            }
        }

        //// ncrunch: rdi default
    }
}
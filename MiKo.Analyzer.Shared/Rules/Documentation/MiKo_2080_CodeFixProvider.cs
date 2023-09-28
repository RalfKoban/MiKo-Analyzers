using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2080_CodeFixProvider)), Shared]
    public sealed class MiKo_2080_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly IReadOnlyCollection<string> ReplacementMapKeys = CreateReplacementMapKeys().ToHashSet()
                                                                                                           .OrderBy(_ => _.Length)
                                                                                                           .ThenBy(_ => _)
                                                                                                           .ToArray();

        private static readonly IReadOnlyCollection<KeyValuePair<string, string>> ReplacementMap = ReplacementMapKeys.OrderByDescending(_ => _.Length)
                                                                                                                     .ThenBy(_ => _)
                                                                                                                     .Select(_ => new KeyValuePair<string, string>(_, string.Empty))
                                                                                                                     .ToArray();

        private static readonly IReadOnlyCollection<string> TypeGuidReplacementMapKeys = new[]
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
                                                                                             }.Distinct()
                                                                                              .OrderBy(_ => _.Length)
                                                                                              .ThenBy(_ => _)
                                                                                              .ToArray();

        private static readonly IReadOnlyCollection<KeyValuePair<string, string>> TypeGuidReplacementMap = TypeGuidReplacementMapKeys.OrderByDescending(_ => _.Length)
                                                                                                                                     .ThenBy(_ => _)
                                                                                                                                     .Select(_ => new KeyValuePair<string, string>(_, "The unique identifier for the type of "))
                                                                                                                                     .ToArray();

        public override string FixableDiagnosticId => MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2080_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;
            var fieldDeclaration = comment.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var field = fieldDeclaration?.Declaration.Variables.First();

            var symbol = (IFieldSymbol)GetSymbol(document, field);
            var phrase = MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.GetStartingPhrase(symbol);

            var preparedComment = Comment(comment, TypeGuidReplacementMapKeys, TypeGuidReplacementMap);
            var preparedComment2 = Comment(preparedComment, ReplacementMapKeys, ReplacementMap);

            return CommentStartingWith(preparedComment2, phrase);
        }

        private static IEnumerable<string> CreateReplacementMapKeys() => Enumerable.Empty<string>()
                                                                                   .Concat(CreateBooleanReplacementMapKeys())
                                                                                   .Concat(CreateGuidReplacementMapKeys())
                                                                                   .Concat(CreateCollectionReplacementMapKeys())
                                                                                   .Concat(CreateGetReplacementMapKeys())
                                                                                   .Concat(CreateOtherReplacementMapKeys());

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
                                 "boolean value",
                                 "Boolean value",
                                 "bool value",
                                 "Bool value",
                                 "boolean",
                                 "Boolean",
                                 "bool",
                                 "Bool",
                                 "flag",
                                 "Flag",
                                 "value",
                                 "Value",
                             };

            // build up the "contains xyz" terms
            foreach (var s in booleans)
            {
                starts.Add("Contains " + s.ToLowerCaseAt(0));
                starts.Add("contains " + s.ToLowerCaseAt(0));
            }

            string[] verbs = { "to indicate", "that indicates", "which indicates", "indicating" };
            string[] continuations = { "if", "whether", "that" };

            foreach (var start in starts)
            {
                foreach (var verb in verbs)
                {
                    foreach (var continuation in continuations)
                    {
                        yield return $"{start} {verb} {continuation} ";
                    }
                }
            }

            foreach (var start in new[] { "To", "Shall", "Should", "Will", "Would" })
            {
                foreach (var continuation in continuations)
                {
                    yield return $"{start} indicate {continuation} ";
                }

                yield return $"{start} indicate ";
            }

            yield return "Indicating if ";
            yield return "Indicating that ";
            yield return "Indicating whether ";
            yield return "Indicates if ";
            yield return "Indicates that ";
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
        }

        private static IEnumerable<string> CreateOtherReplacementMapKeys()
        {
            yield return "A ";
            yield return "An ";
            yield return "The ";
        }
    }
}
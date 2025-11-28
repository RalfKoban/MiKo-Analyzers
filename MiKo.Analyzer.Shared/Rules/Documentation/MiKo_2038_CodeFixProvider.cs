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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2038_CodeFixProvider)), Shared]
    public sealed class MiKo_2038_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly string[] CommandStartingPhrases = CreateCommandStartingPhrases().Except(Constants.Comments.CommandSummaryStartingPhrase).OrderDescendingByLengthAndText(_ => _);

        private static readonly Pair[] CommandReplacementMap = CreateCommandReplacementMapEntries(CommandStartingPhrases).OrderDescendingByLengthAndText(_ => _.Key);

        private static readonly string[] CommandReplacementMapKeys = CommandReplacementMap.ToArray(_ => _.Key);

        private static readonly Pair[] CleanupMap =
                                                    {
                                                        new Pair(" commands for ", " "),
                                                        new Pair(" command for ", " "),
                                                        new Pair(" can in ", " can be used in "),
                                                        new Pair(" can with ", " can be used with "),
                                                    };

        private static readonly string[] CleanupMapKeys = CommandReplacementMap.ToArray(_ => _.Key);

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2038";

        internal static bool CanFix(in ReadOnlySpan<char> text) => text.StartsWithAny(CommandStartingPhrases, StringComparison.OrdinalIgnoreCase);

        internal static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is XmlElementSyntax element)
            {
                var preparedComment = Comment(element, CommandReplacementMapKeys, CommandReplacementMap, FirstWordAdjustment.StartLowerCase);
                var updatedComment = CommentStartingWith(preparedComment, Constants.Comments.CommandSummaryStartingPhrase, FirstWordAdjustment.StartLowerCase | FirstWordAdjustment.MakeInfinite);
                var cleanedComment = Comment(updatedComment, CleanupMapKeys, CleanupMap);

                return cleanedComment;
            }

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

//// ncrunch: rdi off

        private static HashSet<string> CreateCommandStartingPhrases()
        {
            var result = new HashSet<string>();

            result.AddRange(CreateCommandStartingPhrasesLocal());
            result.AddRange(CreateCommandStartingPhrasesLocal().Select(_ => _.ToLowerCaseAt(0)));

            return result;

            IEnumerable<string> CreateCommandStartingPhrasesLocal()
            {
                var adverbs = new[] { string.Empty, "standard ", "toggle ", "backstage ", "back-stage ", "sync ", "async " };

                foreach (var adverb in adverbs)
                {
                    yield return "A " + adverb + "command ";
                    yield return "An " + adverb + "command ";
                    yield return "The " + adverb + "command ";
                    yield return "This " + adverb + "command ";

                    yield return "Base class for " + adverb + "commands ";
                    yield return "Base class for " + adverb;
                    yield return "Base class for ";

                    yield return "Base class for the " + adverb + "commands ";
                    yield return "Base class for the " + adverb;
                    yield return "Base class for the ";
                }

                yield return "Command ";
                yield return "A class ";
                yield return "The class ";
                yield return "This class ";

                var articles = new[] { "a ", "an ", "the " };

                foreach (var adverb in adverbs)
                {
                    yield return "A interface for " + adverb + "commands ";
                    yield return "An interface for " + adverb + "commands ";
                    yield return "The interface for " + adverb + "commands ";
                    yield return "Interface for " + adverb + "commands ";
                    yield return "Interface for " + adverb + "command ";

                    yield return "Represents " + adverb + "command ";

                    foreach (var article in articles)
                    {
                        yield return "A interface for " + article + adverb + "command ";
                        yield return "An interface for " + article + adverb + "command ";
                        yield return "The interface for " + article + adverb + "command ";
                        yield return "Interface for " + article + adverb + "command ";

                        yield return "Represents " + article + adverb + "command ";
                    }
                }

                yield return "Provides functionality ";
                yield return "Provides a functionality ";
                yield return "Provides the functionality ";
            }
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static List<Pair> CreateCommandReplacementMapEntries(string[] commandStartingPhrases)
        {
            var middleParts = new[]
                                  {
                                      "that can",
                                      "that is used to",
                                      "that is used for",
                                      "that is capable to",
                                      "that is capable for",
                                      "that offers to",
                                      "that tries to",
                                      "that will",
                                      "that",
                                      "which can",
                                      "which is capable to",
                                      "which is capable for",
                                      "which is used to",
                                      "which is used for",
                                      "which offers to",
                                      "which tries to",
                                      "which will",
                                      "which",
                                      "will",
                                      "to",
                                      "for",
                                      "can be used to",
                                      "can be used for",
                                      "is used to",
                                      "is used for",
                                      "used to",
                                      "used for",
                                      "is capable to",
                                      "is capable for",
                                      "capable to",
                                      "capable for",
                                      "offers to",
                                      "tries to",
                                  };

            var results = new List<Pair>();

            foreach (var phrase in commandStartingPhrases)
            {
                var start = phrase.AsSpan().Trim().ToString();

                foreach (var middle in middleParts)
                {
                    results.Add(new Pair(string.Concat(start, " ", middle, " ")));
                }

                results.Add(new Pair(string.Concat(start, " ")));
            }

            results.Add(new Pair(string.Concat("Offers to ")));
            results.Add(new Pair(string.Concat("Tries to ")));

            return results;
        }
    }

    //// ncrunch: rdi default
}
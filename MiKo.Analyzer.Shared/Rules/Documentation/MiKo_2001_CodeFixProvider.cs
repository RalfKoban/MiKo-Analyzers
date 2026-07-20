using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2001_CodeFixProvider)), Shared]
    public sealed class MiKo_2001_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly ReplacementMap ReplacementMap = new ReplacementMap("MiKo_2001_Replace", CreatePhrases().OrderDescendingByLengthAndText(_ => _.Key), _ => GetTermsForQuickLookup(_));
        private static readonly ReplacementMap CleanUpMap = new ReplacementMap("MiKo_2001_Cleanup", CreateCleanupPhrases().OrderDescendingByLengthAndText(_ => _.Key), _ => GetTermsForQuickLookup(_));

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2001";

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((XmlElementSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax GetUpdatedSyntax(XmlElementSyntax syntax)
        {
            var preparedComment = Comment(syntax, ReplacementMap, FirstWordAdjustment.StartLowerCase);
            var fixedComment = CommentStartingWith(preparedComment, Constants.Comments.EventSummaryStartingPhrase);
            var cleanedComment = Comment(fixedComment, CleanUpMap);

            return cleanedComment;
        }

//// ncrunch: rdi off

        private static IEnumerable<Pair> CreatePhrases()
        {
            var starts = new[] { "Event", "This event", "The event", "An event", "A event" };
            var adverbs = new[]
                              {
                                  string.Empty,
                                  "is ", "that's ", "that is ", "which is ",
                                  "can be ", "that can be ", "which can be ",
                                  "could be ", "that could be ", "which could be ",
                                  "shall be ", "that shall be ", "which shall be ",
                                  "should be ", "that should be ", "which should be ",
                                  "will be ", "that will be ", "which will be ",
                                  "would be ", "that would be ", "which would be ",
                              };
            var verbs = new[] { "fired", "raised", "caused", "triggerd", "triggered", "occurred", "occured", "notifying", "notfying", "informing", "invoked", "invoking", "sent", "sending" };

            var results = new HashSet<string>();

            foreach (var verb in verbs)
            {
                results.Add(string.Concat(verb.ToUpperCaseAt(0), Constants.SingleSpace));

                foreach (var adverb in adverbs)
                {
                    var end = string.Concat(adverb, verb);

                    results.Add(end);
                    results.Add(end.ToUpperCaseAt(0));

                    foreach (var start in starts)
                    {
                        results.Add(string.Concat(start, Constants.SingleSpace, end, Constants.SingleSpace));
                    }
                }
            }

            var midTerms = new[]
                               {
                                   "to",
                                   "can", "that can", "which can",
                                   "could", "that could", "which could",
                                   "shall", "that shall", "which shall",
                                   "should", "that should", "which should",
                                   "will", "that will", "which will",
                                   "would", "that would", "which would",
                               };
            var verbsInfinite = new[] { "fire", "raise", "cause", "trigger", "occur", "notify", "notfy", "inform", "invoke", "send" };

            foreach (var start in starts)
            {
                foreach (var midTerm in midTerms)
                {
                    var begin = string.Concat(start, Constants.SingleSpace, midTerm, Constants.SingleSpace);

                    foreach (var verb in verbsInfinite)
                    {
                        results.Add(string.Concat(begin, verb, Constants.SingleSpace));
                    }
                }
            }

            var verbsPresent = new[] { "fires", "raises", "causes", "triggers", "occurs", "notifies", "notfies", "informs", "invokes", "sends" };

            foreach (var start in starts)
            {
                foreach (var verb in verbsPresent)
                {
                    results.Add(string.Concat(start, Constants.SingleSpace, verb, Constants.SingleSpace));
                }
            }

            results.Add("Indicates");

            return results.Select(_ => new Pair(_));
        }

        private static IEnumerable<Pair> CreateCleanupPhrases()
        {
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "fire ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "fired ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "fires ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "firing ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "inform ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "informs ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "informing ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notfies ", Constants.Comments.EventSummaryStartingPhrase); // typo
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notfying ", Constants.Comments.EventSummaryStartingPhrase); // typo
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notfy ", Constants.Comments.EventSummaryStartingPhrase); // typo
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notify ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notifies ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notifying ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "raise ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "raised ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "raises ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "raising ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "trigger ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "triggerd ", Constants.Comments.EventSummaryStartingPhrase); // typo
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "triggered ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "triggers ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "triggering ", Constants.Comments.EventSummaryStartingPhrase);

            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "invoked ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "send ", Constants.Comments.EventSummaryStartingPhrase);

            // special cases
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "occur ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "occurs ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "occured ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "occurred ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "occuring ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "occurring ", Constants.Comments.EventSummaryStartingPhrase);

            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "about ", Constants.Comments.EventSummaryStartingPhrase + "when ");
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "that ", Constants.Comments.EventSummaryStartingPhrase + "when ");
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "if ", Constants.Comments.EventSummaryStartingPhrase + "when ");
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notify if ", Constants.Comments.EventSummaryStartingPhrase + "when ");
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notfy if ", Constants.Comments.EventSummaryStartingPhrase + "when "); // typo

            var starts = new[] { "Event", "This event", "The event", "An event", "A event" };

            foreach (var start in starts.Select(_ => Constants.Comments.EventSummaryStartingPhrase + _.ToLowerCaseAt(0)))
            {
               yield return new Pair(start + " that allows to perform an action ", Constants.Comments.EventSummaryStartingPhrase + "when ");
               yield return new Pair(start + " that allows us to perform an action ", Constants.Comments.EventSummaryStartingPhrase + "when ");
               yield return new Pair(start + " which allows to perform an action ", Constants.Comments.EventSummaryStartingPhrase + "when ");
               yield return new Pair(start + " which allows us to perform an action ", Constants.Comments.EventSummaryStartingPhrase + "when ");
               yield return new Pair(start + " allowing to perform an action ", Constants.Comments.EventSummaryStartingPhrase + "when ");
               yield return new Pair(start + " allowing us to perform an action ", Constants.Comments.EventSummaryStartingPhrase + "when ");

               yield return new Pair(start + " that informs about ", Constants.Comments.EventSummaryStartingPhrase + "when ");
               yield return new Pair(start + " which informs about ", Constants.Comments.EventSummaryStartingPhrase + "when ");

               yield return new Pair(start + " that notifies about ", Constants.Comments.EventSummaryStartingPhrase + "when ");
               yield return new Pair(start + " which notifies about ", Constants.Comments.EventSummaryStartingPhrase + "when ");
               yield return new Pair(start + " that notfies about ", Constants.Comments.EventSummaryStartingPhrase + "when "); // typo
               yield return new Pair(start + " which notfies about ", Constants.Comments.EventSummaryStartingPhrase + "when "); // typo
            }

            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notification by a DTM that ", Constants.Comments.EventSummaryStartingPhrase + "when ");
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notification  by a DTM that ", Constants.Comments.EventSummaryStartingPhrase + "when ");
            yield return new Pair(Constants.Comments.EventSummaryStartingPhrase + "notification sent by a DTM that ", Constants.Comments.EventSummaryStartingPhrase + "when ");
        }
//// ncrunch: rdi default
    }
}
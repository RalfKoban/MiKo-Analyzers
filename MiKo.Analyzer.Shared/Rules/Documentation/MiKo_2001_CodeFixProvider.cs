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

        private static readonly Pair[] ReplacementMap = CreatePhrases().Select(_ => new Pair(_)).OrderDescendingByLengthAndText(_ => _.Key);

        private static readonly string[] ReplacementMapKeys = GetTermsForQuickLookup(ReplacementMap);

        private static readonly Pair[] CleanUpMap =
                                                    {
                                                        new Pair("Occurs fire ", "Occurs "),
                                                        new Pair("Occurs fired ", "Occurs "),
                                                        new Pair("Occurs fires ", "Occurs "),
                                                        new Pair("Occurs firing ", "Occurs "),
                                                        new Pair("Occurs raise ", "Occurs "),
                                                        new Pair("Occurs raised ", "Occurs "),
                                                        new Pair("Occurs raises ", "Occurs "),
                                                        new Pair("Occurs raising ", "Occurs "),
                                                        new Pair("Occurs trigger ", "Occurs "),
                                                        new Pair("Occurs triggered ", "Occurs "),
                                                        new Pair("Occurs triggers ", "Occurs "),
                                                        new Pair("Occurs triggering ", "Occurs "),

                                                        new Pair("Occurs invoked ", "Occurs "),

                                                        // special cases
                                                        new Pair("Occurs occur ", "Occurs "),
                                                        new Pair("Occurs occurs ", "Occurs "),
                                                        new Pair("Occurs occured ", "Occurs "),
                                                        new Pair("Occurs occurred ", "Occurs "),
                                                        new Pair("Occurs occuring ", "Occurs "),
                                                        new Pair("Occurs occurring ", "Occurs "),
                                                        new Pair("Occurs that ", "Occurs when "),
                                                        new Pair("Occurs if ", "Occurs when "),
                                                    };

        private static readonly string[] CleanUpPhrases = CleanUpMap.ToArray(_ => _.Key);

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2001";

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((XmlElementSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax GetUpdatedSyntax(XmlElementSyntax syntax)
        {
            var preparedComment = Comment(syntax, ReplacementMapKeys, ReplacementMap, FirstWordAdjustment.StartLowerCase);
            var fixedComment = CommentStartingWith(preparedComment, Constants.Comments.EventSummaryStartingPhrase);
            var cleanedComment = Comment(fixedComment, CleanUpPhrases, CleanUpMap);

            return cleanedComment;
        }

        //// ncrunch: rdi off

        private static HashSet<string> CreatePhrases()
        {
            var starts = new[] { "Event", "This event", "The event", "An event", "A event" };
            var adverbs = new[]
                              {
                                  string.Empty,
                                  "is ", "that is ", "which is ",
                                  "can be ", "that can be ", "which can be ",
                                  "could be ", "that could be ", "which could be ",
                                  "shall be ", "that shall be ", "which shall be ",
                                  "should be ", "that should be ", "which should be ",
                                  "will be ", "that will be ", "which will be ",
                                  "would be ", "that would be ", "which would be ",
                              };
            var verbs = new[] { "fired", "raised", "caused", "triggered", "occurred", "occured" };

            var results = new HashSet<string>();

            foreach (var verb in verbs)
            {
                results.Add(string.Concat(verb.ToUpperCaseAt(0), " "));

                foreach (var adverb in adverbs)
                {
                    var end = string.Concat(adverb, verb);

                    results.Add(end.ToUpperCaseAt(0));

                    foreach (var start in starts)
                    {
                        results.Add(string.Concat(start, " ", end, " "));
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
            var verbsInfinite = new[] { "fire", "raise", "cause", "trigger", "occur" };

            foreach (var start in starts)
            {
                foreach (var midTerm in midTerms)
                {
                    var begin = string.Concat(start, " ", midTerm, " ");

                    foreach (var verb in verbsInfinite)
                    {
                        results.Add(string.Concat(begin, verb, " "));
                    }
                }
            }

            var verbsPresent = new[] { "fires", "raises", "causes", "triggers", "occurs" };

            foreach (var start in starts)
            {
                foreach (var verb in verbsPresent)
                {
                    results.Add(string.Concat(start, " ", verb, " "));
                }
            }

            results.Add("Indicates");

            return results;
        }
//// ncrunch: rdi default
    }
}
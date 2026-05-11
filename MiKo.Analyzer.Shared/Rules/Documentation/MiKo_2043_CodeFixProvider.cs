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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2043_CodeFixProvider)), Shared]
    public sealed class MiKo_2043_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly Pair[] PreparationMap = CreatePreparationMap().OrderDescendingByLengthAndText(_ => _.Key);

        private static readonly string[] PreparationMapPhrases = PreparationMap.ToArray(_ => _.Key);

        private static readonly Pair[] CleanUpMap = CreateCleanUpMap().Select(_ => new Pair(Constants.Comments.DelegateSummaryStartingPhrase + _.Key, Constants.Comments.DelegateSummaryStartingPhrase + _.Value))
                                                                      .OrderDescendingByLengthAndText(_ => _.Key);

        private static readonly string[] CleanUpPhrases = CleanUpMap.ToArray(_ => _.Key);

        public override string FixableDiagnosticId => "MiKo_2043";

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((XmlElementSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax GetUpdatedSyntax(XmlElementSyntax comment)
        {
            if (comment.GetEnclosingSyntaxNode() is DelegateDeclarationSyntax d)
            {
                // fix if delegate only contains it's name + " delegate" (e.g. "MyDelegate delegate") and nothing else
                var lookupTerm = d.GetName() + " delegate";

                var preparedComment1 = Comment(comment, new[] { lookupTerm }, new[] { new Pair(lookupTerm, Constants.Comments.DelegateSummaryStartingPhrase + Constants.TODO) });

                if (ReferenceEquals(preparedComment1, comment))
                {
                    preparedComment1 = Comment(comment, PreparationMapPhrases, PreparationMap, FirstWordAdjustment.StartLowerCase | FirstWordAdjustment.MakeThirdPersonSingular | FirstWordAdjustment.KeepSingleLeadingSpace);
                }
                else
                {
                    return preparedComment1;
                }

                var preparedComment2 = CommentStartingWith(preparedComment1, Constants.Comments.DelegateSummaryStartingPhrase);
                var fixedComment = Comment(preparedComment2, CleanUpPhrases, CleanUpMap);

                return fixedComment;
            }

            return comment;
        }

        private static IEnumerable<Pair> CreatePreparationMap()
        {
            yield return new Pair("Delegate used to", string.Empty);
            yield return new Pair("Delegate used for", string.Empty);
        }

        private static IEnumerable<Pair> CreateCleanUpMap()
        {
            var callbackPrefixes = new[] { "callback", "call-back", "call-Back", "callbacks", "call-backs", "call-Backs" };
            var notifyVariants = new[] { " to notify ", " to notfy " /* typo */ };

            foreach (var prefix in callbackPrefixes)
            {
                foreach (var notify in notifyVariants)
                {
                    yield return new Pair(prefix + notify, "notifies ");
                }
            }

            var verbs = new[] { "references", "represents" };
            var articles = new[] { "a", "the" };
            var actions = new[] { "to call", "to invoke", "to be called", "to be invoked" };

            foreach (var verb in verbs)
            {
                foreach (var article in articles)
                {
                    var start = verb + " " + article + " method ";

                    foreach (var action in actions)
                    {
                        foreach (var notify in notifyVariants)
                        {
                            yield return new Pair(start + action + notify, "notifies ");
                        }
                    }
                }
            }

            yield return new Pair("represents the getter of the ", "gets the ");
            yield return new Pair("represents the getter of ", "gets the ");
            yield return new Pair("represents the setter of the ", "sets the ");
            yield return new Pair("represents the setter of ", "sets the ");
            yield return new Pair("represents the ", "handles a call to the ");

            // events
            var eventHandlings = new[] { "that handles ", "which handles ", "handling " };

            foreach (var article in articles)
            {
                var start = "represents " + article + " method ";

                foreach (var handling in eventHandlings)
                {
                    yield return new Pair(start + handling, "handles ");
                }
            }

            yield return new Pair("event Handler for ", "handles ");
            yield return new Pair("event-Handler for ", "handles ");
            yield return new Pair("event-handler for ", "handles ");
            yield return new Pair("eventhandler for ", "handles ");
            yield return new Pair("eventHandler for ", "handles ");

            yield return new Pair("event fired by", "is called by");
            yield return new Pair("event raised by", "is called by");
            yield return new Pair("event triggered by", "is called by");

            yield return new Pair("builder method that ", string.Empty);
            yield return new Pair("builder method which ", string.Empty);

            yield return new Pair("delegate for the callback by which the application gives", "provides");
            yield return new Pair("delegate for the callback by which the application informs", "notifies");
            yield return new Pair("delegate for the callback by which the application tells", "notifies");
            yield return new Pair("delegate for", "handles callbacks for");
            yield return new Pair("delegate in which the application ", string.Empty);
            yield return new Pair("delegate to handle ", "handles ");

            yield return new Pair(Constants.Comments.DelegateSummaryStartingPhrase.ToLowerCaseAt(0), string.Empty);

            yield return new Pair("an application-defined callback function used with the", "handles callbacks for");
            yield return new Pair("application-defined callback function used with the", "handles callbacks for");

            yield return new Pair("describes a function that is called", "is called");
            yield return new Pair("describes a function which is called", "is called");
        }
    }
}
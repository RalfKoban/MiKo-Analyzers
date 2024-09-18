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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2001_CodeFixProvider)), Shared]
    public sealed class MiKo_2001_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private const string SpecialTerm = "Occurs that ";

        private static readonly string[] SpecialTerms = { SpecialTerm };

        private static readonly Pair[] SpecialTermReplacementMap = { new Pair(SpecialTerm, "Occurs when ") };

        private static readonly Pair[] ReplacementMap = CreatePhrases().Select(_ => new Pair(_, string.Empty))
                                                                       .Append(new Pair("Invoked if ", "when "))
                                                                       .Append(new Pair("Invoked when ", "when "))
                                                                       .OrderByDescending(_ => _.Key.Length)
                                                                       .ThenBy(_ => _.Key)
                                                                       .ToArray();

        private static readonly string[] ReplacementMapKeys = ReplacementMap.ToArray(_ => _.Key);

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2001";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var preparedComment = PrepareComment((XmlElementSyntax)syntax);

            var fixedComment = CommentStartingWith(preparedComment, Constants.Comments.EventSummaryStartingPhrase);

            var text = fixedComment.Content[0].WithoutXmlCommentExterior();

            if (text.StartsWith(SpecialTerm, StringComparison.Ordinal))
            {
                return Comment(fixedComment, SpecialTerms, SpecialTermReplacementMap);
            }

            return fixedComment;
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMapKeys, ReplacementMap, FirstWordHandling.MakeLowerCase);

//// ncrunch: rdi off

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local Violates CA1859
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
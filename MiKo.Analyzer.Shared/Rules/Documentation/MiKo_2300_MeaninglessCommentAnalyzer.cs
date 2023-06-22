using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2300_MeaninglessCommentAnalyzer : SingleLineCommentAnalyzer
    {
        public const string Id = "MiKo_2300";

        private static readonly string[] MeaninglessPhrases =
                                                              {
                                                                  "add ",
                                                                  "adds ",
                                                                  "build ",
                                                                  "builds ",
                                                                  "calculate ",
                                                                  "calculates ",
                                                                  "call ",
                                                                  "calls ",
                                                                  "check ",
                                                                  "checks ",
                                                                  "close ",
                                                                  "closes ",
                                                                  "compare ",
                                                                  "compares ",
                                                                  "convert ",
                                                                  "converts ",
                                                                  "count ",
                                                                  "counts ",
                                                                  "create ",
                                                                  "creates ",
                                                                  "decr.",
                                                                  "decrease ",
                                                                  "decreases ",
                                                                  "decrement ",
                                                                  "decrements ",
                                                                  "determine ",
                                                                  "determines ",
                                                                  "evaluate event arg", // no space at the end to allow combinations of the word
                                                                  "get ",
                                                                  "gets ",
                                                                  "has ",
                                                                  "if ",
                                                                  "incr.",
                                                                  "increase ",
                                                                  "increases ",
                                                                  "increment ",
                                                                  "increments ",
                                                                  "initialize", // no space at the end to allow combinations of the word
                                                                  "invoke", // no space at the end to allow combinations of the word
                                                                  "is ",
                                                                  "iterate", // no space at the end to allow combinations of the word
                                                                  "load",
                                                                  "open ",
                                                                  "opens ",
                                                                  "raise ",
                                                                  "raises ",
                                                                  "remove ",
                                                                  "removes ",
                                                                  "retrieve ",
                                                                  "retrieves ",
                                                                  "return", // no space at the end to allow combinations of the word
                                                                  "save", // no space at the end to allow combinations of the word
                                                                  "set ",
                                                                  "sets ",
                                                                  "start ",
                                                                  "starts ",
                                                                  "stop ",
                                                                  "stops ",
                                                                  "use", // no space at the end to allow combinations of the word
                                                              };

        private static readonly string[] AllowedMarkers =
                                                          {
                                                              "0x",
                                                              "://",
                                                              "@formatter:off",
                                                              "@formatter:on",
                                                              "blank",
                                                              "checked by",
                                                              "ignore",
                                                              "intentionally",
                                                              "No-Op",
                                                              "not needed",
                                                              "nothing",
                                                              "special handling",
                                                              "initializer",
                                                          };

        public MiKo_2300_MeaninglessCommentAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(ReadOnlySpan<char> comment, SemanticModel semanticModel)
        {
            if (comment.StartsWith("//", StringComparison.OrdinalIgnoreCase))
            {
                return false; // ignore all comments that have the "double comment" marker
            }

            if (comment.IsNullOrWhiteSpace())
            {
                return false; // ignore all empty comments
            }

            if (comment.StartsWithAny(MeaninglessPhrases)
             && comment.StartsWithAny(AllowedMarkers) is false)
            {
                return true;
            }

            if (comment.Contains("->"))
            {
                return true;
            }

            var spaces = 0;

            foreach (var c in comment)
            {
                if (c.IsWhiteSpace())
                {
                    spaces++;
                }
            }

            if (spaces < 3)
            {
                // 3 or less words
                if (comment.ContainsAny(AllowedMarkers))
                {
                    return false;
                }

                if (MiKo_2301_TestArrangeActAssertCommentAnalyzer.CommentContainsArrangeActAssert(comment))
                {
                    return false; // already reported by the other analyzer
                }

                return true;
            }

            // TODO: add other stuff
            return false;
        }
    }
}
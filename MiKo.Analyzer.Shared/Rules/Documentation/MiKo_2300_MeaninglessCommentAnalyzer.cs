﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2300_MeaninglessCommentAnalyzer : SingleLineCommentAnalyzer
    {
        public const string Id = "MiKo_2300";

        private static readonly string[] ReasoningMarkers = { "because", " as ", "reason" };

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
                                                              "a mock",
                                                              "blank",
                                                              "checked by",
                                                              "ignore",
                                                              "intentionally",
                                                              "mocked",
                                                              "No-Op",
                                                              "not needed",
                                                              "nothing",
                                                              "special handling",
                                                              "initializer",
                                                              "typo by intent",
                                                              "ncrunch:",
                                                          };

        public MiKo_2300_MeaninglessCommentAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel)
        {
            // perform 'ToString' here to avoid multiple calls later on
            var commentString = comment.ToString();

            return CommentHasIssue(commentString) && commentString.ContainsAny(ReasoningMarkers) is false;
        }

        private static bool CommentHasIssue(string comment)
        {
            if (comment.StartsWith(Constants.Comments.CommentExterior, StringComparison.Ordinal))
            {
                return false; // ignore all comments that have the "double comment" marker
            }

            if (comment.IsNullOrWhiteSpace())
            {
                return false; // ignore all empty comments
            }

            if (MiKo_2311_CommentIsSeparatorAnalyzer.CommentContainsSeparator(comment))
            {
                return false; // already reported by the other analyzer
            }

            var commentSpan = comment.AsSpan();

            if (commentSpan.StartsWithAny(MeaninglessPhrases) && commentSpan.StartsWithAny(AllowedMarkers) is false)
            {
                return true;
            }

            if (commentSpan.Contains("->"))
            {
                return true;
            }

            var spaces = 0;

            foreach (var c in commentSpan)
            {
                if (c.IsWhiteSpace())
                {
                    spaces++;
                }
            }

            if (spaces < 3)
            {
                // 3 or fewer words
                if (comment.ContainsAny(AllowedMarkers))
                {
                    return false;
                }

                if (MiKo_2301_TestArrangeActAssertCommentAnalyzer.CommentContainsArrangeActAssert(commentSpan))
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
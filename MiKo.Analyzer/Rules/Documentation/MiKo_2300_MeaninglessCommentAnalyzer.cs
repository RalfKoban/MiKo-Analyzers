using System;
using System.Linq;

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
                "calculate ",
                "call ",
                "check ",
                "close ",
                "compare ",
                "convert ",
                "count ",
                "create ",
                "decr.",
                "decrease ",
                "decrement ",
                "determine ",
                "determines ",
                "evaluate event arg",
                "get ",
                "has ",
                "if ",
                "incr.",
                "increase ",
                "increment ",
                "initialize ",
                "invoke" ,
                "is " ,
                "open ",
                "raise ",
                "remove ",
                "retrieve ",
                "return ",
                "set ",
                "start ",
                "stop ",
            };

        private static readonly string[] AllowedMarkers =
            {
                "nothing",
                "ignore",
                "0x",
                "://",
            };

        public MiKo_2300_MeaninglessCommentAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment)
        {
            if (comment.StartsWith("//", StringComparison.OrdinalIgnoreCase))
                return false; // ignore all comments that have the //// marker

            if (comment.IsNullOrWhiteSpace())
                return false; // ignore all empty comments

            if (comment.StartsWithAny(MeaninglessPhrases))
                return true;

            if (comment.Contains("->"))
                return true;

            var spaces = comment.Count(_ => _.IsWhiteSpace());
            if (spaces < 3)
            {
                // 3 or less words
                if (comment.ContainsAny(AllowedMarkers))
                    return false;

                if (MiKo_2301_TestArrangeActAssertCommentAnalyzer.CommentContainsArrangeActAssert(comment))
                    return false; // gets already reported by the other analyzer

                return true;
            }

            // TODO: add other stuff
            return false;
        }
    }
}
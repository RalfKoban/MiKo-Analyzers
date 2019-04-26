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

        private const string WebsiteMarker = @"://";

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
                "incr.",
                "increase ",
                "increment ",
                "initialize ",
                "invoke" ,
                "open ",
                "raise ",
                "remove ",
                "retrieve ",
                "return ",
                "set ",
                "start ",
                "stop ",
            };

        private static readonly string[] AllowedPhrases =
            {
                "nothing to do",
                "ignore ",
                "0x",
            };

        public MiKo_2300_MeaninglessCommentAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment)
        {
            const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

            if (comment.StartsWith("//", Comparison))
                return false; // ignore all comments that have the //// marker

            if (comment.IsNullOrWhiteSpace())
                return false; // ignore all empty comments

            if (comment.StartsWithAny(MeaninglessPhrases, Comparison))
                return true;

            if (comment.StartsWith("is ", Comparison) && comment.EndsWith("?", Comparison))
                return true;

            if (comment.Contains("->"))
                return true;

            var spaces = comment.Count(_ => _.IsWhiteSpace());
            if (spaces < 3)
            {
                // 3 or less words
                if (comment.Contains(WebsiteMarker, Comparison))
                    return false;

                if (comment.ContainsAny(AllowedPhrases, Comparison))
                    return false;

                return true;
            }

            // TODO: add other stuff
            return false;
        }
    }
}
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

        private static readonly string[] StartingPhrases =
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
                "decrease ",
                "determine ",
                "determines ",
                "evaluate event arg",
                "get ",
                "has ",
                "increase ",
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

        public MiKo_2300_MeaninglessCommentAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment)
        {
            const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

            if (comment.StartsWithAny(StartingPhrases, Comparison))
                return true;

            if (comment.StartsWith("is ", Comparison) && comment.EndsWith("?", Comparison))
                return true;

            if (comment.Contains("->"))
                return true;

            var spaces = comment.Count(_ => _.IsWhiteSpace());
            if (spaces < 3)
            {
                // 3 or less words
                return true;
            }

            // TODO: add other stuff
            return false;
        }
    }
}
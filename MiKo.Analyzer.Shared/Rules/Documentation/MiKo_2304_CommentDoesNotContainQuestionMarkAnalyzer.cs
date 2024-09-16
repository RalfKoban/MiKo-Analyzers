using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2304_CommentDoesNotContainQuestionMarkAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2304";

        private static readonly string[] TODOs = { "TODO", "TODO:", "TO DO", "TO DO:" };

        public MiKo_2304_CommentDoesNotContainQuestionMarkAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(ReadOnlySpan<char> comment, SemanticModel semanticModel)
        {
            if (comment.Contains('?') is false)
            {
                return false;
            }

            if (comment.ToString().ContainsAny(TODOs))
            {
                // allow TODOs
                return false;
            }

            foreach (ReadOnlySpan<char> word in comment.SplitBy(Constants.WhiteSpaceCharacters))
            {
                if (word.Contains('?') && word.Contains("://") is false)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
using System;
using System.Linq;

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

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel)
        {
            if (comment.Contains("?") is false)
            {
                return false;
            }

            if (comment.ContainsAny(TODOs))
            {
                // allow TODOs
                return false;
            }

            var questionMarkWithoutHyperlink = comment.Split(Constants.WhiteSpaces, StringSplitOptions.RemoveEmptyEntries)
                                                      .Where(_ => _.Contains("?"))
                                                      .Any(_ => _.Contains("://") is false);

            return questionMarkWithoutHyperlink;
        }
    }
}
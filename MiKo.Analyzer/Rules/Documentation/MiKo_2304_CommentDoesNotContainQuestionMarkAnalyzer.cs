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

        public MiKo_2304_CommentDoesNotContainQuestionMarkAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel)
        {
            if (comment.Contains("?") is false)
            {
                return false;
            }

            // allow indicators such as http:// or ftp://
            var questionMarkWithoutHyperlink = comment.Split(Constants.WhiteSpaces, StringSplitOptions.RemoveEmptyEntries)
                                                      .Where(_ => _.Contains("?"))
                                                      .Any(_ => _.Contains("://") is false);
            return questionMarkWithoutHyperlink;
        }
    }
}
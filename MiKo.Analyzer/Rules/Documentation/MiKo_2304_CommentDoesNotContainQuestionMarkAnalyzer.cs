using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2304_CommentDoesNotContainQuestionMarkAnalyzer : SingleLineCommentAnalyzer
    {
        public const string Id = "MiKo_2304";

        public MiKo_2304_CommentDoesNotContainQuestionMarkAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => comment.Contains("?");
    }
}
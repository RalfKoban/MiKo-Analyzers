using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2307_CommentContainsWasNotSuccessfulAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2307";

        public MiKo_2307_CommentContainsWasNotSuccessfulAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => comment.Contains(Constants.Comments.WasNotSuccessfulPhrase);
    }
}
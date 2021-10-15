using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2309_CommentContainsNtContradictionAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2309";

        public MiKo_2309_CommentContainsNtContradictionAnalyzer() : base(Id)
        {
        }

        internal static bool CommentHasIssue(string comment) => comment.ContainsAny(Constants.Comments.NotContradictionPhrase, StringComparison.OrdinalIgnoreCase);

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => CommentHasIssue(comment);
    }
}
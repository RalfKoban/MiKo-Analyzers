using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2303";

        public MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer() : base(Id)
        {
        }

        internal static bool CommentHasIssue(string comment) => comment.EndsWith(".", StringComparison.OrdinalIgnoreCase)
                                                      && comment.EndsWith("...", StringComparison.OrdinalIgnoreCase) is false
                                                      && comment.EndsWith("etc.", StringComparison.OrdinalIgnoreCase) is false;

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => CommentHasIssue(comment);
    }
}
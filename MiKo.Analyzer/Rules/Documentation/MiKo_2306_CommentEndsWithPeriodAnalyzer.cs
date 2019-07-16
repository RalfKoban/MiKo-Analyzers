using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2306_CommentEndsWithPeriodAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2306";

        public MiKo_2306_CommentEndsWithPeriodAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => !comment.EndsWith(".", StringComparison.OrdinalIgnoreCase);
    }
}
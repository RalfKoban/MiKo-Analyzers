using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer : SingleLineCommentAnalyzer
    {
        public const string Id = "MiKo_2303";

        public MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => comment.EndsWith(".", StringComparison.OrdinalIgnoreCase);
    }
}
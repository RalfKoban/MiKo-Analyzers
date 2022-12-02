using System;
using System.Collections.Generic;

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

        internal static bool CommentHasIssue(ReadOnlySpan<char> comment) => comment.EndsWith('.')
                                                                         && comment.EndsWith("...", StringComparison.OrdinalIgnoreCase) is false
                                                                         && comment.EndsWith("etc.", StringComparison.OrdinalIgnoreCase) is false;

        protected override bool CommentHasIssue(ReadOnlySpan<char> comment, SemanticModel semanticModel) => CommentHasIssue(comment);

        protected override IEnumerable<Diagnostic> CollectIssues(string name, SyntaxTrivia trivia)
        {
            yield return Issue(name, GetLastLocation(trivia, ".", StringComparison.OrdinalIgnoreCase));
        }
    }
}
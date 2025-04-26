﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2305";

        public MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel) => DocumentationComment.ContainsDoublePeriod(comment);
    }
}
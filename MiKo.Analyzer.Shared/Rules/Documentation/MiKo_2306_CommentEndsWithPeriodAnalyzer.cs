﻿using System;

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

        public static bool EnabledPerDefault { get; set; } = false;

        protected override bool IsEnabledByDefault => EnabledPerDefault;

        protected override bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel) => comment.EndsWith('.') is false;
    }
}
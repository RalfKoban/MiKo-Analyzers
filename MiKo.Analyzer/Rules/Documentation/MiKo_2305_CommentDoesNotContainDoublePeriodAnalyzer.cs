﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2305";

        private static readonly HashSet<char> AllowedChars = new HashSet<char>
                                                                 {
                                                                     '.',
                                                                     '/',
                                                                     '\\',
                                                                 };

        public MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => comment.Contains("..", _ => !AllowedChars.Contains(_), StringComparison.OrdinalIgnoreCase);
    }
}
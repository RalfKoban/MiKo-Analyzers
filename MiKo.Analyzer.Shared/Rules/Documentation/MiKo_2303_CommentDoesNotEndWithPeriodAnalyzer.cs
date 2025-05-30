﻿using System;
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

        protected override bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel) => DocumentationComment.EndsWithPeriod(comment);

        protected override IEnumerable<Diagnostic> CollectIssues(string name, in SyntaxTrivia trivia)
        {
            return new[] { Issue(name, GetLastLocation(trivia, '.')) };
        }
    }
}
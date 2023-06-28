﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2310_CommentContainsIntentionallyAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2310";

        public MiKo_2310_CommentContainsIntentionallyAnalyzer() : base(Id)
        {
        }

        internal static bool CommentHasIssue(ReadOnlySpan<char> comment) => comment.ContainsAny(Constants.Comments.IntentionallyPhrase, StringComparison.OrdinalIgnoreCase);

        protected override bool CommentHasIssue(ReadOnlySpan<char> comment, SemanticModel semanticModel) => CommentHasIssue(comment);

        protected override IEnumerable<Diagnostic> CollectIssues(string name, SyntaxTrivia trivia) => GetAllLocations(trivia, Constants.Comments.IntentionallyPhrase, StringComparison.OrdinalIgnoreCase).Select(_ => Issue(name, _));
    }
}
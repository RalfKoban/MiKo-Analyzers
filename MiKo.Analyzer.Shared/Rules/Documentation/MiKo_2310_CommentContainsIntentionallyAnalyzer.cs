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

        protected override bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel) => CommentHasIssue(comment);

        protected override IEnumerable<Diagnostic> CollectIssues(string name, in SyntaxTrivia trivia) => GetAllLocations(trivia, Constants.Comments.IntentionallyPhrase, StringComparison.OrdinalIgnoreCase).Select(_ => Issue(name, _));

        private static bool CommentHasIssue(in ReadOnlySpan<char> comment)
        {
            var c = comment.ToString();

            if (c.ContainsAny(Constants.Comments.IntentionallyPhrase, StringComparison.OrdinalIgnoreCase))
            {
                return c.ContainsAny(Constants.Comments.ReasoningPhrases, StringComparison.OrdinalIgnoreCase) is false;
            }

            return false;
        }
    }
}
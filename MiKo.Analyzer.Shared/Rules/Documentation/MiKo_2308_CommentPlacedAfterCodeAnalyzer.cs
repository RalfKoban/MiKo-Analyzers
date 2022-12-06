﻿using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2308_CommentPlacedAfterCodeAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2308";

        public MiKo_2308_CommentPlacedAfterCodeAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(ReadOnlySpan<char> comment, SemanticModel semanticModel) => false;

        protected override bool CommentHasIssue(SyntaxTrivia trivia, SemanticModel semanticModel)
        {
            var current = trivia.Token;

            if (current.IsKind(SyntaxKind.SemicolonToken))
            {
                // comment is on same line as the semicolon, so it's no issue (except for initializers)
                if (current.Parent is LocalDeclarationStatementSyntax d)
                {
                    return d.DescendantNodes<InitializerExpressionSyntax>().Any();
                }

                return false;
            }

            if (current.Parent is InitializerExpressionSyntax)
            {
                // comment is on collection initializer, so it's no issue
                return false;
            }

            var next = current.GetNextToken();

            if (next.IsKind(SyntaxKind.CloseBraceToken))
            {
                var previous = current.GetPreviousToken();

                if (previous.IsKind(SyntaxKind.OpenBraceToken))
                {
                    // comment is the only comment between opening and closing brace
                    return false;
                }

                var comment = trivia.ToString().AsSpan().Trim();

                if (comment.StartsWith("////", StringComparison.OrdinalIgnoreCase))
                {
                    // that's a comment to ignore
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
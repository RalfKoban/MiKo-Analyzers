using System;

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
                    foreach (var node in d.DescendantNodes())
                    {
                        switch (node)
                        {
                            case InitializerExpressionSyntax _:
#if VS2022
                            case CollectionExpressionSyntax _:
#endif
                                return true;
                        }
                    }
                }

                return false;
            }

            if (IsOnInitializer(current.Parent))
            {
                // comment is on initializer, so it's no issue
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

        private static bool IsOnInitializer(SyntaxNode node)
        {
            if (node is InitializerExpressionSyntax)
            {
                return true;
            }

            switch (node?.Parent)
            {
                case InitializerExpressionSyntax _:
                case AssignmentExpressionSyntax assignment when assignment.Parent is InitializerExpressionSyntax:
                    return true;

                default:
                    return false;
            }
        }
    }
}
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6044_CodeFixProvider)), Shared]
    public sealed class MiKo_6044_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6044";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case BinaryExpressionSyntax _:
                    case PrefixUnaryExpressionSyntax _:
                    case PostfixUnaryExpressionSyntax _:
                        return node;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case BinaryExpressionSyntax binary: return GetUpdatedSyntax(binary, issue);
                case PrefixUnaryExpressionSyntax unary: return GetUpdatedSyntax(unary);
                case PostfixUnaryExpressionSyntax unary: return GetUpdatedSyntax(unary);
            }

            return syntax;
        }

        private static PrefixUnaryExpressionSyntax GetUpdatedSyntax(PrefixUnaryExpressionSyntax unary) => unary.WithOperatorToken(unary.OperatorToken.WithoutTrailingTrivia())
                                                                                                               .WithOperand(unary.Operand.WithoutLeadingTrivia());

        private static PostfixUnaryExpressionSyntax GetUpdatedSyntax(PostfixUnaryExpressionSyntax unary) => unary.WithOperand(unary.Operand.WithoutTrailingTrivia())
                                                                                                                 .WithOperatorToken(unary.OperatorToken.WithoutLeadingTrivia());

        private static BinaryExpressionSyntax GetUpdatedSyntax(BinaryExpressionSyntax binary, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            var left = binary.Left;
            var operatorToken = binary.OperatorToken;

            var updatedLeft = left;

            if (left.IsOnSameLineAs(operatorToken) || left.HasTrailingComment() is false)
            {
                // copy comment or line break
                updatedLeft = left.WithTrailingTriviaFrom(operatorToken);
            }

            var updatedToken = operatorToken.WithLeadingSpaces(spaces).WithTrailingSpace();
            var updatedRight = binary.Right.WithoutLeadingTrivia();

            return binary.WithLeft(updatedLeft)
                         .WithOperatorToken(updatedToken)
                         .WithRight(updatedRight);
        }
    }
}
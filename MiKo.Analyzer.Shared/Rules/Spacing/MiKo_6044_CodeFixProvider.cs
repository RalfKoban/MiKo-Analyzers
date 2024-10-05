using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

        private static BinaryExpressionSyntax GetUpdatedSyntax(BinaryExpressionSyntax binary, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            var left = binary.Left;
            var operatorToken = binary.OperatorToken;
            var right = binary.Right;

            var updatedLeft = left.GetStartingLine() != operatorToken.GetStartingLine() && left.HasTrailingComment()
                                  ? left // there is already a comment, so nothing to do here
                                  : left.WithTrailingTriviaFrom(operatorToken); // copy comment or line break

            var updatedToken = operatorToken.WithLeadingSpaces(spaces).WithTrailingSpace();
            var updatedRight = right.WithoutLeadingTrivia();

            return binary.WithLeft(updatedLeft)
                         .WithOperatorToken(updatedToken)
                         .WithRight(updatedRight);
        }

        private static PrefixUnaryExpressionSyntax GetUpdatedSyntax(PrefixUnaryExpressionSyntax unary) => unary.WithOperatorToken(unary.OperatorToken.WithoutTrailingTrivia())
                                                                                                               .WithOperand(unary.Operand.WithoutLeadingTrivia());

        private static PostfixUnaryExpressionSyntax GetUpdatedSyntax(PostfixUnaryExpressionSyntax unary) => unary.WithOperand(unary.Operand.WithoutTrailingTrivia())
                                                                                                                 .WithOperatorToken(unary.OperatorToken.WithoutLeadingTrivia());
    }
}
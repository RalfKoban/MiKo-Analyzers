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
        public override string FixableDiagnosticId => MiKo_6044_OperatorsOfBinaryExpressionsAreOnSameLineAsRightOperandAnalyzer.Id;

        protected override string Title => Resources.MiKo_6044_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BinaryExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BinaryExpressionSyntax binary)
            {
                var spaces = binary.Right.GetStartPosition().Character;

                // copy comments by applying the trailing trivia from the corresponding items
                return binary.WithLeft(binary.Left.WithTrailingTrivia(binary.OperatorToken.TrailingTrivia))
                             .WithOperatorToken(binary.OperatorToken.WithLeadingSpaces(spaces).WithTrailingTrivia(binary.Right.GetTrailingTrivia()))
                             .WithRight(binary.Right.WithoutTrivia().WithLeadingSpace());
            }

            return syntax;
        }
    }
}
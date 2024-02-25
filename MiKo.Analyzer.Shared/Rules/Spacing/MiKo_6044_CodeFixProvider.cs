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

        protected override string Title => Resources.MiKo_6044_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BinaryExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BinaryExpressionSyntax binary)
            {
                var spaces = MiKo_6044_BooleanOperatorsAreOnSameLineAsRightOperandAnalyzer.GetSpaces(issue);

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

            return syntax;
        }
    }
}
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6074_CodeFixProvider)), Shared]
    public sealed class MiKo_6074_CodeFixProvider : IndendedSpacingCodeFixProvider<BinaryExpressionSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6074";

        protected override BinaryExpressionSyntax GetUpdatedSyntax(BinaryExpressionSyntax node, int spaces)
        {
            var operatorToken = node.OperatorToken;
            var rightOperand = node.Right;

            if (operatorToken.IsOnSameLineAs(rightOperand))
            {
                return node.WithOperatorToken(operatorToken.WithLeadingSpaces(spaces));
            }

            return node.WithRight(rightOperand.WithLeadingSpaces(spaces));
        }
    }
}
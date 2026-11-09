using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6074_CodeFixProvider)), Shared]
    public sealed class MiKo_6074_CodeFixProvider : IndentedSpacingCodeFixProvider<BinaryExpressionSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6074";

        protected override TSyntaxNode GetUpdatedSyntax<TSyntaxNode>(TSyntaxNode syntax, in int leadingSpaces)
        {
            if (syntax is BinaryExpressionSyntax node)
            {
                var operatorToken = node.OperatorToken;
                var rightOperand = node.Right;

                if (operatorToken.IsOnSameLineAs(rightOperand))
                {
                    return node.WithOperatorToken(operatorToken.WithLeadingSpaces(leadingSpaces)) as TSyntaxNode;
                }

                return node.WithRight(rightOperand.WithLeadingSpaces(leadingSpaces)) as TSyntaxNode;
            }

            return syntax;
        }
    }
}
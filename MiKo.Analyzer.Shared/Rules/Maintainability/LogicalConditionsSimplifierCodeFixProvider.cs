using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class LogicalConditionsSimplifierCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected abstract SyntaxKind PredefinedTypeKind { get; }

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var node = syntaxNodes.OfType<BinaryExpressionSyntax>().FirstOrDefault();

            if (node?.Parent is ParenthesizedExpressionSyntax parenthesized && parenthesized.Expression == node)
            {
                return parenthesized;
            }

            return node;
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is ExpressionSyntax expression && expression.WithoutParenthesis() is BinaryExpressionSyntax binary)
            {
                if (binary.Left.WithoutParenthesis() is BinaryExpressionSyntax left)
                {
                    var arguments = binary.Right.FirstDescendant<ArgumentListSyntax>()?.Arguments;

                    var argument1 = Argument(left.Left);
                    var argument2 = Argument(left.Right);

                    var member = Member(PredefinedType(PredefinedTypeKind), nameof(Equals));

                    var updatedSyntax = arguments?.Count > 1
                                        ? Invocation(member, argument1, argument2, arguments.Value[1])
                                        : Invocation(member, argument1, argument2);

                    return updatedSyntax.WithTriviaFrom(syntax);
                }
            }

            return syntax;
        }
    }
}
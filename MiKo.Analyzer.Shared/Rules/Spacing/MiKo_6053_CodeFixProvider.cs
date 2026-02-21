using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6053_CodeFixProvider)), Shared]
    public sealed class MiKo_6053_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6053";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ArgumentSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is ArgumentSyntax argument)
            {
                if (argument.Expression is MemberAccessExpressionSyntax expression)
                {
                    var updatedExpression = expression.WithExpression(expression.Expression.WithoutTrailingTrivia())
                                                      .WithOperatorToken(expression.OperatorToken.WithoutTrivia())
                                                      .WithName(expression.Name.WithoutLeadingTrivia());

                    return argument.WithExpression(updatedExpression);
                }
            }

            return syntax;
        }
    }
}

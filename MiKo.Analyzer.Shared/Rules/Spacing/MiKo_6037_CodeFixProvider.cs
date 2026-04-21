using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6037_CodeFixProvider)), Shared]
    public sealed class MiKo_6037_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6037";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var foundArgument = false;

            // we have an argument that is part of an invocation, but the invocation itself could also be part of another argument,
            // so we need to find the argument first and then get the invocation from it (as otherwise we would report the wrong, nested invocation)
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case ArgumentSyntax _:
                        foundArgument = true;

                        break;

                    case InvocationExpressionSyntax i when foundArgument:
                        return i;
                }
            }

            return null;
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is InvocationExpressionSyntax invocation)
            {
                return invocation.WithArgumentList(invocation.ArgumentList.PlacedOnSameLine())
                                 .WithExpression(PlacedOnSameLine(invocation.Expression))
                                 .WithTrailingTriviaFrom(invocation);
            }

            return syntax;
        }

        private static ExpressionSyntax PlacedOnSameLine(ExpressionSyntax expression) => expression is MemberAccessExpressionSyntax maes
                                                                                         ? maes.WithName(maes.Name.PlacedOnSameLine())
                                                                                         : expression;
    }
}
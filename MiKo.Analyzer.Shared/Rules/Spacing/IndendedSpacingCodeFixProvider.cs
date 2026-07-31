using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class IndendedSpacingCodeFixProvider<T> : SpacingCodeFixProvider where T : SyntaxNode
    {
        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<T>().First();

        protected sealed override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        protected virtual T GetUpdatedSyntax(T node, int spaces) => node.WithLeadingSpaces(spaces);

        private SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is T node)
            {
                var spaces = GetProposedSpaces(issue);

                return GetUpdatedSyntax(node, spaces);
            }

            return syntax;
        }
    }
}
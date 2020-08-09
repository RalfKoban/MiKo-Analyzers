using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityCodeFixProvider : MiKoCodeFixProvider
    {
        protected MaintainabilityCodeFixProvider() : base(false)
        {
        }

        protected sealed override Task<Document> ApplyDocumentCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);
            var newRoot = root.ReplaceNode(syntax, updatedSyntax);
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        protected abstract SyntaxNode GetUpdatedSyntax(SyntaxNode syntax);
    }
}
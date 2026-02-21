using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class OrderingCodeFixProvider : MiKoCodeFixProvider
    {
        protected static SyntaxNode PlaceFirst<T>(SyntaxNode syntax, BaseTypeDeclarationSyntax typeSyntax) where T : SyntaxNode
        {
            var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(syntax);

            var firstChild = modifiedType.FirstChild<T>();

            return modifiedType.InsertNodeBefore(firstChild, syntax);
        }

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected sealed override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult(syntax);
        }

        protected override async Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var typeSyntax = syntax.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();

            var updatedTypeSyntax = await GetUpdatedTypeSyntaxAsync(document, typeSyntax, syntax, issue, cancellationToken).ConfigureAwait(false);

            return root.ReplaceNode(typeSyntax, updatedTypeSyntax);
        }

        protected abstract Task<SyntaxNode> GetUpdatedTypeSyntaxAsync(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic issue, CancellationToken cancellationToken);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class AttributeSyntaxCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<AttributeSyntax>().FirstOrDefault();

        protected sealed override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult(syntax);
        }

        protected sealed override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntaxRoot(root, syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax)
        {
            if (syntax is AttributeSyntax)
            {
                var nodeToRemove = syntax.Parent is AttributeListSyntax attributeList && attributeList.Attributes.Count is 1
                                       ? attributeList
                                       : syntax;

                return root.Without(nodeToRemove);
            }

            return root;
        }
    }
}
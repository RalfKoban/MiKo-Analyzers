using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class OrderingCodeFixProvider : MiKoCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var typeSyntax = syntax.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().First();

            var updatedTypeSyntax = GetUpdatedTypeSyntax(document, typeSyntax, syntax, diagnostic);

            return root.ReplaceNode(typeSyntax, updatedTypeSyntax);
        }

        protected abstract SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic);
    }
}
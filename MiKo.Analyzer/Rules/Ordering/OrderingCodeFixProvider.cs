using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class OrderingCodeFixProvider : MiKoCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BaseTypeDeclarationSyntax>().First();

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax) => GetUpdatedTypeSyntax(document, (BaseTypeDeclarationSyntax)syntax);

        protected abstract SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax syntax);
    }
}
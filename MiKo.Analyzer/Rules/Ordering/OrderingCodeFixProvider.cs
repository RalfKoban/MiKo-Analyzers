using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class OrderingCodeFixProvider : MiKoCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected sealed override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic issue)
        {
            var typeSyntax = syntax.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().First();

            var updatedTypeSyntax = GetUpdatedTypeSyntax(context, typeSyntax, syntax, issue);

            return root.ReplaceNode(typeSyntax, updatedTypeSyntax);
        }

        protected abstract SyntaxNode GetUpdatedTypeSyntax(CodeFixContext context, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic);
    }
}
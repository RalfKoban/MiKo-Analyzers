﻿using System.Collections.Generic;
using System.Linq;

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

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            var typeSyntax = syntax.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();

            var updatedTypeSyntax = GetUpdatedTypeSyntax(document, typeSyntax, syntax, issue);

            return root.ReplaceNode(typeSyntax, updatedTypeSyntax);
        }

        protected abstract SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic);
    }
}
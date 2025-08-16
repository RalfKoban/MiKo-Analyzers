using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
#pragma warning disable CA1506
namespace MiKoSolutions.Analyzers
{
    internal static partial class SyntaxNodeExtensions
    {
        private static readonly Func<SyntaxNode, bool> AlwaysDescendIntoChildren = _ => true;

        internal static IEnumerable<SyntaxNode> AllDescendantNodes(this SyntaxNode value) => value?.DescendantNodes(AlwaysDescendIntoChildren, true) ?? Array.Empty<SyntaxNode>();

        internal static IEnumerable<T> AllDescendantNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.AllDescendantNodes().OfType<T>();

        internal static IEnumerable<SyntaxNodeOrToken> AllDescendantNodesAndTokens(this SyntaxNode value) => value.DescendantNodesAndTokens(AlwaysDescendIntoChildren);

        internal static IEnumerable<T> Ancestors<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors().OfType<T>(); // value.AncestorsAndSelf().OfType<T>();

        internal static IEnumerable<T> AncestorsWithinMethods<T>(this SyntaxNode value) where T : SyntaxNode
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var ancestor in value.Ancestors())
            {
                if (ancestor is T t)
                {
                    yield return t;
                }

                switch (ancestor)
                {
                    case BaseMethodDeclarationSyntax _: // found the surrounding method
                    case LocalFunctionStatementSyntax _: // found the surrounding local function
                    case BasePropertyDeclarationSyntax _: // found the surrounding property, so we already skipped the getters or setters
                        yield break;
                }
            }
        }

        internal static IEnumerable<T> AncestorsWithinDocumentation<T>(this SyntaxNode value) where T : XmlNodeSyntax
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var ancestor in value.Ancestors())
            {
                if (ancestor is T t)
                {
                    yield return t;
                }

                if (ancestor is DocumentationCommentTriviaSyntax)
                {
                    yield break;
                }
            }
        }

        internal static IEnumerable<T> ChildNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes().OfType<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes().OfType<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, in SyntaxKind kind) where T : SyntaxNode => value.DescendantNodes().OfKind(kind).Cast<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().Where(predicate);

        internal static IEnumerable<SyntaxToken> DescendantTokens(this SyntaxNode value, in SyntaxKind kind) => value.DescendantTokens().OfKind(kind);

        internal static T FirstAncestor<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault();

        internal static T FirstAncestor<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault(predicate);

        internal static T FirstAncestor<T>(this SyntaxNode value, ISet<SyntaxKind> kinds) where T : SyntaxNode => value.FirstAncestor<T>(_ => _.IsAnyKind(kinds));

        internal static SyntaxNode FirstChild(this SyntaxNode value) => value.ChildNodes().FirstOrDefault();

        internal static T FirstChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault();

        internal static SyntaxNode FirstChild(this SyntaxNode value, Func<SyntaxNode, bool> predicate) => value.ChildNodes().FirstOrDefault(predicate);

        internal static T FirstChild<T>(this SyntaxNode value, in SyntaxKind kind) where T : SyntaxNode => value.ChildNodes().OfKind(kind).FirstOrDefault() as T;

        internal static T FirstChild<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault(predicate);

        internal static SyntaxToken FirstChildToken(this SyntaxNode value) => value.ChildTokens().FirstOrDefault();

        internal static SyntaxToken FirstChildToken(this SyntaxNode value, in SyntaxKind kind) => value.ChildTokens().OfKind(kind).First();

        internal static T FirstDescendant<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault();

        internal static T FirstDescendant<T>(this SyntaxNode value, in SyntaxKind kind) where T : SyntaxNode => value.DescendantNodes().OfKind(kind).FirstOrDefault() as T;

        internal static T FirstDescendant<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault(predicate);

        internal static SyntaxToken FirstDescendantToken(this SyntaxNode value) => value.DescendantTokens().FirstOrDefault();

        internal static SyntaxToken FirstDescendantToken(this SyntaxNode value, in SyntaxKind kind) => value.DescendantTokens().OfKind(kind).First();

        internal static T LastChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().LastOrDefault();

        internal static T GetEnclosing<T>(this SyntaxNode value) where T : SyntaxNode => value.FirstAncestorOrSelf<T>();

        internal static SyntaxNode GetEnclosing(this SyntaxNode value, ISet<SyntaxKind> syntaxKinds)
        {
            var node = value;

            while (true)
            {
                if (node is null)
                {
                    return null;
                }

                if (node.IsAnyKind(syntaxKinds))
                {
                    return node;
                }

                node = node is DocumentationCommentTriviaSyntax d
                       ? d.ParentTrivia.Token.Parent
                       : node.Parent;
            }
        }

        internal static SyntaxNode GetEnclosing(this SyntaxNode value, in ReadOnlySpan<SyntaxKind> syntaxKinds)
        {
            var node = value;

            while (true)
            {
                if (node is null)
                {
                    return null;
                }

                if (node.IsAnyKind(syntaxKinds))
                {
                    return node;
                }

                node = node is DocumentationCommentTriviaSyntax d
                       ? d.ParentTrivia.Token.Parent
                       : node.Parent;
            }
        }

        internal static IMethodSymbol GetEnclosingMethod(this in SyntaxNodeAnalysisContext value)
        {
            if (value.ContainingSymbol is IMethodSymbol m)
            {
                return m;
            }

            return GetEnclosingMethod(value.Node, value.SemanticModel);
        }

        internal static IMethodSymbol GetEnclosingMethod(this SyntaxNode value, SemanticModel semanticModel) => value.GetEnclosingSymbol(semanticModel) as IMethodSymbol;

        internal static ISymbol GetEnclosingSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            switch (value)
            {
                case FieldDeclarationSyntax f: return semanticModel.GetDeclaredSymbol(f);
                case MethodDeclarationSyntax s: return semanticModel.GetDeclaredSymbol(s);
                case PropertyDeclarationSyntax p: return semanticModel.GetDeclaredSymbol(p);
                case ConstructorDeclarationSyntax c: return semanticModel.GetDeclaredSymbol(c);
                case EventDeclarationSyntax e: return semanticModel.GetDeclaredSymbol(e);
                default:
                    return semanticModel.GetEnclosingSymbol(value.GetLocation().SourceSpan.Start);
            }
        }

        internal static SyntaxNode PreviousSibling(this SyntaxNode value)
        {
            var parent = value?.Parent;

            if (parent is null)
            {
                return null;
            }

            SyntaxNode previousChild = null;

            foreach (var child in parent.ChildNodes())
            {
                if (child == value)
                {
                    return previousChild;
                }

                previousChild = child;
            }

            return null;
        }

        internal static SyntaxNodeOrToken PreviousSiblingNodeOrToken(this SyntaxNode value)
        {
            var parent = value?.Parent;

            if (parent is null)
            {
                return default;
            }

            SyntaxNodeOrToken previousChild = default;

            foreach (var child in parent.ChildNodesAndTokens())
            {
                if (child == value)
                {
                    return previousChild;
                }

                previousChild = child;
            }

            return default;
        }

        internal static SyntaxNode NextSibling(this SyntaxNode value)
        {
            var parent = value?.Parent;

            if (parent is null)
            {
                return null;
            }

            using (var enumerator = parent.ChildNodes().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current == value)
                    {
                        var nextSibling = enumerator.MoveNext()
                                          ? enumerator.Current
                                          : null;

                        return nextSibling;
                    }
                }
            }

            return null;
        }

        internal static IList<SyntaxNode> Siblings(this SyntaxNode value) => Siblings<SyntaxNode>(value);

        internal static IList<T> Siblings<T>(this SyntaxNode value) where T : SyntaxNode
        {
            var parent = value?.Parent;

            if (parent != null)
            {
                return parent.ChildNodes<T>().ToList();
            }

            return Array.Empty<T>();
        }
    }
}
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
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxNode"/>s that focus on navigation.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        /// <summary>
        /// Gets all descendant nodes of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get descendants from.
        /// </param>
        /// <returns>
        /// A sequence that contains all descendant nodes of the specified syntax node, or an empty collection if the syntax node is <see langword="null"/>.
        /// </returns>
        internal static IEnumerable<SyntaxNode> AllDescendantNodes(this SyntaxNode value) => value?.DescendantNodes(/* always descent */ descendIntoTrivia: true) ?? Array.Empty<SyntaxNode>();

        /// <summary>
        /// Gets all descendant nodes of the specified syntax node that are of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of nodes to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to get descendants from.
        /// </param>
        /// <returns>
        /// A sequence that contains all descendant nodes of the specified type.
        /// </returns>
        internal static IEnumerable<T> AllDescendantNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.AllDescendantNodes().OfType<T>();

        /// <summary>
        /// Gets all descendant nodes and tokens of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get descendants from.
        /// </param>
        /// <returns>
        /// A sequence that contains all descendant nodes and tokens of the specified syntax node.
        /// </returns>
        internal static IEnumerable<SyntaxNodeOrToken> AllDescendantNodesAndTokens(this SyntaxNode value) => value.DescendantNodesAndTokens(/* always descent */);

        /// <summary>
        /// Gets all ancestors of the specified syntax node that are of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of nodes to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose ancestors to retrieve.
        /// </param>
        /// <returns>
        /// A sequence that contains all ancestors of the specified type.
        /// </returns>
        internal static IEnumerable<T> Ancestors<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors().OfType<T>(); // value.AncestorsAndSelf().OfType<T>();

        /// <summary>
        /// Gets all ancestors of the specified syntax node that are of type <typeparamref name="T"/> and are within a documentation comment.
        /// </summary>
        /// <typeparam name="T">
        /// The type of XML nodes to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose ancestors to retrieve.
        /// </param>
        /// <returns>
        /// A sequence that contains all ancestors of the specified type that are within the documentation comment scope.
        /// </returns>
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

        /// <summary>
        /// Gets all ancestors of the specified syntax node that are of type <typeparamref name="T"/> and are within the scope of a method, local function, or property.
        /// </summary>
        /// <typeparam name="T">
        /// The type of nodes to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose ancestors to retrieve.
        /// </param>
        /// <returns>
        /// A sequence that contains all ancestors of the specified type that are within the method, local function, or property scope.
        /// </returns>
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

        /// <summary>
        /// Gets all child nodes of the specified syntax node that are of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of nodes to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose children to retrieve.
        /// </param>
        /// <returns>
        /// A sequence that contains all child nodes of the specified type.
        /// </returns>
        internal static IEnumerable<T> ChildNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes().OfType<T>();

        /// <summary>
        /// Gets all descendant nodes of the specified syntax node that are of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of nodes to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to get descendants from.
        /// </param>
        /// <returns>
        /// A sequence that contains all descendant nodes of the specified type.
        /// </returns>
        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes().OfType<T>();

        /// <summary>
        /// Gets all descendant nodes of the specified syntax node that are of type <typeparamref name="T"/> and have the specified syntax kind.
        /// </summary>
        /// <typeparam name="T">
        /// The type of nodes to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to get descendants from.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to filter by.
        /// </param>
        /// <returns>
        /// A sequence that contains all descendant nodes of the specified type and syntax kind.
        /// </returns>
        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, in SyntaxKind kind) where T : SyntaxNode
        {
            List<T> results = null;

            foreach (var node in value.DescendantNodes())
            {
                if (node.RawKind != (int)kind)
                {
                    continue;
                }

                if (results is null)
                {
                    results = new List<T>();
                }

                results.Add((T)node);
            }

            return results ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Gets all descendant nodes of the specified syntax node that are of type <typeparamref name="T"/> and satisfy the specified predicate.
        /// </summary>
        /// <typeparam name="T">
        /// The type of nodes to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to get descendants from.
        /// </param>
        /// <param name="predicate">
        /// The predicate to filter nodes by.
        /// </param>
        /// <returns>
        /// A sequence that contains all descendant nodes of the specified type that satisfy the predicate.
        /// </returns>
        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().Where(predicate);

        /// <summary>
        /// Gets all descendant tokens of the specified syntax node that have the specified syntax kind.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get descendant tokens from.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to filter by.
        /// </param>
        /// <returns>
        /// A sequence that contains all descendant tokens of the specified syntax kind.
        /// </returns>
        internal static IEnumerable<SyntaxToken> DescendantTokens(this SyntaxNode value, in SyntaxKind kind) => value.DescendantTokens().OfKind(kind);

        /// <summary>
        /// Gets the first ancestor of the specified syntax node that is of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose ancestor to retrieve.
        /// </param>
        /// <returns>
        /// The first ancestor of the specified type, or <see langword="null"/> if no such ancestor exists.
        /// </returns>
        internal static T FirstAncestor<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault();

        /// <summary>
        /// Gets the first ancestor of the specified syntax node that is of type <typeparamref name="T"/> and satisfies the specified predicate.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose ancestor to retrieve.
        /// </param>
        /// <param name="predicate">
        /// The predicate to filter by.
        /// </param>
        /// <returns>
        /// The first ancestor of the specified type that satisfies the predicate, or <see langword="null"/> if no such ancestor exists.
        /// </returns>
        internal static T FirstAncestor<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault(predicate);

        /// <summary>
        /// Gets the first ancestor of the specified syntax node that is of type <typeparamref name="T"/> and has one of the specified syntax kinds.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose ancestor to retrieve.
        /// </param>
        /// <param name="kinds">
        /// The set of syntax kinds to filter by.
        /// </param>
        /// <returns>
        /// The first ancestor of the specified type that has one of the specified syntax kinds, or <see langword="null"/> if no such ancestor exists.
        /// </returns>
        internal static T FirstAncestor<T>(this SyntaxNode value, ISet<SyntaxKind> kinds) where T : SyntaxNode => value.FirstAncestor<T>(_ => _.IsAnyKind(kinds));

        /// <summary>
        /// Gets the first child node of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose first child to retrieve.
        /// </param>
        /// <returns>
        /// The first child node, or <see langword="null"/> if the node has no children.
        /// </returns>
        internal static SyntaxNode FirstChild(this SyntaxNode value) => value.ChildNodes().FirstOrDefault();

        /// <summary>
        /// Gets the first child node of the specified syntax node that is of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose first child of the specified type to retrieve.
        /// </param>
        /// <returns>
        /// The first child node of the specified type, or <see langword="null"/> if no such child exists.
        /// </returns>
        internal static T FirstChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault();

        /// <summary>
        /// Gets the first child node of the specified syntax node that satisfies the specified predicate.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose first child to retrieve.
        /// </param>
        /// <param name="predicate">
        /// The predicate to filter by.
        /// </param>
        /// <returns>
        /// The first child node that satisfies the predicate, or <see langword="null"/> if no such child exists.
        /// </returns>
        internal static SyntaxNode FirstChild(this SyntaxNode value, Func<SyntaxNode, bool> predicate) => value.ChildNodes().FirstOrDefault(predicate);

        /// <summary>
        /// Gets the first child node of the specified syntax node that is of type <typeparamref name="T"/> and has the specified syntax kind.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose first child to retrieve.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to filter by.
        /// </param>
        /// <returns>
        /// The first child node of the specified type and syntax kind, or <see langword="null"/> if no such child exists.
        /// </returns>
        internal static T FirstChild<T>(this SyntaxNode value, in SyntaxKind kind) where T : SyntaxNode => value.ChildNodes().OfKind(kind).FirstOrDefault() as T;

        /// <summary>
        /// Gets the first child node of the specified syntax node that is of type <typeparamref name="T"/> and satisfies the specified predicate.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose first child to retrieve.
        /// </param>
        /// <param name="predicate">
        /// The predicate to filter by.
        /// </param>
        /// <returns>
        /// The first child node of the specified type that satisfies the predicate, or <see langword="null"/> if no such child exists.
        /// </returns>
        internal static T FirstChild<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault(predicate);

        /// <summary>
        /// Gets the first child token of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose first child token to retrieve.
        /// </param>
        /// <returns>
        /// The first child token, or a default token if the node has no child tokens.
        /// </returns>
        internal static SyntaxToken FirstChildToken(this SyntaxNode value) => value.ChildTokens().FirstOrDefault();

        /// <summary>
        /// Gets the first child token of the specified syntax node that has the specified syntax kind.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose first child token to retrieve.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to filter by.
        /// </param>
        /// <returns>
        /// The first child token of the specified syntax kind.
        /// </returns>
        internal static SyntaxToken FirstChildToken(this SyntaxNode value, in SyntaxKind kind) => value.ChildTokens().OfKind(kind).First();

        /// <summary>
        /// Gets the first descendant node of the specified syntax node that is of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose first descendant to retrieve.
        /// </param>
        /// <returns>
        /// The first descendant node of the specified type, or <see langword="null"/> if no such descendant exists.
        /// </returns>
        internal static T FirstDescendant<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault();

        /// <summary>
        /// Gets the first descendant node of the specified syntax node that is of type <typeparamref name="T"/> and has the specified syntax kind.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose first descendant to retrieve.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to filter by.
        /// </param>
        /// <returns>
        /// The first descendant node of the specified type and syntax kind, or <see langword="null"/> if no such descendant exists.
        /// </returns>
        internal static T FirstDescendant<T>(this SyntaxNode value, in SyntaxKind kind) where T : SyntaxNode => value.DescendantNodes().OfKind(kind).FirstOrDefault() as T;

        /// <summary>
        /// Gets the first descendant node of the specified syntax node that is of type <typeparamref name="T"/> and satisfies the specified predicate.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose first descendant to retrieve.
        /// </param>
        /// <param name="predicate">
        /// The predicate to filter by.
        /// </param>
        /// <returns>
        /// The first descendant node of the specified type that satisfies the predicate, or <see langword="null"/> if no such descendant exists.
        /// </returns>
        internal static T FirstDescendant<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault(predicate);

        /// <summary>
        /// Gets the first descendant token of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose first descendant token to retrieve.
        /// </param>
        /// <returns>
        /// The first descendant token, or a default token if the node has no descendant tokens.
        /// </returns>
        internal static SyntaxToken FirstDescendantToken(this SyntaxNode value) => value.DescendantTokens().FirstOrDefault();

        /// <summary>
        /// Gets the first descendant token of the specified syntax node that has the specified syntax kind.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose first descendant token to retrieve.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to filter by.
        /// </param>
        /// <returns>
        /// The first descendant token of the specified syntax kind.
        /// </returns>
        internal static SyntaxToken FirstDescendantToken(this SyntaxNode value, in SyntaxKind kind) => value.DescendantTokens().OfKind(kind).First();

        /// <summary>
        /// Gets the nearest enclosing node of type <typeparamref name="T"/> that contains the specified syntax node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose enclosing node to retrieve.
        /// </param>
        /// <returns>
        /// The enclosing node of the specified type, or <see langword="null"/> if no such node exists.
        /// </returns>
        internal static T GetEnclosing<T>(this SyntaxNode value) where T : SyntaxNode => value.FirstAncestorOrSelf<T>();

        /// <summary>
        /// Gets the nearest enclosing node that contains the specified syntax node and has one of the specified syntax kinds.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose enclosing node to retrieve.
        /// </param>
        /// <param name="syntaxKinds">
        /// The set of syntax kinds to filter by.
        /// </param>
        /// <returns>
        /// The enclosing node that has one of the specified syntax kinds, or <see langword="null"/> if no such node exists.
        /// </returns>
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

        /// <summary>
        /// Gets the nearest enclosing node that contains the specified syntax node and has one of the specified syntax kinds.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose enclosing node to retrieve.
        /// </param>
        /// <param name="syntaxKinds">
        /// The span of syntax kinds to filter by.
        /// </param>
        /// <returns>
        /// The enclosing node that has one of the specified syntax kinds, or <see langword="null"/> if no such node exists.
        /// </returns>
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

        /// <summary>
        /// Gets the method symbol that encloses the syntax node in the specified analysis context.
        /// </summary>
        /// <param name="value">
        /// The analysis context containing the syntax node.
        /// </param>
        /// <returns>
        /// The method symbol that encloses the syntax node, or <see langword="null"/> if no enclosing method exists.
        /// </returns>
        internal static IMethodSymbol GetEnclosingMethod(this in SyntaxNodeAnalysisContext value)
        {
            if (value.ContainingSymbol is IMethodSymbol m)
            {
                return m;
            }

            return GetEnclosingMethod(value.Node, value.SemanticModel);
        }

        /// <summary>
        /// Gets the method symbol that encloses the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose enclosing method symbol to retrieve.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for symbol resolution.
        /// </param>
        /// <returns>
        /// The method symbol that encloses the syntax node, or <see langword="null"/> if no enclosing method exists.
        /// </returns>
        internal static IMethodSymbol GetEnclosingMethod(this SyntaxNode value, SemanticModel semanticModel) => value.GetEnclosingSymbol(semanticModel) as IMethodSymbol;

        /// <summary>
        /// Gets the symbol that encloses the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose enclosing symbol to retrieve.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for symbol resolution.
        /// </param>
        /// <returns>
        /// The symbol that encloses the syntax node, or <see langword="null"/> if no enclosing symbol exists.
        /// </returns>
        internal static ISymbol GetEnclosingSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            while (true)
            {
                switch (value)
                {
                    case null: return null;
                    case FieldDeclarationSyntax f: return semanticModel.GetDeclaredSymbol(f);
                    case MethodDeclarationSyntax s: return semanticModel.GetDeclaredSymbol(s);
                    case PropertyDeclarationSyntax p: return semanticModel.GetDeclaredSymbol(p);
                    case ConstructorDeclarationSyntax c: return semanticModel.GetDeclaredSymbol(c);
                    case EventDeclarationSyntax e: return semanticModel.GetDeclaredSymbol(e);
                    case DocumentationCommentTriviaSyntax d:
                    {
                        value = d.ParentTrivia.Token.Parent;

                        continue;
                    }

                    default: return semanticModel.GetEnclosingSymbol(value.GetLocation().SourceSpan.Start);
                }
            }
        }

        /// <summary>
        /// Gets the last child node of the specified syntax node that is of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose last child of the specified type to retrieve.
        /// </param>
        /// <returns>
        /// The last child node of the specified type, or <see langword="null"/> if no such child exists.
        /// </returns>
        internal static T LastChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().LastOrDefault();

        /// <summary>
        /// Gets the next sibling node of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose next sibling to retrieve.
        /// </param>
        /// <returns>
        /// The next sibling node, or <see langword="null"/> if no next sibling exists.
        /// </returns>
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

        /// <summary>
        /// Gets the previous sibling node of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose previous sibling to retrieve.
        /// </param>
        /// <returns>
        /// The previous sibling node, or <see langword="null"/> if no previous sibling exists.
        /// </returns>
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

        /// <summary>
        /// Gets the previous sibling node or token of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose previous sibling node or token to retrieve.
        /// </param>
        /// <returns>
        /// The previous sibling node or token, or a default value if no previous sibling exists.
        /// </returns>
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

        /// <summary>
        /// Gets all sibling nodes of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node whose siblings to retrieve.
        /// </param>
        /// <returns>
        /// A collection of syntax nodes that contains all sibling nodes, including the node itself.
        /// </returns>
        internal static IList<SyntaxNode> Siblings(this SyntaxNode value) => Siblings<SyntaxNode>(value);

        /// <summary>
        /// Gets all sibling nodes of the specified syntax node that are of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of nodes to return.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose siblings to retrieve.
        /// </param>
        /// <returns>
        /// A collection of syntax nodes that contains all sibling nodes of the specified type, including the node itself if it's of the specified type.
        /// </returns>
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
#pragma warning disable CA1506
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides extensions for <see cref="SyntaxNode"/>s that are suitable for code fixes where the <see cref="Document"/> class is available.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        /// <summary>
        /// Gets the <see cref="SemanticModel"/> for the specified <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The document to retrieve the semantic model for.
        /// </param>
        /// <returns>
        /// The semantic model for the document.
        /// </returns>
        internal static SemanticModel GetSemanticModel(this Document value)
        {
            if (value.TryGetSemanticModel(out var result))
            {
                return result;
            }

            return value.GetSemanticModelAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously gets the symbol represented by the specified <see cref="SyntaxNode"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the syntax node.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The value of the <see cref="Task{TResult}.Result"/> parameter contains the symbol for the syntax node, or <see langword="null"/> if no symbol is found.
        /// </returns>
        internal static async Task<ISymbol> GetSymbolAsync(this SyntaxNode value, Document document, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            if (document.TryGetSemanticModel(out var semanticModel) is false)
            {
                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            }

            if (value is TypeSyntax typeSyntax)
            {
                return semanticModel?.GetTypeInfo(typeSyntax, cancellationToken).Type;
            }

            return semanticModel?.GetDeclaredSymbol(value, cancellationToken);
        }

        /// <summary>
        /// Gets the symbol represented by the specified <see cref="SyntaxNode"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the syntax node.
        /// </param>
        /// <returns>
        /// The symbol for the syntax node, or <see langword="null"/> if no symbol is found.
        /// </returns>
        internal static ISymbol GetSymbol(this SyntaxNode value, Document document) => value.GetSymbolAsync(document, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Gets the symbol represented by the specified <see cref="InvocationExpressionSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The invocation expression syntax to get the symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the syntax node.
        /// </param>
        /// <returns>
        /// The symbol for the invocation expression, or <see langword="null"/> if no symbol is found.
        /// </returns>
        internal static ISymbol GetSymbol(this InvocationExpressionSyntax value, Document document) => value.GetSymbol(document.GetSemanticModel());

        /// <summary>
        /// Gets the type symbol for the specified <see cref="ArgumentSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The argument syntax to get the type symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the argument syntax.
        /// </param>
        /// <returns>
        /// The type symbol for the argument, or <see langword="null"/> if no type is found.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this ArgumentSyntax value, Document document) => value?.GetTypeSymbol(GetSemanticModel(document));

        /// <summary>
        /// Gets the type symbol for the specified <see cref="ExpressionSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The expression syntax to get the type symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the expression syntax.
        /// </param>
        /// <returns>
        /// The type symbol for the expression, or <see langword="null"/> if no type is found.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this ExpressionSyntax value, Document document)
        {
            if (value is null)
            {
                return null;
            }

            var semanticModel = GetSemanticModel(document);
            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        /// <summary>
        /// Gets the type symbol for the specified <see cref="MemberAccessExpressionSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The member access expression syntax to get the type symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the member access expression syntax.
        /// </param>
        /// <returns>
        /// The type symbol for the member access expression, or <see langword="null"/> if no type is found.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this MemberAccessExpressionSyntax value, Document document) => value?.GetTypeSymbol(GetSemanticModel(document));

        /// <summary>
        /// Gets the type symbol for the specified <see cref="BaseTypeSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The base type syntax to get the type symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the base type syntax.
        /// </param>
        /// <returns>
        /// The type symbol for the base type, or <see langword="null"/> if no type is found.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this BaseTypeSyntax value, Document document) => value?.GetTypeSymbol(GetSemanticModel(document));

        /// <summary>
        /// Gets the type symbol for the specified <see cref="ClassDeclarationSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The class declaration syntax to get the type symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the class declaration syntax.
        /// </param>
        /// <returns>
        /// The type symbol for the class declaration, or <see langword="null"/> if no type is found.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this ClassDeclarationSyntax value, Document document) => value?.GetTypeSymbol(GetSemanticModel(document));

        /// <summary>
        /// Gets the type symbol for the specified <see cref="RecordDeclarationSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The record declaration syntax to get the type symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the record declaration syntax.
        /// </param>
        /// <returns>
        /// The type symbol for the record declaration, or <see langword="null"/> if no type is found.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this RecordDeclarationSyntax value, Document document) => value?.GetTypeSymbol(GetSemanticModel(document));

        /// <summary>
        /// Gets the type symbol for the specified <see cref="VariableDeclarationSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The variable declaration syntax to get the type symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the variable declaration syntax.
        /// </param>
        /// <returns>
        /// The type symbol for the variable declaration, or <see langword="null"/> if no type is found.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this VariableDeclarationSyntax value, Document document) => value?.GetTypeSymbol(GetSemanticModel(document));

        /// <summary>
        /// Gets the type symbol for the specified <see cref="VariableDesignationSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The variable designation syntax to get the type symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the variable designation syntax.
        /// </param>
        /// <returns>
        /// The type symbol for the variable designation, or <see langword="null"/> if no type is found.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this VariableDesignationSyntax value, Document document) => value?.GetTypeSymbol(GetSemanticModel(document));

        /// <summary>
        /// Gets the type symbol for the specified <see cref="TypeSyntax"/> in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The type syntax to get the type symbol for.
        /// </param>
        /// <param name="document">
        /// The document that contains the type syntax.
        /// </param>
        /// <returns>
        /// The type symbol for the type syntax, or <see langword="null"/> if no type is found.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this TypeSyntax value, Document document) => value?.GetTypeSymbol(GetSemanticModel(document));

        /// <summary>
        /// Determines whether the specified <see cref="Document"/> has at least the given C# language version.
        /// </summary>
        /// <param name="value">
        /// The document to check.
        /// </param>
        /// <param name="wantedVersion">
        /// One of the enumeration members that specifies the minimum required C# language version.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the document has at least the specified language version; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasMinimumCSharpVersion(this Document value, LanguageVersion wantedVersion) => value.TryGetSyntaxTree(out var syntaxTree) && syntaxTree.HasMinimumCSharpVersion(wantedVersion);

        /// <summary>
        /// Determines whether the specified <see cref="ArgumentSyntax"/> represents a constant value in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The argument syntax to check.
        /// </param>
        /// <param name="document">
        /// The document that contains the argument syntax.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the argument is a constant; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsConst(this ArgumentSyntax value, Document document)
        {
            var identifierName = value.Expression.GetName();

            var method = value.GetEnclosingMethod(document.GetSemanticModel());
            var type = method.FindContainingType();

            var isConst = type.GetFields(identifierName).Any(_ => _.IsConst);

            if (isConst)
            {
                // const value inside class
                return true;
            }

            // local const variable
            var isLocalConst = method.GetSyntax().DescendantNodes<LocalDeclarationStatementSyntax>(_ => _.IsConst)
                                     .Any(_ => _.Declaration.Variables.Any(__ => __.GetName() == identifierName));

            return isLocalConst;
        }

        /// <summary>
        /// Determines whether the specified <see cref="IsPatternExpressionSyntax"/> represents a nullable type in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The pattern expression syntax to check.
        /// </param>
        /// <param name="document">
        /// The document that contains the pattern expression syntax.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the pattern is nullable; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsNullable(this IsPatternExpressionSyntax value, Document document) => value.Expression.GetSymbol(document) is ITypeSymbol typeSymbol && typeSymbol.IsNullable();

        /// <summary>
        /// Determines whether the specified <see cref="ArgumentSyntax"/> represents an enum value in the context of the given <see cref="Document"/>.
        /// </summary>
        /// <param name="value">
        /// The argument syntax to check.
        /// </param>
        /// <param name="document">
        /// The document that contains the argument syntax.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the argument is an enum; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsEnum(this ArgumentSyntax value, Document document)
        {
            var expression = (MemberAccessExpressionSyntax)value.Expression;

            if (expression.Expression.GetSymbol(document) is ITypeSymbol type)
            {
                return type.IsEnum();
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxNode"/> represents a <c>&lt;see cref="..."/&gt;</c> XML element.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a <c>&lt;see cref="..."/&gt;</c> element; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSeeCref(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax element when element.GetName() is Constants.XmlTag.See:
                {
                    return IsCref(element.Attributes);
                }

                case XmlElementSyntax element when element.GetName() is Constants.XmlTag.See:
                {
                    return IsCref(element.StartTag.Attributes);
                }

                default:
                {
                    return false;
                }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax) => syntax.FirstOrDefault() is XmlCrefAttributeSyntax;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxNode"/> represents a <c>&lt;see cref="..."/&gt;</c> XML element with the given cref value.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="type">
        /// The cref value to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a <c>&lt;see cref="..."/&gt;</c> element with the specified cref; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSeeCref(this SyntaxNode value, string type)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax element when element.GetName() is Constants.XmlTag.See:
                {
                    return IsCref(element.Attributes, type);
                }

                case XmlElementSyntax element when element.GetName() is Constants.XmlTag.See:
                {
                    return IsCref(element.StartTag.Attributes, type);
                }

                default:
                {
                    return false;
                }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax, string content) => syntax.FirstOrDefault() is XmlCrefAttributeSyntax attribute && attribute.Cref.ToString() == content;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxNode"/> represents a <c>&lt;see cref="..."/&gt;</c> XML element with the given <see cref="TypeSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="type">
        /// The type syntax to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a <c>&lt;see cref="..."/&gt;</c> element with the specified type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSeeCref(this SyntaxNode value, TypeSyntax type)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax element when element.GetName() is Constants.XmlTag.See:
                {
                    return IsCref(element.Attributes, type);
                }

                case XmlElementSyntax element when element.GetName() is Constants.XmlTag.See:
                {
                    return IsCref(element.StartTag.Attributes, type);
                }

                default:
                {
                    return false;
                }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax, TypeSyntax t)
            {
                if (syntax.FirstOrDefault() is XmlCrefAttributeSyntax attribute)
                {
                    if (attribute.Cref is NameMemberCrefSyntax m)
                    {
                        return t is GenericNameSyntax
                               ? IsSameGeneric(m.Name, t)
                               : IsSameName(m.Name, t);
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxNode"/> represents a <c>&lt;see cref="..."/&gt;</c> XML element with the given <see cref="TypeSyntax"/> and member name.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="type">
        /// The type syntax to match.
        /// </param>
        /// <param name="member">
        /// The member name to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a <c>&lt;see cref="..."/&gt;</c> element with the specified type and member; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSeeCref(this SyntaxNode value, TypeSyntax type, NameSyntax member)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax element when element.GetName() is Constants.XmlTag.See:
                {
                    return IsCref(element.Attributes, type, member);
                }

                case XmlElementSyntax element when element.GetName() is Constants.XmlTag.See:
                {
                    return IsCref(element.StartTag.Attributes, type, member);
                }

                default:
                {
                    return false;
                }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax, TypeSyntax t, NameSyntax name)
            {
                if (syntax.FirstOrDefault() is XmlCrefAttributeSyntax attribute)
                {
                    if (attribute.Cref is QualifiedCrefSyntax q && IsSameGeneric(q.Container, t))
                    {
                        if (q.Member is NameMemberCrefSyntax m && IsSameName(m.Name, name))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxNode"/> represents a <c>&lt;see cref="Task{TResult}.Result"/&gt;</c> XML element.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a <c>&lt;see cref="Task{TResult}.Result"/&gt;</c> element; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSeeCrefTaskResult(this SyntaxNode value)
        {
            var type = "Task<TResult>".AsTypeSyntax();
            var member = SyntaxFactory.ParseName(nameof(Task<object>.Result));

            return value.IsSeeCref(type, member);
        }

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxNode"/> represents a <c>&lt;see cref="Task"/&gt;</c> or <c>&lt;see cref="Task{TResult}"/&gt;</c> XML element.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a <c>&lt;see cref="Task"/&gt;</c> or <c>&lt;see cref="Task{TResult}"/&gt;</c> element; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSeeCrefTask(this SyntaxNode value)
        {
            if (value.IsSeeCref("Task".AsTypeSyntax()))
            {
                return true;
            }

            if (value.IsSeeCref("Task<TResult>".AsTypeSyntax()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxNode"/> represents a string creation expression.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a string creation expression; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsStringCreation(this SyntaxNode value)
        {
            if (value is BinaryExpressionSyntax b && b.IsKind(SyntaxKind.AddExpression))
            {
                if (b.Left.IsStringLiteral() || b.Right.IsStringLiteral())
                {
                    return true;
                }

                if (b.Left.IsStringCreation() || b.Right.IsStringCreation())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a copy of the specified <see cref="AccessorDeclarationSyntax"/> with a semicolon token.
        /// </summary>
        /// <param name="value">
        /// The accessor declaration syntax to modify.
        /// </param>
        /// <returns>
        /// A copy of the accessor declaration syntax with a semicolon token.
        /// </returns>
        internal static AccessorDeclarationSyntax WithSemicolonToken(this AccessorDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.SemicolonToken.AsToken());

        /// <summary>
        /// Gets a copy of the specified <see cref="MethodDeclarationSyntax"/> with a semicolon token.
        /// </summary>
        /// <param name="value">
        /// The method declaration syntax to modify.
        /// </param>
        /// <returns>
        /// A copy of the method declaration syntax with a semicolon token.
        /// </returns>
        internal static MethodDeclarationSyntax WithSemicolonToken(this MethodDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.SemicolonToken.AsToken());

        /// <summary>
        /// Gets a copy of the specified <see cref="PropertyDeclarationSyntax"/> with a semicolon token.
        /// </summary>
        /// <param name="value">
        /// The property declaration syntax to modify.
        /// </param>
        /// <returns>
        /// A copy of the property declaration syntax with a semicolon token.
        /// </returns>
        internal static PropertyDeclarationSyntax WithSemicolonToken(this PropertyDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.SemicolonToken.AsToken());

        /// <summary>
        /// Gets a copy of the specified <see cref="AccessorDeclarationSyntax"/> without a semicolon token.
        /// </summary>
        /// <param name="value">
        /// The accessor declaration syntax to modify.
        /// </param>
        /// <returns>
        /// A copy of the accessor declaration syntax without a semicolon token.
        /// </returns>
        internal static AccessorDeclarationSyntax WithoutSemicolonToken(this AccessorDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.None.AsToken());

        /// <summary>
        /// Gets a copy of the specified <see cref="MethodDeclarationSyntax"/> without a semicolon token.
        /// </summary>
        /// <param name="value">
        /// The method declaration syntax to modify.
        /// </param>
        /// <returns>
        /// A copy of the method declaration syntax without a semicolon token.
        /// </returns>
        internal static MethodDeclarationSyntax WithoutSemicolonToken(this MethodDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.None.AsToken());

        /// <summary>
        /// Gets a copy of the specified <see cref="PropertyDeclarationSyntax"/> without a semicolon token.
        /// </summary>
        /// <param name="value">
        /// The property declaration syntax to modify.
        /// </param>
        /// <returns>
        /// A copy of the property declaration syntax without a semicolon token.
        /// </returns>
        internal static PropertyDeclarationSyntax WithoutSemicolonToken(this PropertyDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.None.AsToken());

        /// <summary>
        /// Gets a copy of the specified <see cref="AccessorDeclarationSyntax"/> without an expression body and without a semicolon token.
        /// </summary>
        /// <param name="value">
        /// The accessor declaration syntax to modify.
        /// </param>
        /// <returns>
        /// A copy of the accessor declaration syntax without an expression body and without a semicolon token.
        /// </returns>
        internal static AccessorDeclarationSyntax WithoutExpressionBody(this AccessorDeclarationSyntax value) => value.WithExpressionBody(null).WithoutSemicolonToken();

        /// <summary>
        /// Gets a copy of the specified <see cref="MethodDeclarationSyntax"/> without an expression body and without a semicolon token.
        /// </summary>
        /// <param name="value">
        /// The method declaration syntax to modify.
        /// </param>
        /// <returns>
        /// A copy of the method declaration syntax without an expression body and without a semicolon token.
        /// </returns>
        internal static MethodDeclarationSyntax WithoutExpressionBody(this MethodDeclarationSyntax value) => value.WithExpressionBody(null).WithoutSemicolonToken();

        /// <summary>
        /// Gets a copy of the specified <see cref="PropertyDeclarationSyntax"/> without an expression body and without a semicolon token.
        /// </summary>
        /// <param name="value">
        /// The property declaration syntax to modify.
        /// </param>
        /// <returns>
        /// A copy of the property declaration syntax without an expression body and without a semicolon token.
        /// </returns>
        internal static PropertyDeclarationSyntax WithoutExpressionBody(this PropertyDeclarationSyntax value) => value.WithExpressionBody(null).WithoutSemicolonToken();

        private static bool IsSameGeneric(TypeSyntax t1, TypeSyntax t2)
        {
            if (t1 is GenericNameSyntax g1 && t2 is GenericNameSyntax g2)
            {
                if (g1.Identifier.ValueText == g2.Identifier.ValueText)
                {
                    var arguments1 = g1.TypeArgumentList.Arguments;
                    var arguments2 = g2.TypeArgumentList.Arguments;

                    // keep in local variable to avoid multiple requests (see Roslyn implementation)
                    var arguments1Count = arguments1.Count;
                    var arguments2Count = arguments2.Count;

                    if (arguments1Count == arguments2Count)
                    {
                        for (var i = 0; i < arguments1Count; i++)
                        {
                            if (IsSameName(arguments1[i], arguments2[i]) is false)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsSameName(TypeSyntax t1, TypeSyntax t2)
        {
            if (t1 is IdentifierNameSyntax n1 && t2 is IdentifierNameSyntax n2)
            {
                return n1.Identifier.ValueText == n2.Identifier.ValueText;
            }

            return t1.ToString() == t2.ToString();
        }
    }
}
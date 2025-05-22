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
    /// Provides extensions for <see cref="SyntaxNode"/>s that can be used in code fixes where the <see cref="Document"/> class is available.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        internal static SemanticModel GetSemanticModel(this Document document)
        {
            if (document.TryGetSemanticModel(out var result))
            {
                return result;
            }

            return document.GetSemanticModelAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        internal static async Task<ISymbol> GetSymbolAsync(this SyntaxNode syntax, Document document, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            if (document.TryGetSemanticModel(out var semanticModel) is false)
            {
                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            }

            if (syntax is TypeSyntax typeSyntax)
            {
                return semanticModel?.GetTypeInfo(typeSyntax, cancellationToken).Type;
            }

            return semanticModel?.GetDeclaredSymbol(syntax, cancellationToken);
        }

        internal static ISymbol GetSymbol(this SyntaxNode syntax, Document document) => syntax.GetSymbolAsync(document, CancellationToken.None).GetAwaiter().GetResult();

        internal static ISymbol GetSymbol(this InvocationExpressionSyntax syntax, Document document) => syntax.GetSymbol(document.GetSemanticModel());

        internal static ITypeSymbol GetTypeSymbol(this ArgumentSyntax value, Document document) => value?.Expression.GetTypeSymbol(GetSemanticModel(document));

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

        internal static ITypeSymbol GetTypeSymbol(this MemberAccessExpressionSyntax value, Document document) => value?.Expression.GetTypeSymbol(GetSemanticModel(document));

        internal static ITypeSymbol GetTypeSymbol(this BaseTypeSyntax value, Document document) => value?.Type.GetTypeSymbol(GetSemanticModel(document));

        internal static ITypeSymbol GetTypeSymbol(this ClassDeclarationSyntax value, Document document) => value?.Identifier.GetSymbol(GetSemanticModel(document)) as ITypeSymbol;

        internal static ITypeSymbol GetTypeSymbol(this RecordDeclarationSyntax value, Document document) => value?.Identifier.GetSymbol(GetSemanticModel(document)) as ITypeSymbol;

        internal static ITypeSymbol GetTypeSymbol(this VariableDeclarationSyntax value, Document document) => value?.Type.GetTypeSymbol(GetSemanticModel(document));

        internal static ITypeSymbol GetTypeSymbol(this TypeSyntax value, Document document)
        {
            if (value is null)
            {
                return null;
            }

            var semanticModel = GetSemanticModel(document);
            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        internal static bool HasMinimumCSharpVersion(this Document document, LanguageVersion wantedVersion) => document.TryGetSyntaxTree(out var syntaxTree) && syntaxTree.HasMinimumCSharpVersion(wantedVersion);

        internal static bool IsConst(this ArgumentSyntax syntax, Document document)
        {
            var identifierName = syntax.Expression.GetName();

            var method = syntax.GetEnclosingMethod(document.GetSemanticModel());
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

        internal static bool IsNullable(this IsPatternExpressionSyntax pattern, Document document) => pattern.Expression.GetSymbol(document) is ITypeSymbol typeSymbol && typeSymbol.IsNullable();

        internal static bool IsEnum(this ArgumentSyntax syntax, Document document)
        {
            var expression = (MemberAccessExpressionSyntax)syntax.Expression;

            if (expression.Expression.GetSymbol(document) is ITypeSymbol type)
            {
                return type.IsEnum();
            }

            return false;
        }

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

        internal static bool IsSeeCrefTaskResult(this SyntaxNode value)
        {
            var type = SyntaxFactory.ParseTypeName("Task<TResult>");
            var member = SyntaxFactory.ParseName(nameof(Task<object>.Result));

            return value.IsSeeCref(type, member);
        }

        internal static bool IsSeeCrefTask(this SyntaxNode value)
        {
            if (value.IsSeeCref(SyntaxFactory.ParseTypeName("Task")))
            {
                return true;
            }

            if (value.IsSeeCref(SyntaxFactory.ParseTypeName("Task<TResult>")))
            {
                return true;
            }

            return false;
        }

        internal static bool IsStringCreation(this SyntaxNode node)
        {
            if (node is BinaryExpressionSyntax b && b.IsKind(SyntaxKind.AddExpression))
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

        internal static AccessorDeclarationSyntax WithSemicolonToken(this AccessorDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.SemicolonToken.AsToken());

        internal static MethodDeclarationSyntax WithSemicolonToken(this MethodDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.SemicolonToken.AsToken());

        internal static PropertyDeclarationSyntax WithSemicolonToken(this PropertyDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.SemicolonToken.AsToken());

        internal static AccessorDeclarationSyntax WithoutSemicolonToken(this AccessorDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.None.AsToken());

        internal static MethodDeclarationSyntax WithoutSemicolonToken(this MethodDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.None.AsToken());

        internal static PropertyDeclarationSyntax WithoutSemicolonToken(this PropertyDeclarationSyntax value) => value.WithSemicolonToken(SyntaxKind.None.AsToken());

        internal static AccessorDeclarationSyntax WithoutExpressionBody(this AccessorDeclarationSyntax value) => value.WithExpressionBody(null).WithoutSemicolonToken();

        internal static MethodDeclarationSyntax WithoutExpressionBody(this MethodDeclarationSyntax value) => value.WithExpressionBody(null).WithoutSemicolonToken();

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
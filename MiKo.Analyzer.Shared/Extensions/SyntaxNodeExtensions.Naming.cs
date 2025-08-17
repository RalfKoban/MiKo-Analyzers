using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
    /// Provides extensions for <see cref="SyntaxNode"/>s that focus on naming.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        private static readonly SyntaxKind[] MethodNameSyntaxKinds = { SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration };

        internal static HashSet<string> GetAllUsedVariables(this SyntaxNode value, SemanticModel semanticModel)
        {
            var dataFlow = semanticModel.AnalyzeDataFlow(value);

            var result = new HashSet<string>();

            // do not use the declared ones as we are interested in parameters, not unused variables
            foreach (var variable in dataFlow.ReadInside)
            {
                result.Add(variable.Name);
            }

            foreach (var variable in dataFlow.ReadOutside)
            {
                result.Add(variable.Name);
            }

            // do not include the ones that are written outside as those are the ones that are not used at all
            foreach (var variable in dataFlow.WrittenInside)
            {
                result.Add(variable.Name);
            }

            return result;
        }

        internal static string GetIdentifierNameFromPropertyExpression(this PropertyDeclarationSyntax value)
        {
            var expression = value.GetPropertyExpression();

            return expression is IdentifierNameSyntax identifier
                   ? identifier.GetName()
                   : null;
        }

        internal static string GetIdentifierName(this ArgumentSyntax value) => value.Expression.GetIdentifierName();

        internal static string GetIdentifierName(this ExpressionSyntax value) => value.GetIdentifierExpression().GetName();

        internal static string GetIdentifierName(this InvocationExpressionSyntax value) => value.GetIdentifierExpression().GetName();

        internal static string GetMethodName(this ParameterSyntax value)
        {
            var enclosingNode = value.GetEnclosing(MethodNameSyntaxKinds);

            switch (enclosingNode)
            {
                case MethodDeclarationSyntax m: return m.GetName();
                case ConstructorDeclarationSyntax c: return c.GetName();
                case ConversionOperatorDeclarationSyntax c: return c.GetName();
                case DestructorDeclarationSyntax d: return d.GetName();
                case OperatorDeclarationSyntax o: return o.GetName();
                default:
                    return null;
            }
        }

        internal static string GetName(this AccessorDeclarationSyntax value)
        {
            var syntaxNode = value.Parent?.Parent;

            switch (syntaxNode)
            {
                case BasePropertyDeclarationSyntax b:
                    return b.GetName();

                case EventFieldDeclarationSyntax ef:
                    return ef.GetName();
            }

            return string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ArgumentSyntax value) => value.Expression.GetName();

        internal static string GetName(this AttributeSyntax value)
        {
            switch (value.Name)
            {
                case SimpleNameSyntax s: return s.GetName();
                case QualifiedNameSyntax q: return q.Right.GetName();
                default: return string.Empty;
            }
        }

        internal static string GetName(this BaseMethodDeclarationSyntax value)
        {
            switch (value)
            {
                case MethodDeclarationSyntax m: return m.GetName();
                case ConstructorDeclarationSyntax c: return c.GetName();
                case ConversionOperatorDeclarationSyntax c: return c.GetName();
                case DestructorDeclarationSyntax d: return d.GetName();
                case OperatorDeclarationSyntax o: return o.GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this BaseTypeDeclarationSyntax value)
        {
            switch (value)
            {
                case TypeDeclarationSyntax s: return s.GetName();
                case EnumDeclarationSyntax s: return s.GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this BaseFieldDeclarationSyntax value)
        {
            switch (value)
            {
                case FieldDeclarationSyntax s: return s.Declaration.Variables.FirstOrDefault().GetName();
                case EventFieldDeclarationSyntax s: return s.Declaration.Variables.FirstOrDefault().GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this BasePropertyDeclarationSyntax value)
        {
            switch (value)
            {
                case PropertyDeclarationSyntax p: return p.GetName();
                case IndexerDeclarationSyntax i: return i.GetName();
                case EventDeclarationSyntax e: return e.GetName();
                default:
                    return string.Empty;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this CatchDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ClassDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ConstructorDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ConversionOperatorDeclarationSyntax value) => value?.OperatorKeyword.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this DestructorDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this EnumDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this EnumMemberDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this EventDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this ExpressionSyntax value)
        {
            switch (value)
            {
                case MemberAccessExpressionSyntax m: return m.GetName();
                case MemberBindingExpressionSyntax b: return b.GetName();
                case InvocationExpressionSyntax i: return i.GetName();
                case LiteralExpressionSyntax l: return l.GetName();
                case TypeSyntax t: return t.GetName();
                default: return string.Empty;
            }
        }

        internal static string GetName(this InvocationExpressionSyntax value)
        {
            switch (value?.Expression)
            {
                case IdentifierNameSyntax identifier:
                {
                    var text = identifier.GetName();

                    if (text is "nameof" && value.Ancestors<MemberAccessExpressionSyntax>().None())
                    {
                        // nameof
                        var arguments = value.ArgumentList.Arguments;

                        if (arguments.Count > 0)
                        {
                            return arguments[0].ToString();
                        }
                    }

                    return text;
                }

                case MemberAccessExpressionSyntax m:
                {
                    return m.GetName();
                }

                case MemberBindingExpressionSyntax b:
                {
                    return b.GetName();
                }
            }

            return string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this IdentifierNameSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this IndexerDeclarationSyntax value) => value?.ThisKeyword.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this InterfaceDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this LiteralExpressionSyntax value) => value?.Token.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this LocalFunctionStatementSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this MemberAccessExpressionSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this MemberBindingExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this MemberDeclarationSyntax value)
        {
            switch (value)
            {
                case BaseTypeDeclarationSyntax s: return s.GetName();
                case BaseMethodDeclarationSyntax s: return s.GetName();
                case BasePropertyDeclarationSyntax s: return s.GetName();
                case BaseFieldDeclarationSyntax s: return s.GetName();
                case EnumMemberDeclarationSyntax s: return s.GetName();
                default:
                    return string.Empty;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this MethodDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this NameColonSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this NameEqualsSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this OperatorDeclarationSyntax value) => value?.OperatorToken.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ParameterSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this PropertyDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this RecordDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this SimpleNameSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this StructDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this TypeDeclarationSyntax value)
        {
            switch (value)
            {
                case ClassDeclarationSyntax s: return s.GetName();
                case InterfaceDeclarationSyntax s: return s.GetName();
                case RecordDeclarationSyntax s: return s.GetName();
                case StructDeclarationSyntax s: return s.GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this TypeSyntax value)
        {
            switch (value)
            {
                case null: return string.Empty;
                case IdentifierNameSyntax i: return i.GetName();
                case SimpleNameSyntax s: return s.GetName();
                default:
                    return value.ToString();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this UsingDirectiveSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this VariableDeclaratorSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlAttributeSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlElementSyntax value) => value?.StartTag.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlEmptyElementSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlElementStartTagSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlElementEndTagSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlNameSyntax value) => value?.LocalName.ValueText;

        internal static IEnumerable<string> GetNames(this BaseFieldDeclarationSyntax value)
        {
            switch (value)
            {
                case FieldDeclarationSyntax s: return s.Declaration.Variables.GetNames();
                case EventFieldDeclarationSyntax s: return s.Declaration.Variables.GetNames();
                default:
                    return Array.Empty<string>();
            }
        }

        internal static string[] GetNames(this InvocationExpressionSyntax value)
        {
            var names = new Stack<string>();

            var expression = value.Expression;

            while (expression is MemberAccessExpressionSyntax maes)
            {
                names.Push(maes.GetName());

                expression = maes.Expression;
            }

            return names.ToArray();
        }

        internal static string GetNameOnlyPart(this TypeSyntax value) => value?.ToString().GetNameOnlyPart();

        internal static string GetNameOnlyPartWithoutGeneric(this TypeSyntax value)
        {
            var type = GetNameLocal();

            return type.GetNameOnlyPart();

            ReadOnlySpan<char> GetNameLocal()
            {
                switch (value)
                {
                    case GenericNameSyntax generic: return generic.GetName().AsSpan();
                    case SimpleNameSyntax simple: return simple.GetName().AsSpan();
                    default:
                        return value is null
                               ? ReadOnlySpan<char>.Empty
                               : value.ToString().AsSpan();
                }
            }
        }

        internal static string GetParameterName(this XmlElementSyntax value)
        {
            var list = value.GetAttributes<XmlNameAttributeSyntax>();

            return list.Count > 0
                       ? list[0].Identifier.GetName()
                       : null;
        }

        internal static string GetParameterName(this XmlEmptyElementSyntax value)
        {
            var attributes = value.Attributes;

            var count = attributes.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    if (attributes[index] is XmlNameAttributeSyntax syntax)
                    {
                        return syntax.Identifier.GetName();
                    }
                }
            }

            return null;
        }

        internal static string[] GetParameterNames(this XmlElementSyntax value)
        {
            foreach (var ancestor in value.Ancestors())
            {
                switch (ancestor)
                {
                    case BaseMethodDeclarationSyntax method:
                        return method.ParameterList.Parameters.ToArray(_ => _.GetName());

                    case IndexerDeclarationSyntax indexer:
                        return indexer.ParameterList.Parameters.ToArray(_ => _.GetName());

                    case BasePropertyDeclarationSyntax property:
                        return property.AccessorList?.Accessors.Any(_ => _.IsKind(SyntaxKind.SetAccessorDeclaration)) is true
                               ? Constants.Names.DefaultPropertyParameterNames
                               : Array.Empty<string>();

                    case BaseTypeDeclarationSyntax _:
                        return Array.Empty<string>();
                }
            }

            return Array.Empty<string>();
        }

        internal static string GetReferencedName(this SyntaxNode node)
        {
            var name = node.GetCref().GetCrefType().GetName();

            if (name.IsNullOrWhiteSpace())
            {
                var nameAttribute = node.GetNameAttribute();

                if (nameAttribute != null)
                {
                    name = nameAttribute.TextTokens[0].ValueText;
                }
            }

            return name;
        }

        internal static string GetXmlTagName(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax ee: return ee.GetName();
                case XmlElementSyntax e: return e.GetName();
                case XmlElementStartTagSyntax est: return est.GetName();
                case XmlNameSyntax n: return n.GetName();
                default: return string.Empty;
            }
        }

        internal static bool HasAttributeName(this MemberDeclarationSyntax value, IEnumerable<string> names)
        {
            var attributeLists = value.AttributeLists;

            for (int i = 0, count = attributeLists.Count; i < count; i++)
            {
                var attributes = attributeLists[i].Attributes;

                for (int index = 0, attributesCount = attributes.Count; index < attributesCount; index++)
                {
                    if (names.Contains(attributes[index].GetName()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
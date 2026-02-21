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
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxNode"/>s that focus on naming.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        private static readonly SyntaxKind[] MethodNameSyntaxKinds = { SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration };

        /// <summary>
        /// Gets all variable names that are used inside the specified <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="value">
        /// The syntax node to analyze.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for data flow analysis.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{T}"/> containing the names of all used variables.
        /// </returns>
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

        /// <summary>
        /// Gets the identifier name from the specified <see cref="ArgumentSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The argument syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the identifier name; or <see langword="null"/> if no name is found.
        /// </returns>
        internal static string GetIdentifierName(this ArgumentSyntax value) => value.Expression.GetIdentifierName();

        /// <summary>
        /// Gets the identifier name from the specified <see cref="ExpressionSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The expression syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the identifier name; or <see langword="null"/> if no name is found.
        /// </returns>
        internal static string GetIdentifierName(this ExpressionSyntax value) => value.GetIdentifierExpression().GetName();

        /// <summary>
        /// Gets the identifier name from the specified <see cref="InvocationExpressionSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The invocation expression syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the identifier name; or <see langword="null"/> if no name is found.
        /// </returns>
        internal static string GetIdentifierName(this InvocationExpressionSyntax value) => value.GetIdentifierExpression().GetName();

        /// <summary>
        /// Gets the identifier name from the property expression of the specified <see cref="PropertyDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The property declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the identifier name; or <see langword="null"/> if no name is found.
        /// </returns>
        internal static string GetIdentifierNameFromPropertyExpression(this PropertyDeclarationSyntax value)
        {
            var expression = value.GetPropertyExpression();

            return expression is IdentifierNameSyntax identifier
                   ? identifier.GetName()
                   : null;
        }

        /// <summary>
        /// Gets the name of the method that contains the specified <see cref="ParameterSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The parameter syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the enclosing method; or <see langword="null"/> if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the property or event that contains the specified <see cref="AccessorDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The accessor declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the containing property or event; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the specified <see cref="ArgumentSyntax"/> expression.
        /// </summary>
        /// <param name="value">
        /// The argument syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the argument expression; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ArgumentSyntax value) => value.Expression.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="AttributeSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The attribute syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the attribute; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
        internal static string GetName(this AttributeSyntax value)
        {
            switch (value.Name)
            {
                case SimpleNameSyntax s: return s.GetName();
                case QualifiedNameSyntax q: return q.Right.GetName();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Gets the name of the specified <see cref="BaseMethodDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The base method declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the method; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the specified <see cref="BaseTypeDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The base type declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the type; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the specified <see cref="BaseFieldDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The base field declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the field; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the specified <see cref="BasePropertyDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The base property declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the property; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the specified <see cref="CatchDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The catch declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the catch variable; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this CatchDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="ClassDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The class declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the class; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ClassDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="ConstructorDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The constructor declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the constructor; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ConstructorDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="ConversionOperatorDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The conversion operator declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the conversion operator; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ConversionOperatorDeclarationSyntax value) => value?.OperatorKeyword.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="DestructorDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The destructor declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the destructor; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this DestructorDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="EnumDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The enum declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the enum; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this EnumDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="EnumMemberDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The enum member declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the enum member; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this EnumMemberDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="EventDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The event declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the event; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this EventDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="ExpressionSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The expression syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the expression; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the specified <see cref="InvocationExpressionSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The invocation expression syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the invocation; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
        internal static string GetName(this InvocationExpressionSyntax value)
        {
            switch (value?.Expression)
            {
                case IdentifierNameSyntax identifier:
                {
                    if (identifier.IsNameOf() && value.Ancestors<MemberAccessExpressionSyntax>().None())
                    {
                        // nameof
                        var arguments = value.ArgumentList.Arguments;

                        if (arguments.Count > 0)
                        {
                            return arguments[0].ToString();
                        }
                    }

                    return identifier.GetName();
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

        /// <summary>
        /// Gets the name of the specified <see cref="IdentifierNameSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The identifier name syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the identifier name; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this IdentifierNameSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="IndexerDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The indexer declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the indexer; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this IndexerDeclarationSyntax value) => value?.ThisKeyword.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="InterfaceDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The interface declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the interface; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this InterfaceDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="LiteralExpressionSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The literal expression syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the value of the literal expression; or <see langword="null"/> if no value is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this LiteralExpressionSyntax value) => value?.Token.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="LocalFunctionStatementSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The local function statement syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the local function; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this LocalFunctionStatementSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="MemberAccessExpressionSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The member access expression syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the member being accessed; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this MemberAccessExpressionSyntax value) => value?.Name.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="MemberBindingExpressionSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The member binding expression syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the member being bound; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this MemberBindingExpressionSyntax value) => value?.Name.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="MemberDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The member declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the member; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the specified <see cref="MethodDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The method declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the method; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this MethodDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="NameColonSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The name colon syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this NameColonSyntax value) => value?.Name.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="NameEqualsSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The name equals syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this NameEqualsSyntax value) => value?.Name.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="OperatorDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The operator declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the operator; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this OperatorDeclarationSyntax value) => value?.OperatorToken.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="ParameterSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The parameter syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the parameter; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ParameterSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="PropertyDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The property declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the property; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this PropertyDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="RecordDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The record declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the record; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this RecordDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="SimpleNameSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The simple name syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the simple name; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this SimpleNameSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="StructDeclarationSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The struct declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the struct; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this StructDeclarationSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="TypeDeclarationSyntax"/>, including all parameters and generic type information.
        /// </summary>
        /// <param name="value">
        /// The type declaration syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the type, including any generic type information; or the <see langword="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the specified <see cref="TypeParameterConstraintClauseSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The type syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the constraint; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
        internal static string GetName(this TypeParameterConstraintClauseSyntax value) => value?.Name.GetName() ?? string.Empty;

        /// <summary>
        /// Gets the name of the specified <see cref="TypeParameterSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The type syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the type parameter; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
        internal static string GetName(this TypeParameterSyntax value) => value?.Identifier.ValueText ?? string.Empty;

        /// <summary>
        /// Gets the name of the specified <see cref="TypeSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The type syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the type; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the name of the specified <see cref="UsingDirectiveSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The using directive syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the using directive; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this UsingDirectiveSyntax value) => value?.Name.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="VariableDeclaratorSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The variable declarator syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the variable; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this VariableDeclaratorSyntax value) => value?.Identifier.ValueText;

        /// <summary>
        /// Gets the name of the specified <see cref="XmlAttributeSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The XML attribute syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the XML attribute; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlAttributeSyntax value) => value?.Name.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="XmlElementSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the XML element; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlElementSyntax value) => value?.StartTag.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="XmlEmptyElementSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The XML empty element syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the XML empty element; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlEmptyElementSyntax value) => value?.Name.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="XmlElementStartTagSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The XML element start tag syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the XML element start tag; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlElementStartTagSyntax value) => value?.Name.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="XmlElementEndTagSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The XML element end tag syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name of the XML element end tag; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlElementEndTagSyntax value) => value?.Name.GetName();

        /// <summary>
        /// Gets the name of the specified <see cref="XmlNameSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The XML name syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the local name of the XML name; or <see langword="null"/> if no name is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlNameSyntax value) => value?.LocalName.ValueText;

        /// <summary>
        /// Gets the name-only part of the specified <see cref="TypeSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The type syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name-only part of the type as a <see cref="string"/>; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
        internal static string GetNameOnlyPart(this TypeSyntax value) => value?.ToString().GetNameOnlyPart();

        /// <summary>
        /// Gets the name-only part of the specified <see cref="TypeSyntax"/>, excluding any generic type information.
        /// </summary>
        /// <param name="value">
        /// The type syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the name-only part of the type without generic information; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the names of the specified <see cref="BaseFieldDeclarationSyntax"/> variables.
        /// </summary>
        /// <param name="value">
        /// The base field declaration syntax.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="string"/> that contains the names of the variables, or an empty array if no names are found.
        /// </returns>
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

        /// <summary>
        /// Gets the names of the chained members in the specified <see cref="InvocationExpressionSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The invocation expression syntax.
        /// </param>
        /// <returns>
        /// An array of names representing the chained members.
        /// </returns>
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

        /// <summary>
        /// Gets the parameter name from the specified <see cref="XmlElementSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the parameter name; or <see langword="null"/> if no name is found.
        /// </returns>
        internal static string GetParameterName(this XmlElementSyntax value)
        {
            var list = value.GetAttributes<XmlNameAttributeSyntax>();

            return list.Count > 0
                   ? list[0].Identifier.GetName()
                   : null;
        }

        /// <summary>
        /// Gets the parameter name from the specified <see cref="XmlEmptyElementSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The XML empty element syntax.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the parameter name; or <see langword="null"/> if no name is found.
        /// </returns>
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

        /// <summary>
        /// Gets the parameter names from the specified <see cref="XmlElementSyntax"/>.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax.
        /// </param>
        /// <returns>
        /// An array of <see cref="string"/> containing the parameter names, or an empty array if no names are found.
        /// </returns>
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

        /// <summary>
        /// Gets the referenced name from the specified <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="value">
        /// The syntax node.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the referenced name; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
        internal static string GetReferencedName(this SyntaxNode value)
        {
            var name = value.GetCref().GetCrefType().GetName();

            if (name.IsNullOrWhiteSpace())
            {
                var nameAttribute = value.GetNameAttribute();

                if (nameAttribute != null)
                {
                    name = nameAttribute.TextTokens[0].ValueText;
                }
            }

            return name;
        }

        /// <summary>
        /// Gets the XML tag name from the specified <see cref="SyntaxNode"/>.
        /// </summary>
        /// <param name="value">
        /// The syntax node.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the XML tag name; or the <see cref="string.Empty"/> string ("") if no name is found.
        /// </returns>
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

        /// <summary>
        /// Determines whether the specified <see cref="MemberDeclarationSyntax"/> has an attribute with a name contained in the provided collection.
        /// </summary>
        /// <param name="value">
        /// The member declaration syntax.
        /// </param>
        /// <param name="names">
        /// The collection of attribute names to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if an attribute with a matching name exists; otherwise, <see langword="false"/>.
        /// </returns>
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
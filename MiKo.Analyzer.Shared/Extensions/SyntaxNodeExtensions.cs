using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;

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
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxNode"/>s.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        private static readonly SyntaxList<TypeParameterConstraintClauseSyntax> EmptyConstraintClauses = SyntaxFactory.List<TypeParameterConstraintClauseSyntax>();

        /// <summary>
        /// Determines whether the specified value contains the given character.
        /// </summary>
        /// <param name="value">
        /// The syntax node to inspect.
        /// </param>
        /// <param name="c">
        /// The character to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="string"/> representation of the node contains the character; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Contains(this SyntaxNode value, in char c) => value?.ToString().Contains(c) ?? false;

        /// <summary>
        /// Determines whether the enclosing method of a syntax node has a parameter with the specified name.
        /// </summary>
        /// <param name="value">
        /// The syntax node to inspect.
        /// </param>
        /// <param name="parameterName">
        /// The name of the parameter to seek.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the enclosing method has a parameter with the specified name; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool EnclosingMethodHasParameter(this SyntaxNode value, string parameterName, SemanticModel semanticModel)
        {
            var method = value.GetEnclosingMethod(semanticModel);

            if (method is null)
            {
                return false;
            }

            var parameters = method.Parameters;

            return parameters.Length > 0 && parameters.Any(_ => _.Name == parameterName);
        }

        /// <summary>
        /// Finds the token at the location specified by the diagnostic issue.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to search in.
        /// </param>
        /// <param name="issue">
        /// The diagnostic issue that provides the location.
        /// </param>
        /// <returns>
        /// The syntax token at the specified position.
        /// </returns>
        internal static SyntaxToken FindToken<T>(this T value, Diagnostic issue) where T : SyntaxNode
        {
            var position = issue.Location.SourceSpan.Start;
            var token = value.FindToken(position, true);

            return token;
        }

        /// <summary>
        /// Gets the constraint clauses associated with the specified type parameter constraint clause.
        /// </summary>
        /// <param name="value">
        /// The type parameter constraint clause to get constraint clauses for.
        /// </param>
        /// <returns>
        /// A collection of type parameter constraint clauses.
        /// </returns>
        internal static SyntaxList<TypeParameterConstraintClauseSyntax> GetConstraintClauses(this TypeParameterConstraintClauseSyntax value)
        {
            switch (value.Parent)
            {
                case ClassDeclarationSyntax c: return c.ConstraintClauses;
                case InterfaceDeclarationSyntax i: return i.ConstraintClauses;
                case RecordDeclarationSyntax r: return r.ConstraintClauses;
                case StructDeclarationSyntax s: return s.ConstraintClauses;
                case MethodDeclarationSyntax b: return b.ConstraintClauses;
                case LocalFunctionStatementSyntax f: return f.ConstraintClauses;

                default:
                    return EmptyConstraintClauses;
            }
        }

        /// <summary>
        /// Gets the reference token for a type parameter constraint.
        /// </summary>
        /// <param name="value">
        /// The type parameter constraint clause to get the reference token for.
        /// </param>
        /// <returns>
        /// The syntax token that represents the reference for the constraint.
        /// </returns>
        internal static SyntaxToken GetTypeParameterConstraintReferenceToken(this TypeParameterConstraintClauseSyntax value)
        {
            switch (value.Parent)
            {
//// ReSharper disable PossibleNullReferenceException Cannot be null as there is already a type parameter constraint
                case ClassDeclarationSyntax c: return c.TypeParameterList.GreaterThanToken;
                case InterfaceDeclarationSyntax i: return i.TypeParameterList.GreaterThanToken;
                case RecordDeclarationSyntax r: return r.TypeParameterList.GreaterThanToken;
                case StructDeclarationSyntax s: return s.TypeParameterList.GreaterThanToken;
//// ReSharper restore PossibleNullReferenceException

                case MethodDeclarationSyntax b: return b.ParameterList.CloseParenToken;
                case LocalFunctionStatementSyntax f: return f.ParameterList.CloseParenToken;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Gets the get accessor of a property declaration.
        /// </summary>
        /// <param name="value">
        /// The property declaration to get the accessor from.
        /// </param>
        /// <returns>
        /// The get accessor declaration, or <see langword="null"/> if there is none.
        /// </returns>
        internal static AccessorDeclarationSyntax GetGetter(this PropertyDeclarationSyntax value) => value?.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.GetAccessorDeclaration);

        /// <summary>
        /// Gets the set accessor of a property declaration.
        /// </summary>
        /// <param name="value">
        /// The property declaration to get the accessor from.
        /// </param>
        /// <returns>
        /// The set accessor declaration, or <see langword="null"/> if there is none.
        /// </returns>
        internal static AccessorDeclarationSyntax GetSetter(this PropertyDeclarationSyntax value) => value?.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.SetAccessorDeclaration);

        /// <summary>
        /// Gets the line span for a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the line span for.
        /// </param>
        /// <returns>
        /// The file line position span of the syntax node.
        /// </returns>
        internal static FileLinePositionSpan GetLineSpan(this SyntaxNode value) => value.GetLocation().GetLineSpan();

        /// <summary>
        /// Gets the parameter comment from a documentation comment.
        /// </summary>
        /// <param name="value">
        /// The documentation comment to search in.
        /// </param>
        /// <param name="parameterName">
        /// The name of the parameter to seek the comment for.
        /// </param>
        /// <returns>
        /// The XML element that contains the parameter comment, or <see langword="null"/> if none exists.
        /// </returns>
        internal static XmlElementSyntax GetParameterComment(this DocumentationCommentTriviaSyntax value, string parameterName) => value.FirstDescendant<XmlElementSyntax>(_ => _.GetName() is Constants.XmlTag.Param && _.GetParameterName() == parameterName);

        /// <summary>
        /// Gets the expression associated with a property declaration.
        /// </summary>
        /// <param name="value">
        /// The property declaration to get the expression from.
        /// </param>
        /// <returns>
        /// The expression of the property, or <see langword="null"/> if none exists.
        /// </returns>
        internal static ExpressionSyntax GetPropertyExpression(this PropertyDeclarationSyntax value)
        {
            var expression = value.ExpressionBody?.Expression;

            if (expression != null)
            {
                return expression;
            }

            var accessorList = value.AccessorList;

            if (accessorList != null)
            {
                var getter = accessorList.Accessors[0];

                expression = getter.ExpressionBody?.Expression;

                if (expression != null)
                {
                    return expression;
                }

                if (getter.Body?.Statements.FirstOrDefault() is ReturnStatementSyntax r)
                {
                    return r.Expression;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the related if statement for a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the related if statement for.
        /// </param>
        /// <returns>
        /// The related if statement, or <see langword="null"/> if none exists.
        /// </returns>
        internal static IfStatementSyntax GetRelatedIfStatement(this SyntaxNode value)
        {
            var ifStatement = value.FirstAncestorOrSelf<IfStatementSyntax>();

            if (ifStatement is null)
            {
                // maybe part of a block outside the if statement
                var block = value.FirstAncestorOrSelf<BlockSyntax>();

                if (block != null)
                {
                    // try to find the corresponding if statement
                    ifStatement = block.FirstChild<IfStatementSyntax>();
                }
            }

            return ifStatement;
        }

        /// <summary>
        /// Gets the related condition for a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the related condition for.
        /// </param>
        /// <returns>
        /// The related condition expression, or <see langword="null"/> if none exists.
        /// </returns>
        internal static ExpressionSyntax GetRelatedCondition(this SyntaxNode value)
        {
            var coalesceExpression = value.FirstAncestorOrSelf<BinaryExpressionSyntax>(_ => _.IsKind(SyntaxKind.CoalesceExpression));

            if (coalesceExpression != null)
            {
                return coalesceExpression;
            }

            // most probably it's a if/else, but it might be a switch statement as well
            var condition = value.GetRelatedIfStatement()?.Condition ?? value.GetEnclosing<SwitchStatementSyntax>()?.Expression;

            return condition;
        }

        /// <summary>
        /// Gets the syntax node that swallows an exception.
        /// </summary>
        /// <param name="value">
        /// The object creation expression to analyze.
        /// </param>
        /// <param name="semanticModelCallback">
        /// A callback that provides the semantic model for analysis.
        /// </param>
        /// <returns>
        /// The syntax node that swallows an exception, or <see langword="null"/> if none exists.
        /// </returns>
        internal static SyntaxNode GetExceptionSwallowingNode(this ObjectCreationExpressionSyntax value, Func<SemanticModel> semanticModelCallback)
        {
            var catchClause = value.FirstAncestorOrSelf<CatchClauseSyntax>();

            if (catchClause != null)
            {
                // we found an exception inside a catch block that does not get the caught exception as inner exception
                return catchClause;
            }

            // inspect any 'if' or 'switch' or 'else if' to see if there is an exception involved
            var expression = value.GetRelatedCondition()?.FirstDescendant<ExpressionSyntax>(_ => _.GetTypeSymbol(semanticModelCallback())?.IsException() is true);

            if (expression != null)
            {
                return expression;
            }

            // inspect method arguments
            var parameter = value.GetEnclosing<MethodDeclarationSyntax>()?.ParameterList.Parameters.FirstOrDefault(_ => _.Type.IsException());

            return parameter;
        }

        /// <summary>
        /// Gets the parameter that is used in an object creation expression.
        /// </summary>
        /// <param name="value">
        /// The object creation expression to analyze.
        /// </param>
        /// <returns>
        /// The parameter that is used, or <see langword="null"/> if none is used.
        /// </returns>
        internal static ParameterSyntax GetUsedParameter(this ObjectCreationExpressionSyntax value)
        {
            var parameters = CollectParameters(value);

            if (parameters.Any())
            {
                // there might be multiple parameters, so we have to find out which parameter is meant
                var condition = GetRelatedCondition(value);

                if (condition is null)
                {
                    // nothing found
                    return null;
                }

                var identifiers = condition.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().ToHashSet(_ => _.GetName());

                return parameters.FirstOrDefault(_ => identifiers.Contains(_.GetName()));
            }

            return null;
        }

        /// <summary>
        /// Gets attributes of a specific type from an XML element.
        /// </summary>
        /// <typeparam name="T">
        /// The type of XML attribute to retrieve.
        /// </typeparam>
        /// <param name="value">
        /// The XML element to get attributes from.
        /// </param>
        /// <returns>
        /// A collection of attributes of the specified type.
        /// </returns>
        internal static IReadOnlyList<T> GetAttributes<T>(this XmlElementSyntax value) where T : XmlAttributeSyntax
        {
            return value?.StartTag.Attributes.OfType<XmlAttributeSyntax, T>() ?? Array.Empty<T>();
        }

        /// <summary>
        /// Gets attributes of a specific type from an XML empty element.
        /// </summary>
        /// <typeparam name="T">
        /// The type of XML attribute to retrieve.
        /// </typeparam>
        /// <param name="value">
        /// The XML empty element to get attributes from.
        /// </param>
        /// <returns>
        /// A collection of attributes of the specified type.
        /// </returns>
        internal static IReadOnlyList<T> GetAttributes<T>(this XmlEmptyElementSyntax value) where T : XmlAttributeSyntax
        {
            return value?.Attributes.OfType<XmlAttributeSyntax, T>() ?? Array.Empty<T>();
        }

        /// <summary>
        /// Gets the identifier expression from an expression syntax.
        /// </summary>
        /// <param name="value">
        /// The expression to extract the identifier from.
        /// </param>
        /// <returns>
        /// The identifier expression, or <see langword="null"/> if none exists.
        /// </returns>
        internal static ExpressionSyntax GetIdentifierExpression(this ExpressionSyntax value)
        {
            switch (value)
            {
                case InvocationExpressionSyntax invocation:
                    return invocation.GetIdentifierExpression();

                case IdentifierNameSyntax identifier:
                    return identifier;

                case MemberAccessExpressionSyntax maes:
                    return maes.Expression;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the identifier expression from an invocation expression syntax.
        /// </summary>
        /// <param name="value">
        /// The invocation expression to extract the identifier from.
        /// </param>
        /// <returns>
        /// The identifier expression, or <see langword="null"/> if none exists.
        /// </returns>
        internal static ExpressionSyntax GetIdentifierExpression(this InvocationExpressionSyntax value)
        {
            switch (value?.Expression)
            {
                case MemberAccessExpressionSyntax maes:
                    return maes.Expression;

                case MemberBindingExpressionSyntax mbes: // find parent conditional access expression as that contains the identifier
                    return mbes.FirstAncestor<ConditionalAccessExpressionSyntax>()?.Expression;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the type arguments from a member access expression.
        /// </summary>
        /// <param name="value">
        /// The member access expression to extract types from.
        /// </param>
        /// <returns>
        /// An array of type syntax nodes representing the type arguments.
        /// </returns>
        internal static TypeSyntax[] GetTypes(this MemberAccessExpressionSyntax value)
        {
            if (value.Name is GenericNameSyntax generic)
            {
                return generic.TypeArgumentList.Arguments.ToArray();
            }

            return Array.Empty<TypeSyntax>();
        }

        /// <summary>
        /// Gets the type arguments from an invocation expression.
        /// </summary>
        /// <param name="value">
        /// The invocation expression to extract types from.
        /// </param>
        /// <returns>
        /// An array of type syntax nodes representing the type arguments.
        /// </returns>
        internal static TypeSyntax[] GetTypes(this InvocationExpressionSyntax value)
        {
            var types = new List<TypeSyntax>();

            var expression = value.Expression;

            while (expression is MemberAccessExpressionSyntax maes)
            {
                types.AddRange(maes.GetTypes());

                expression = maes.Expression;
            }

            return types.ToArray();
        }

        /// <summary>
        /// Gets the semicolon token from a statement.
        /// </summary>
        /// <param name="value">
        /// The statement to get the semicolon token from.
        /// </param>
        /// <returns>
        /// The semicolon token of the statement, or a default token if not applicable.
        /// </returns>
        internal static SyntaxToken GetSemicolonToken(this StatementSyntax value)
        {
            switch (value)
            {
                case LocalDeclarationStatementSyntax l: return l.SemicolonToken;
                case ExpressionStatementSyntax e: return e.SemicolonToken;
                case ReturnStatementSyntax r: return r.SemicolonToken;
                case ThrowStatementSyntax t: return t.SemicolonToken;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Gets the semicolon token from a member declaration.
        /// </summary>
        /// <param name="value">
        /// The member declaration to get the semicolon token from.
        /// </param>
        /// <returns>
        /// The semicolon token of the declaration, or a default token if not applicable.
        /// </returns>
        internal static SyntaxToken GetSemicolonToken(this MemberDeclarationSyntax value)
        {
            switch (value)
            {
                case BaseMethodDeclarationSyntax m: return m.SemicolonToken;
                case PropertyDeclarationSyntax p: return p.SemicolonToken;
                case EventDeclarationSyntax e: return e.SemicolonToken;
                case EventFieldDeclarationSyntax ef: return ef.SemicolonToken;
                case FieldDeclarationSyntax f: return f.SemicolonToken;

                default:
                    return default;
            }
        }

        /// <summary>
        /// Gets the symbol associated with a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The symbol associated with the syntax node, or <see langword="null"/> if none exists.
        /// </returns>
        internal static ISymbol GetSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            var symbolInfo = GetSymbolInfo();

            var symbol = symbolInfo.Symbol;

            if (symbol is null)
            {
                if (symbolInfo.CandidateReason is CandidateReason.OverloadResolutionFailure)
                {
                    // we did not find the symbol, so we take the first one, assuming that this is the right one
                    return symbolInfo.CandidateSymbols.FirstOrDefault();
                }
            }

            return symbol;

            SymbolInfo GetSymbolInfo()
            {
                switch (value)
                {
                    case ConstructorInitializerSyntax cis:
                        return semanticModel.GetSymbolInfo(cis);

                    default:
                        return semanticModel.GetSymbolInfo(value);
                }
            }
        }

        /// <summary>
        /// Gets the symbol associated with a syntax node using the specified compilation.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the symbol for.
        /// </param>
        /// <param name="compilation">
        /// The compilation to use for analysis.
        /// </param>
        /// <returns>
        /// The symbol associated with the syntax node, or <see langword="null"/> if none exists.
        /// </returns>
        internal static ISymbol GetSymbol(this SyntaxNode value, Compilation compilation) => value?.GetSymbol(compilation.GetSemanticModel(value.SyntaxTree));

        /// <summary>
        /// Gets the method symbol for a local function statement.
        /// </summary>
        /// <param name="value">
        /// The local function statement to get the symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The method symbol associated with the local function.
        /// </returns>
        internal static IMethodSymbol GetSymbol(this LocalFunctionStatementSyntax value, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetDeclaredSymbol(value);

#if VS2022
            return symbol;
#else
            return symbol as IMethodSymbol;
#endif
        }

        /// <summary>
        /// Gets the type symbol of an argument using the specified compilation.
        /// </summary>
        /// <param name="value">
        /// The argument to get the type symbol for.
        /// </param>
        /// <param name="compilation">
        /// The compilation to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the argument, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this ArgumentSyntax value, Compilation compilation) => value?.GetTypeSymbol(compilation.GetSemanticModel(value.SyntaxTree));

        /// <summary>
        /// Gets the type symbol of an argument using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The argument to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the argument, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this ArgumentSyntax value, SemanticModel semanticModel) => value?.Expression.GetTypeSymbol(semanticModel);

        /// <summary>
        /// Gets the type symbol of an expression using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The expression to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the expression, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value is null)
            {
                return null;
            }

            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        /// <summary>
        /// Gets the type symbol of a member access expression using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The member access expression to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the member access expression, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this MemberAccessExpressionSyntax value, SemanticModel semanticModel) => value?.Expression.GetTypeSymbol(semanticModel);

        /// <summary>
        /// Gets the type symbol of a type syntax using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The type syntax to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the type, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this TypeSyntax value, SemanticModel semanticModel)
        {
            if (value is null)
            {
                return null;
            }

            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        /// <summary>
        /// Gets the type symbol of a base type syntax using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The base type syntax to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the base type, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this BaseTypeSyntax value, SemanticModel semanticModel) => value?.Type.GetTypeSymbol(semanticModel);

        /// <summary>
        /// Gets the type symbol of a class declaration using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The class declaration to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the class, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this ClassDeclarationSyntax value, SemanticModel semanticModel) => value?.Identifier.GetSymbol(semanticModel) as ITypeSymbol;

        /// <summary>
        /// Gets the type symbol of a record declaration using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The record declaration to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the record, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this RecordDeclarationSyntax value, SemanticModel semanticModel) => value?.Identifier.GetSymbol(semanticModel) as ITypeSymbol;

        /// <summary>
        /// Gets the type symbol of a variable declaration using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The variable declaration to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the variable declaration, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this VariableDeclarationSyntax value, SemanticModel semanticModel) => value?.Type.GetTypeSymbol(semanticModel);

        /// <summary>
        /// Gets the type symbol of a variable designation using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The variable designation to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the variable designation, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this VariableDesignationSyntax value, SemanticModel semanticModel)
        {
            if (value is null)
            {
                return null;
            }

            if (semanticModel.GetDeclaredSymbol(value) is ILocalSymbol symbol)
            {
                return symbol.Type;
            }

            return null;
        }

        /// <summary>
        /// Gets the type symbol of a syntax node using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the type symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// The type symbol of the syntax node, or <see langword="null"/> if not available.
        /// </returns>
        internal static ITypeSymbol GetTypeSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            switch (value)
            {
                case null: return null;
                case ArgumentSyntax a: return a.GetTypeSymbol(semanticModel);
                case MemberAccessExpressionSyntax m: return m.GetTypeSymbol(semanticModel);
                case ClassDeclarationSyntax c: return c.GetTypeSymbol(semanticModel);
                case RecordDeclarationSyntax r: return r.GetTypeSymbol(semanticModel);
                case BaseTypeSyntax b: return b.GetTypeSymbol(semanticModel);
                case TypeSyntax t: return t.GetTypeSymbol(semanticModel);
                case VariableDeclarationSyntax declaration: return declaration.GetTypeSymbol(semanticModel);
                case ParenthesizedVariableDesignationSyntax p: return p.Parent.GetTypeSymbol(semanticModel); // TODO RKN: seems we have an parenthesize around
                case VariableDesignationSyntax designation: return designation.GetTypeSymbol(semanticModel);
                case DeclarationPatternSyntax dp: return dp.Type.GetTypeSymbol(semanticModel);
                case DeclarationExpressionSyntax de: return de.Type.GetTypeSymbol(semanticModel);
                case ExpressionSyntax e: return e.GetTypeSymbol(semanticModel);

                default:
                {
                    var typeInfo = semanticModel.GetTypeInfo(value);

                    return typeInfo.Type;
                }
            }
        }

        /// <summary>
        /// Determines whether the syntax tree has a minimum C# language version.
        /// </summary>
        /// <param name="value">
        /// The syntax tree to check.
        /// </param>
        /// <param name="expectedVersion">
        /// One of the enumeration members that specifies the minimum expected C# language version.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax tree's language version is greater than or equal to the expected version; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasMinimumCSharpVersion(this SyntaxTree value, LanguageVersion expectedVersion)
        {
            var languageVersion = ((CSharpParseOptions)value.Options).LanguageVersion;

            // ignore the latest versions (or above)
            return languageVersion >= expectedVersion && expectedVersion < LanguageVersion.LatestMajor;
        }

        /// <summary>
        /// Determines whether a syntax node has a LINQ extension method.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node has any LINQ extension methods; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasLinqExtensionMethod(this SyntaxNode value, SemanticModel semanticModel) => value.LinqExtensionMethods(semanticModel).Any();

#if VS2022

        /// <summary>
        /// Determines whether a class declaration has a primary constructor.
        /// </summary>
        /// <param name="value">
        /// The class declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the class declaration has a primary constructor; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasPrimaryConstructor(this ClassDeclarationSyntax value) => value.ParameterList != null;

        /// <summary>
        /// Determines whether a struct declaration has a primary constructor.
        /// </summary>
        /// <param name="value">
        /// The struct declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the struct declaration has a primary constructor; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasPrimaryConstructor(this StructDeclarationSyntax value) => value.ParameterList != null;

#endif

        /// <summary>
        /// Determines whether a record declaration has a primary constructor.
        /// </summary>
        /// <param name="value">
        /// The record declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the record declaration has a primary constructor; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasPrimaryConstructor(this RecordDeclarationSyntax value) => value.ParameterList != null;

        /// <summary>
        /// Determines whether an arrow expression clause is part of a method that returns a value.
        /// </summary>
        /// <param name="value">
        /// The arrow expression clause to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the arrow expression clause is part of a method that returns a value; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasReturnValue(this ArrowExpressionClauseSyntax value) => value?.Parent is BaseMethodDeclarationSyntax method && method.HasReturnValue();

        /// <summary>
        /// Determines whether a method declaration returns a value.
        /// </summary>
        /// <param name="value">
        /// The method declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method returns a value; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasReturnValue(this BaseMethodDeclarationSyntax value)
        {
            switch (value)
            {
                case MethodDeclarationSyntax m: return m.ReturnType?.IsVoid() is false;
                case OperatorDeclarationSyntax o: return o.ReturnType?.IsVoid() is false;
                case ConversionOperatorDeclarationSyntax c: return c.Type?.IsVoid() is false;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Inserts a new syntax node after the specified node in a list.
        /// </summary>
        /// <typeparam name="TRoot">
        /// The type of the root node.
        /// </typeparam>
        /// <param name="value">
        /// The root node that contains the list.
        /// </param>
        /// <param name="nodeInList">
        /// The node in the list after which to insert.
        /// </param>
        /// <param name="newNode">
        /// The new node to insert.
        /// </param>
        /// <returns>
        /// A new root node with the inserted node.
        /// </returns>
        internal static TRoot InsertNodeAfter<TRoot>(this TRoot value, SyntaxNode nodeInList, SyntaxNode newNode) where TRoot : SyntaxNode
        {
            return value.InsertNodesAfter(nodeInList, new[] { newNode });
        }

        /// <summary>
        /// Inserts a new syntax node before the specified node in a list.
        /// </summary>
        /// <typeparam name="TRoot">
        /// The type of the root node.
        /// </typeparam>
        /// <param name="value">
        /// The root node that contains the list.
        /// </param>
        /// <param name="nodeInList">
        /// The node in the list before which to insert.
        /// </param>
        /// <param name="newNode">
        /// The new node to insert.
        /// </param>
        /// <returns>
        /// A new root node with the inserted node.
        /// </returns>
        internal static TRoot InsertNodeBefore<TRoot>(this TRoot value, SyntaxNode nodeInList, SyntaxNode newNode) where TRoot : SyntaxNode
        {
            // method needs to be indented and a CRLF needs to be added
            var modifiedNode = newNode.WithIndentation().WithEndOfLine();

            return value.InsertNodesBefore(nodeInList, new[] { modifiedNode });
        }

        /// <summary>
        /// Determines whether a syntax node is of the specified syntax kind.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node is of the specified kind; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsKind(this SyntaxNode value, in SyntaxKind kind) => value?.RawKind == (int)kind;

        /// <summary>
        /// Determines whether a syntax node is of the specified syntax kinds.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="kinds">
        /// A set of syntax kinds to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node is of the specified kinds; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAnyKind(this SyntaxNode value, ISet<SyntaxKind> kinds) => kinds.Contains(value.Kind());

        /// <summary>
        /// Determines whether a syntax node is of the specified syntax kinds.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="kinds">
        /// A span of syntax kinds to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node is of the specified kinds; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAnyKind(this SyntaxNode value, in ReadOnlySpan<SyntaxKind> kinds)
        {
            var kindsLength = kinds.Length;

            if (kindsLength > 0)
            {
                var valueKind = value.RawKind;

                if (kindsLength is 2)
                {
                    return valueKind == (int)kinds[0] || valueKind == (int)kinds[1];
                }

                for (var index = 0; index < kindsLength; index++)
                {
                    if (valueKind == (int)kinds[index])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether a type syntax represents a boolean type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents a boolean type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsBoolean(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax predefined)
            {
                return predefined.Keyword.IsKind(SyntaxKind.BoolKeyword);
            }

            switch (value?.ToString())
            {
                case nameof(Boolean):
                case nameof(System) + "." + nameof(Boolean):
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a type syntax represents a <see cref="byte"/> type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents a <see cref="byte"/> type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsByte(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax predefined)
            {
                return predefined.Keyword.IsKind(SyntaxKind.ByteKeyword);
            }

            switch (value?.ToString())
            {
                case nameof(Byte):
                case nameof(System) + "." + nameof(Byte):
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a type syntax represents a <see cref="Guid"/> type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents a <see cref="Guid"/> type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsGuid(this TypeSyntax value)
        {
            switch (value?.ToString())
            {
                case nameof(Guid):
                case nameof(Guid) + "?":
                case nameof(System) + "." + nameof(Guid):
                case nameof(System) + "." + nameof(Guid) + "?":
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a syntax node is inside any of the specified syntax kinds.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="kinds">
        /// A set of syntax kinds to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node is inside any of the specified kinds; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsInside(this SyntaxNode value, ISet<SyntaxKind> kinds)
        {
            foreach (var ancestor in value.Ancestors())
            {
                var kind = ancestor.Kind();

                if (kinds.Contains(kind))
                {
                    return true;
                }

                switch (kind)
                {
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.EventFieldDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                        return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an expression syntax represents a nameof expression.
        /// </summary>
        /// <param name="value">
        /// The expression syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression represents a nameof expression; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsNameOf(this ExpressionSyntax value)
        {
            switch (value)
            {
                case IdentifierNameSyntax identifier:
                    return identifier.IsNameOf();

                case InvocationExpressionSyntax invocation:
                    return invocation.IsNameOf();

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether an identifier name syntax represents a nameof keyword.
        /// </summary>
        /// <param name="value">
        /// The identifier name syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the identifier represents a nameof keyword; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsNameOf(this IdentifierNameSyntax value) => value.GetName() is "nameof";

        /// <summary>
        /// Determines whether an invocation expression syntax represents a nameof expression.
        /// </summary>
        /// <param name="value">
        /// The invocation expression syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the invocation represents a nameof expression; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsNameOf(this InvocationExpressionSyntax value) => value.Expression.IsNameOf();

        /// <summary>
        /// Determines whether a syntax node is the only node inside a region.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is the only one inside a region; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnlyNodeInsideRegion(this SyntaxNode value)
        {
            if (value.TryGetRegionDirective(out var regionTrivia))
            {
                var relatedDirectives = regionTrivia.GetRelatedDirectives();

                if (relatedDirectives.Count is 2)
                {
                    var endRegionTrivia = relatedDirectives[1];

                    var otherSyntaxNode = endRegionTrivia.ParentTrivia.Token.Parent;

                    if (otherSyntaxNode != null)
                    {
                        if (otherSyntaxNode.IsEquivalentTo(value.NextSibling()))
                        {
                            return true;
                        }

                        if (otherSyntaxNode.IsEquivalentTo(value.Parent))
                        {
                            // seems like same type
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an invocation expression is a Moq <c>It.Is</c> condition matcher.
        /// </summary>
        /// <param name="value">
        /// The invocation expression to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression is a Moq <c>It.Is</c> condition matcher; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsMoqItIsConditionMatcher(this InvocationExpressionSyntax value) => value.Expression is MemberAccessExpressionSyntax maes
                                                                                              && maes.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                                                                                              && maes.Expression is IdentifierNameSyntax invokedType
                                                                                              && invokedType.GetName() is Constants.Moq.ConditionMatcher.It
                                                                                              && maes.GetName() is Constants.Moq.ConditionMatcher.Is;

        /// <summary>
        /// Determines whether a member access expression is inside a Moq call.
        /// </summary>
        /// <param name="value">
        /// The member access expression to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression is inside a Moq call; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsInsideMoqCall(this MemberAccessExpressionSyntax value)
        {
            if (value.Parent is InvocationExpressionSyntax i && i.Parent is LambdaExpressionSyntax lambda)
            {
                return IsMoqCall(lambda);
            }

            return false;
        }

        /// <summary>
        /// Determines whether a lambda expression is a Moq call.
        /// </summary>
        /// <param name="value">
        /// The lambda expression to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the lambda expression is a Moq call; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsMoqCall(this LambdaExpressionSyntax value)
        {
            if (value.Parent is ArgumentSyntax a && a.Parent?.Parent is InvocationExpressionSyntax i && i.Expression is MemberAccessExpressionSyntax m)
            {
                switch (m.GetName())
                {
                    case Constants.Moq.Setup:
                    case Constants.Moq.SetupGet:
                    case Constants.Moq.SetupSet:
                    case Constants.Moq.SetupSequence:
                    case Constants.Moq.Verify:
                    case Constants.Moq.VerifyGet:
                    case Constants.Moq.VerifySet:
                    case Constants.Moq.Of when m.Expression is IdentifierNameSyntax ins && ins.GetName() is Constants.Moq.Mock:
                    {
                        // here we assume that we have a Moq call
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to get Moq types from a member access expression.
        /// </summary>
        /// <param name="value">
        /// The member access expression to analyze.
        /// </param>
        /// <param name="result">
        /// On successful return, contains the array of type syntax nodes if found; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if Moq types were found; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool TryGetMoqTypes(this MemberAccessExpressionSyntax value, out TypeSyntax[] result)
        {
            result = null;

            if (value.GetName() is Constants.Moq.Object)
            {
                var expression = value.Expression.WithoutParenthesis(); // let's see if we can fix it in case we remove the surrounding parenthesis

                if (expression is ObjectCreationExpressionSyntax o && o.Type.GetNameOnlyPartWithoutGeneric() is Constants.Moq.Mock && o.Type is GenericNameSyntax genericName)
                {
                    result = genericName.TypeArgumentList.Arguments.ToArray();
                }
            }

            return result != null;
        }

        /// <summary>
        /// Determines whether a statement is an assignment of the specified identifier.
        /// </summary>
        /// <param name="value">
        /// The statement to check.
        /// </param>
        /// <param name="identifierName">
        /// The name of the identifier to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the statement is an assignment of the specified identifier; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAssignmentOf(this StatementSyntax value, string identifierName) => value is ExpressionStatementSyntax e
                                                                                               && e.Expression is AssignmentExpressionSyntax a
                                                                                               && a.IsKind(SyntaxKind.SimpleAssignmentExpression)
                                                                                               && a.Left is IdentifierNameSyntax i
                                                                                               && i.GetName() == identifierName;

        /// <summary>
        /// Determines whether a type syntax represents a command type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type represents a command; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsCommand(this TypeSyntax value, SemanticModel semanticModel)
        {
            if (value is PredefinedTypeSyntax)
            {
                return false;
            }

            var name = value.ToString();

            if (name.Contains(Constants.Names.Command) && name.Contains("CommandManager") is false)
            {
                return semanticModel.LookupSymbols(value.GetLocation().SourceSpan.Start, name: name).FirstOrDefault() is ITypeSymbol symbol && symbol.IsCommand();
            }

            return false;
        }

        /// <summary>
        /// Determines whether a field declaration has the const modifier.
        /// </summary>
        /// <param name="value">
        /// The field declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the field declaration has the const modifier; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsConst(this FieldDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.ConstKeyword);

        /// <summary>
        /// Determines whether a syntax node represents a constant value.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a constant value; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsConst(this SyntaxNode value, in SyntaxNodeAnalysisContext context)
        {
            switch (value)
            {
                case IdentifierNameSyntax i: return i.IsConst(context);
                case MemberAccessExpressionSyntax m: return m.IsConst(context.SemanticModel);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether an identifier name represents a constant field of the specified type.
        /// </summary>
        /// <param name="value">
        /// The identifier name to check.
        /// </param>
        /// <param name="type">
        /// The type to seek the constant.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the identifier name represents a constant field; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsConst(this IdentifierNameSyntax value, ITypeSymbol type)
        {
            var isConst = type.GetFields(value.GetName()).Any(_ => _.IsConst);

            return isConst;
        }

        /// <summary>
        /// Determines whether an identifier name represents a constant field in the containing type.
        /// </summary>
        /// <param name="value">
        /// The identifier name to check.
        /// </param>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the identifier name represents a constant field; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsConst(this IdentifierNameSyntax value, in SyntaxNodeAnalysisContext context) => value.IsConst(context.FindContainingType());

        /// <summary>
        /// Determines whether a member access expression represents a constant field.
        /// </summary>
        /// <param name="value">
        /// The member access expression to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the member access expression represents a constant field; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsConst(this MemberAccessExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var type = value.GetTypeSymbol(semanticModel);

                if (type is null)
                {
                    // we do not know, so we assume it's not
                    return false;
                }

                if (type.IsEnum())
                {
                    // only get the real enum members, no local variables or something
                    return true;
                }

                if (value.Name is IdentifierNameSyntax identifierName)
                {
                    // find out whether the identifier is a const field
                    return identifierName.IsConst(type);
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an is pattern expression represents an enum check.
        /// </summary>
        /// <param name="value">
        /// The is pattern expression to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression represents an enum check; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsEnum(this IsPatternExpressionSyntax value, SemanticModel semanticModel) => value.Expression.IsEnum(semanticModel);

        /// <summary>
        /// Determines whether a member access expression represents an enum access.
        /// </summary>
        /// <param name="value">
        /// The member access expression to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression represents an enum access; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsEnum(this MemberAccessExpressionSyntax value, SemanticModel semanticModel) => value.Expression.IsEnum(semanticModel);

        /// <summary>
        /// Determines whether an expression syntax represents an enum value.
        /// </summary>
        /// <param name="value">
        /// The expression syntax to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression represents an enum value; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsEnum(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value is MemberAccessExpressionSyntax maes)
            {
                return maes.IsEnum(semanticModel);
            }

            var type = value.GetTypeSymbol(semanticModel);

            return type.IsEnum();
        }

        /// <summary>
        /// Determines whether a statement represents an event registration.
        /// </summary>
        /// <param name="value">
        /// The statement to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the statement represents an event registration; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsEventRegistration(this StatementSyntax value, SemanticModel semanticModel)
        {
            if (value is ExpressionStatementSyntax e && e.Expression is AssignmentExpressionSyntax assignment)
            {
                return IsEventRegistration(assignment, semanticModel);
            }

            return false;
        }

        /// <summary>
        /// Determines whether an assignment expression represents an event registration.
        /// </summary>
        /// <param name="value">
        /// The assignment expression to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the assignment expression represents an event registration; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsEventRegistration(this AssignmentExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value.Right is IdentifierNameSyntax)
            {
                switch (value.Left)
                {
                    case MemberAccessExpressionSyntax maes:
                        return maes.GetSymbol(semanticModel) is IEventSymbol;

                    case IdentifierNameSyntax identifier:
                        return identifier.GetSymbol(semanticModel) is IEventSymbol;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether a type syntax represents an exception type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents an exception type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsException(this TypeSyntax value) => value.IsException<Exception>();

        /// <summary>
        /// Determines whether a type syntax represents a specific exception type.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type to check for.
        /// </typeparam>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents the specified exception type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsException<T>(this TypeSyntax value) where T : Exception => value.IsException(typeof(T));

        /// <summary>
        /// Determines whether a type syntax represents a specific exception type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <param name="exceptionType">
        /// The exception type to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents the specified exception type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsException(this TypeSyntax value, Type exceptionType)
        {
            while (true)
            {
                switch (value)
                {
                    case PredefinedTypeSyntax _:
                        return false;

                    case NullableTypeSyntax nullable:
                        value = nullable.ElementType;

                        continue;

                    default:
                        var s = value.ToString();

                        return s == exceptionType.Name || s == exceptionType.FullName;
                }
            }
        }

        /// <summary>
        /// Determines whether a syntax node is part of an expression tree.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node is part of an expression tree; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsExpressionTree(this SyntaxNode value, SemanticModel semanticModel)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var a in value.AncestorsWithinMethods<ArgumentSyntax>())
            {
                var convertedType = semanticModel.GetTypeInfo(a.Expression).ConvertedType;
                var isExpression = convertedType?.InheritsFrom<Expression>() is true;

                if (isExpression)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether a type declaration is generated.
        /// </summary>
        /// <param name="value">
        /// The type declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type declaration is generated; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsGenerated(this TypeDeclarationSyntax value) => value.HasAttributeName(Constants.Names.GeneratedAttributeNames);

        /// <summary>
        /// Determines whether a syntax node is inside an if statement with a call to the specified method.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="methodName">
        /// The name of the method to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node is inside an if statement with a call to the specified method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsInsideIfStatementWithCallTo(this SyntaxNode value, string methodName)
        {
            while (true)
            {
                var ifStatement = value.GetEnclosing<IfStatementSyntax>();

                if (ifStatement != null)
                {
                    if (ifStatement.IsCallTo(methodName))
                    {
                        var elseStatement = value.GetEnclosing<ElseClauseSyntax>();

                        if (elseStatement != null && ifStatement.Equals(elseStatement.Parent))
                        {
                            // we are in the else part, not inside the 'if' part, so we fail
                            return false;
                        }

                        return true;
                    }

                    // maybe a nested one, so check parent
                    value = ifStatement.Parent;
                }
                else
                {
                    // maybe an else block
                    var elseStatement = value.GetEnclosing<ElseClauseSyntax>();

                    if (elseStatement != null)
                    {
                        value = elseStatement.Parent;

                        continue;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Determines whether an if statement contains a call to the specified method.
        /// </summary>
        /// <param name="value">
        /// The if statement to check.
        /// </param>
        /// <param name="methodName">
        /// The name of the method to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the if statement contains a call to the specified method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsCallTo(this IfStatementSyntax value, string methodName)
        {
            var ifExpression = value.FirstChild<MemberAccessExpressionSyntax>();

            if (ifExpression.IsCallTo(methodName))
            {
                return true;
            }

            var binaryExpression = value.FirstChild<BinaryExpressionSyntax>();

            if (binaryExpression.IsBinaryCallTo(methodName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether an expression syntax is a call to the specified method.
        /// </summary>
        /// <param name="value">
        /// The expression syntax to check.
        /// </param>
        /// <param name="methodName">
        /// The name of the method to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression is a call to the specified method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsCallTo(this ExpressionSyntax value, string methodName) => value is MemberAccessExpressionSyntax m && m.Name.ToString() == methodName;

        /// <summary>
        /// Determines whether an expression statement is an invocation on an object under test.
        /// </summary>
        /// <param name="value">
        /// The expression statement to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression is an invocation on an object under test; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsInvocationOnObjectUnderTest(this ExpressionStatementSyntax value)
        {
            switch (value.Expression)
            {
                case InvocationExpressionSyntax i when i.IsInvocationOnObjectUnderTest():
                case AwaitExpressionSyntax a when a.Expression is InvocationExpressionSyntax inv && inv.IsInvocationOnObjectUnderTest():
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether an invocation expression is on an object under test.
        /// </summary>
        /// <param name="value">
        /// The invocation expression to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the invocation is on an object under test; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsInvocationOnObjectUnderTest(this InvocationExpressionSyntax value) => value.Expression.IsAccessOnObjectUnderTest();

        /// <summary>
        /// Determines whether a method declaration has the abstract modifier.
        /// </summary>
        /// <param name="value">
        /// The method declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method declaration has the abstract modifier; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAbstract(this MethodDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.AbstractKeyword);

        /// <summary>
        /// Determines whether an expression syntax accesses an object under test.
        /// </summary>
        /// <param name="value">
        /// The expression syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression accesses an object under test; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAccessOnObjectUnderTest(this ExpressionSyntax value)
        {
            if (value is MemberAccessExpressionSyntax mae)
            {
                switch (mae.Expression)
                {
                    case IdentifierNameSyntax ins when Constants.Names.ObjectUnderTestNames.Contains(ins.GetName()):
                    case InvocationExpressionSyntax i when i.IsInvocationOnObjectUnderTest():
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether a base property declaration has the async modifier.
        /// </summary>
        /// <param name="value">
        /// The base property declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the property declaration has the async modifier; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAsync(this BasePropertyDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.AsyncKeyword);

        /// <summary>
        /// Determines whether a method declaration has the async modifier.
        /// </summary>
        /// <param name="value">
        /// The method declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method declaration has the async modifier; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAsync(this MethodDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.AsyncKeyword);

        /// <summary>
        /// Determines whether a syntax node is a local variable declaration with any of the specified identifier names.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="identifierNames">
        /// The set of identifier names to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a local variable declaration with any of the specified identifier names; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsLocalVariableDeclaration(this SyntaxNode value, ISet<string> identifierNames) => value is LocalDeclarationStatementSyntax l && l.Declaration.Variables.Any(__ => identifierNames.Contains(__.Identifier.ValueText));

        /// <summary>
        /// Determines whether a syntax node is a local variable declaration with the specified identifier name.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="identifierName">
        /// The identifier name to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a local variable declaration with the specified identifier name; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsLocalVariableDeclaration(this SyntaxNode value, string identifierName) => value is LocalDeclarationStatementSyntax l && l.Declaration.Variables.Any(__ => __.Identifier.ValueText == identifierName);

        /// <summary>
        /// Determines whether a syntax node is a field variable declaration with any of the specified identifier names.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="identifierNames">
        /// The set of identifier names to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a field variable declaration with any of the specified identifier names; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsFieldVariableDeclaration(this SyntaxNode value, ISet<string> identifierNames) => value is FieldDeclarationSyntax f && f.Declaration.Variables.Any(__ => identifierNames.Contains(__.Identifier.ValueText));

        /// <summary>
        /// Determines whether a syntax node is a field variable declaration with the specified identifier name.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="identifierName">
        /// The identifier name to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is a field variable declaration with the specified identifier name; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsFieldVariableDeclaration(this SyntaxNode value, string identifierName) => value is FieldDeclarationSyntax f && f.Declaration.Variables.Any(__ => __.Identifier.ValueText == identifierName);

        /// <summary>
        /// Determines whether an identifier name syntax represents a parameter in its enclosing method.
        /// </summary>
        /// <param name="value">
        /// The identifier name syntax to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the identifier name represents a parameter; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsParameter(this IdentifierNameSyntax value, SemanticModel semanticModel) => value.EnclosingMethodHasParameter(value.GetName(), semanticModel);

        /// <summary>
        /// Determines whether an is pattern expression is checking for a specific kind.
        /// </summary>
        /// <param name="value">
        /// The is pattern expression to check.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the pattern is checking for the specified kind; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsPatternCheckFor(this IsPatternExpressionSyntax value, in SyntaxKind kind) => value?.Pattern.IsPatternCheckFor(kind) is true;

        /// <summary>
        /// Determines whether a pattern syntax is checking for a specific kind.
        /// </summary>
        /// <param name="value">
        /// The pattern syntax to check.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the pattern is checking for the specified kind; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsPatternCheckFor(this PatternSyntax value, in SyntaxKind kind)
        {
            while (true)
            {
                switch (value)
                {
                    case ConstantPatternSyntax pattern:
                        return pattern.Expression.IsKind(kind);

                    case UnaryPatternSyntax unary:
                        value = unary.Pattern;

                        continue;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Determines whether a field declaration has the <see langword="readonly"/> modifier.
        /// </summary>
        /// <param name="value">
        /// The field declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the field declaration has the <see langword="readonly"/> modifier; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsReadOnly(this FieldDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);

        /// <summary>
        /// Determines whether a field declaration has the <see langword="static"/> modifier.
        /// </summary>
        /// <param name="value">
        /// The field declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the field declaration has the <see langword="static"/> modifier; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsStatic(this FieldDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.StaticKeyword);

        /// <summary>
        /// Determines whether a syntax node spans multiple lines.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node spans multiple lines; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSpanningMultipleLines(this SyntaxNode value)
        {
            var lineSpan = value.GetLineSpan();

            var startingLine = lineSpan.StartLinePosition.Line;
            var endingLine = lineSpan.EndLinePosition.Line;

            return startingLine != endingLine;
        }

        /// <summary>
        /// Determines whether a type syntax represents a <see cref="SerializationInfo"/> type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents a <see cref="SerializationInfo"/> type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSerializationInfo(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax)
            {
                return false;
            }

            var s = value.ToString();

            return s == nameof(SerializationInfo) || s == TypeNames.SerializationInfo;
        }

        /// <summary>
        /// Determines whether a type syntax represents a <see cref="StreamingContext"/> type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents a <see cref="StreamingContext"/> type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsStreamingContext(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax)
            {
                return false;
            }

            var s = value.ToString();

            return s == nameof(StreamingContext) || s == TypeNames.StreamingContext;
        }

        /// <summary>
        /// Determines whether a type syntax represents a <see cref="string"/> type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents a <see cref="string"/> type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsString(this TypeSyntax value)
        {
            while (true)
            {
                switch (value)
                {
                    case PredefinedTypeSyntax predefined:
                        return predefined.Keyword.IsKind(SyntaxKind.StringKeyword);

                    case NullableTypeSyntax nullable:
                        value = nullable.ElementType;

                        continue;

                    default:
                        switch (value.ToString())
                        {
                            case nameof(String):
                            case nameof(System) + "." + nameof(String):
                                return true;

                            default:
                                return false;
                        }
                }
            }
        }

        /// <summary>
        /// Determines whether an argument syntax represents a <see cref="string"/> value.
        /// </summary>
        /// <param name="value">
        /// The argument syntax to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the argument represents a <see cref="string"/> value; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsString(this ArgumentSyntax value, SemanticModel semanticModel)
        {
            if (value.IsStringLiteral())
            {
                return true;
            }

            return value.GetTypeSymbol(semanticModel).IsString();
        }

        /// <summary>
        /// Determines whether an expression syntax represents a <see cref="string"/> value.
        /// </summary>
        /// <param name="value">
        /// The expression syntax to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression represents a <see cref="string"/> value; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsString(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value.IsStringLiteral())
            {
                return true;
            }

            return value.GetTypeSymbol(semanticModel).IsString();
        }

        /// <summary>
        /// Determines whether an argument syntax represents a <see cref="string"/> literal.
        /// </summary>
        /// <param name="value">
        /// The argument syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the argument represents a <see cref="string"/> literal; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsStringLiteral(this ArgumentSyntax value) => value?.Expression.IsStringLiteral() is true;

        /// <summary>
        /// Determines whether an expression syntax represents a <see cref="string"/> literal.
        /// </summary>
        /// <param name="value">
        /// The expression syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression represents a <see cref="string"/> literal; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsStringLiteral(this ExpressionSyntax value)
        {
            switch (value?.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.InterpolatedStringExpression:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether an expression syntax represents a <see cref="string"/> concatenation.
        /// </summary>
        /// <param name="value">
        /// The expression syntax to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis, or <see langword="null"/> if not available.
        /// The default is <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression represents a <see cref="string"/> concatenation; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsStringConcatenation(this ExpressionSyntax value, SemanticModel semanticModel = null)
        {
            if (value.IsKind(SyntaxKind.AddExpression))
            {
                var b = (BinaryExpressionSyntax)value;

                if (b.Left.IsStringLiteral() || b.Right.IsStringLiteral())
                {
                    return true;
                }

                if (b.Left.IsStringConcatenation(semanticModel) || b.Right.IsStringConcatenation(semanticModel))
                {
                    return true;
                }

                if (semanticModel != null)
                {
                    if (b.Left.IsString(semanticModel) || b.Right.IsString(semanticModel))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an invocation expression represents a <see cref="string"/> format call.
        /// </summary>
        /// <param name="value">
        /// The invocation expression to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the invocation represents a <see cref="string"/> format call; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsStringFormat(this InvocationExpressionSyntax value) => value.Expression is MemberAccessExpressionSyntax maes
                                                                                   && maes.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                                                                                   && maes.Expression is TypeSyntax invokedType
                                                                                   && invokedType.IsString()
                                                                                   && maes.GetName() == nameof(string.Format);

        /// <summary>
        /// Determines whether an expression syntax represents a struct or enum value.
        /// </summary>
        /// <param name="value">
        /// The expression syntax to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the expression represents a struct or enum value; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsStruct(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            var type = value.GetTypeSymbol(semanticModel);

            switch (type?.TypeKind)
            {
                case TypeKind.Struct:
                case TypeKind.Enum:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a type syntax represents an object type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents an object type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsObject(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax predefined)
            {
                return predefined.Keyword.IsKind(SyntaxKind.ObjectKeyword);
            }

            switch (value.ToString())
            {
                case nameof(Object):
                case nameof(System) + "." + nameof(Object):
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a syntax node is inside a test class.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node is inside a test class; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsInsideTestClass(this SyntaxNode value) => value.Ancestors<ClassDeclarationSyntax>().Any(_ => _.IsTestClass());

        /// <summary>
        /// Determines whether a base type declaration represents a test class.
        /// </summary>
        /// <param name="value">
        /// The base type declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type declaration represents a test class; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTestClass(this BaseTypeDeclarationSyntax value) => value is ClassDeclarationSyntax declaration && IsTestClass(declaration);

        /// <summary>
        /// Determines whether a class declaration represents a test class.
        /// </summary>
        /// <param name="value">
        /// The class declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the class declaration represents a test class; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTestClass(this ClassDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestClassAttributeNames);

        /// <summary>
        /// Determines whether a method is an assembly-wide test setup method.
        /// </summary>
        /// <param name="value">
        /// The method to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method is an assembly-wide test setup method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTestAssemblyWideSetUpMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestAssemblyWideSetupAttributeNames);

        /// <summary>
        /// Determines whether a method is an assembly-wide test tear down method.
        /// </summary>
        /// <param name="value">
        /// The method to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method is an assembly-wide test tear down method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTestAssemblyWideTearDownMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestAssemblyWideTearDownAttributeNames);

        /// <summary>
        /// Determines whether a method declaration represents a test one-time setup method.
        /// </summary>
        /// <param name="value">
        /// The method declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method declaration represents a test one-time setup method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTestOneTimeSetUpMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestOneTimeSetupAttributeNames);

        /// <summary>
        /// Determines whether a method declaration represents a test one-time tear down method.
        /// </summary>
        /// <param name="value">
        /// The method declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method declaration represents a test one-time tear down method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTestOneTimeTearDownMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestOneTimeTearDownAttributeNames);

        /// <summary>
        /// Determines whether a method declaration represents a test setup method.
        /// </summary>
        /// <param name="value">
        /// The method declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method declaration represents a test setup method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTestSetUpMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestSetupAttributeNames);

        /// <summary>
        /// Determines whether a method declaration represents a test tear down method.
        /// </summary>
        /// <param name="value">
        /// The method declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method declaration represents a test tear down method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTestTearDownMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestTearDownAttributeNames);

        /// <summary>
        /// Determines whether a method declaration represents a test method.
        /// </summary>
        /// <param name="value">
        /// The method declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method declaration represents a test method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTestMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestMethodAttributeNames);

        /// <summary>
        /// Determines whether a method declaration represents a type under test creation method.
        /// </summary>
        /// <param name="value">
        /// The method declaration to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the method declaration represents a type under test creation method; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTypeUnderTestCreationMethod(this MethodDeclarationSyntax value) => Constants.Names.TypeUnderTestMethodNames.Contains(value.GetName());

        /// <summary>
        /// Determines whether a variable declarator represents a type under test variable.
        /// </summary>
        /// <param name="value">
        /// The variable declarator to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the variable declarator represents a type under test variable; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsTypeUnderTestVariable(this VariableDeclaratorSyntax value) => Constants.Names.TypeUnderTestVariableNames.Contains(value.GetName());

        /// <summary>
        /// Determines whether a type syntax represents a void type.
        /// </summary>
        /// <param name="value">
        /// The type syntax to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the type syntax represents a void type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsVoid(this TypeSyntax value) => value is PredefinedTypeSyntax p && p.Keyword.IsKind(SyntaxKind.VoidKeyword);

        /// <summary>
        /// Gets all LINQ extension methods in a syntax node using the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The syntax node to search in.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// A sequence that contains the invocation expressions that represent LINQ extension methods.
        /// </returns>
        internal static IEnumerable<InvocationExpressionSyntax> LinqExtensionMethods(this SyntaxNode value, SemanticModel semanticModel) => value.DescendantNodes<InvocationExpressionSyntax>(_ => IsLinqExtensionMethod(_, semanticModel));

        /// <summary>
        /// Removes a specific trivia from a syntax node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to modify.
        /// </param>
        /// <param name="trivia">
        /// The trivia to remove.
        /// </param>
        /// <returns>
        /// A new syntax node with the specified trivia removed.
        /// </returns>
        internal static T RemoveTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode => value.ReplaceTrivia(trivia, SyntaxFactory.ElasticMarker);

        /// <summary>
        /// Removes a node from a base type declaration and adjusts the open and close braces.
        /// </summary>
        /// <param name="value">
        /// The base type declaration to modify.
        /// </param>
        /// <param name="node">
        /// The node to remove.
        /// </param>
        /// <returns>
        /// A new base type declaration with the node removed and braces adjusted.
        /// </returns>
        internal static BaseTypeDeclarationSyntax RemoveNodeAndAdjustOpenCloseBraces(this BaseTypeDeclarationSyntax value, SyntaxNode node)
        {
            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var openBraceToken = value.OpenBraceToken.WithoutTrivia().WithEndOfLine();

            return value.Without(node)
                        .WithOpenBraceToken(openBraceToken);
        }

        /// <summary>
        /// Removes multiple nodes from a base type declaration and adjusts the open and close braces.
        /// </summary>
        /// <param name="value">
        /// The base type declaration to modify.
        /// </param>
        /// <param name="nodes">
        /// The nodes to remove.
        /// </param>
        /// <returns>
        /// A new base type declaration with the nodes removed and braces adjusted.
        /// </returns>
        internal static BaseTypeDeclarationSyntax RemoveNodesAndAdjustOpenCloseBraces(this BaseTypeDeclarationSyntax value, IEnumerable<SyntaxNode> nodes)
        {
            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var openBraceToken = value.OpenBraceToken.WithoutTrivia().WithEndOfLine();

            return value.Without(nodes)
                        .WithOpenBraceToken(openBraceToken);
        }

        internal static T ReplaceNodes<T, TNode>(this T value, IEnumerable<TNode> nodes, Func<TNode, IEnumerable<SyntaxNode>> computeReplacementNodes)
                                                                                                                                                  where T : SyntaxNode
                                                                                                                                                  where TNode : SyntaxNode
        {
            // replace all nodes by following algorithm:
            // 1. Create a dictionary with SyntaxAnnotations and replacement nodes for the node to annotate (new SyntaxAnnotation)
            // 2. Annotate the node to keep track (node.WithAnnotation())
            // 3. Loop over all annotated nodes and replace them with the replacement nodes (document.GetAnnotatedNodes(annotation))
            var annotation = new SyntaxAnnotation();

            var result = value.ReplaceNodes(nodes, (_, rewritten) => rewritten.WithAnnotation(annotation));

            while (true)
            {
                var oldNode = result.GetAnnotatedNodes(annotation).OfType<TNode>().FirstOrDefault();

                if (oldNode is null)
                {
                    // nothing left
                    break;
                }

                // create replacement nodes
                var replacements = computeReplacementNodes(oldNode);

                // and remove the annotations in case we get the same node back (to avoid endless while loop as we would always get the same node again)
                result = result.ReplaceNode(oldNode, replacements.Select(_ => _.WithoutAnnotations(annotation)));
            }

            return result;
        }

        /// <summary>
        /// Determines whether an if statement returns immediately.
        /// </summary>
        /// <param name="value">
        /// The if statement to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the if statement returns immediately; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool ReturnsImmediately(this IfStatementSyntax value)
        {
            switch (value?.Statement)
            {
                case ReturnStatementSyntax _:
                case BlockSyntax block when block.Statements.FirstOrDefault() is ReturnStatementSyntax:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether an else clause returns immediately.
        /// </summary>
        /// <param name="value">
        /// The else clause to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the else clause returns immediately; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool ReturnsImmediately(this ElseClauseSyntax value)
        {
            switch (value?.Statement)
            {
                case ReturnStatementSyntax _:
                case BlockSyntax block when block.Statements.FirstOrDefault() is ReturnStatementSyntax:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a return statement returns a completed task.
        /// </summary>
        /// <param name="value">
        /// The return statement to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the return statement returns a completed task; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool ReturnsCompletedTask(this ReturnStatementSyntax value) => value.Expression is MemberAccessExpressionSyntax maes && maes.Expression.GetName() == nameof(Task) && maes.GetName() == nameof(Task.CompletedTask);

        /// <summary>
        /// Determines whether a syntax node throws a specific exception type.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type to check for.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node throws the specified exception type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Throws<T>(this SyntaxNode value) where T : Exception
        {
            switch (value)
            {
                case ThrowStatementSyntax ts when ts.Expression is ObjectCreationExpressionSyntax tso && tso.Type.IsException<T>():
                case ThrowExpressionSyntax te when te.Expression is ObjectCreationExpressionSyntax teo && teo.Type.IsException<T>():
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a syntax node has a region directive.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node has a region directive; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasRegionDirective(this SyntaxNode value) => value != null && value.HasStructuredTrivia && value.GetLeadingTrivia().Any(SyntaxKind.RegionDirectiveTrivia);

        /// <summary>
        /// Attempts to get the region directive from a syntax node.
        /// </summary>
        /// <param name="source">
        /// The syntax node to get the region directive from.
        /// </param>
        /// <param name="regionDirective">
        /// On successful return, contains the directive trivia syntax if found; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a region directive was found; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool TryGetRegionDirective(this SyntaxNode source, out DirectiveTriviaSyntax regionDirective)
        {
            if (source != null && source.HasStructuredTrivia)
            {
                var leadingTrivia = source.GetLeadingTrivia();

                var count = leadingTrivia.Count;

                if (count > 0)
                {
                    for (var index = 0; index < count; index++)
                    {
                        var t = leadingTrivia[index];

                        if (t.IsKind(SyntaxKind.RegionDirectiveTrivia))
                        {
                            regionDirective = t.GetStructure() as DirectiveTriviaSyntax;

                            return true;
                        }
                    }
                }
            }

            regionDirective = null;

            return false;
        }

        /// <summary>
        /// Gets a cleaned-up <see cref="string"/> representation of an expression syntax.
        /// </summary>
        /// <param name="source">
        /// The expression syntax to get a <see cref="string"/> representation for.
        /// </param>
        /// <returns>
        /// A cleaned-up <see cref="string"/> representation of the expression.
        /// </returns>
        internal static string ToCleanedUpString(this ExpressionSyntax source) => source?.ToString().Without(Constants.WhiteSpaces);

        /// <summary>
        /// Adds an annotation to a syntax node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to annotate.
        /// </param>
        /// <param name="annotation">
        /// The annotation to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the annotation added.
        /// </returns>
        internal static T WithAnnotation<T>(this T value, SyntaxAnnotation annotation) where T : SyntaxNode => value.WithAdditionalAnnotations(annotation);

        /// <summary>
        /// Creates a new invocation expression with the specified arguments.
        /// </summary>
        /// <param name="value">
        /// The invocation expression to modify.
        /// </param>
        /// <param name="arguments">
        /// The new arguments for the invocation.
        /// </param>
        /// <returns>
        /// A new invocation expression with the specified arguments.
        /// </returns>
        internal static InvocationExpressionSyntax WithArguments(this InvocationExpressionSyntax value, in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return value.WithArgumentList(SyntaxFactory.ArgumentList(arguments));
        }

        /// <summary>
        /// Creates a new invocation expression with the specified arguments.
        /// </summary>
        /// <param name="value">
        /// The invocation expression to modify.
        /// </param>
        /// <param name="arguments">
        /// The new arguments for the invocation.
        /// </param>
        /// <returns>
        /// A new invocation expression with the specified arguments.
        /// </returns>
        internal static InvocationExpressionSyntax WithArguments(this InvocationExpressionSyntax value, params ArgumentSyntax[] arguments)
        {
            return value.WithArguments(arguments.ToSeparatedSyntaxList());
        }

        /// <summary>
        /// Creates a new field declaration with the specified modifiers.
        /// </summary>
        /// <param name="value">
        /// The field declaration to modify.
        /// </param>
        /// <param name="modifiers">
        /// The modifiers to apply to the field.
        /// </param>
        /// <returns>
        /// A new syntax node with the specified modifiers.
        /// </returns>
        internal static SyntaxNode WithModifiers(this FieldDeclarationSyntax value, IEnumerable<SyntaxKind> modifiers)
        {
            var oldModifiers = value.Modifiers;

            var newModifiers = modifiers.ToTokenList();
            var modifier = newModifiers[0];

            if (oldModifiers.Count > 0)
            {
                // keep comments
                newModifiers = newModifiers.Replace(modifier, modifier.WithTriviaFrom(oldModifiers[0]));

                return value.WithModifiers(newModifiers);
            }

            var declaration = value.Declaration;
            var type = declaration.Type;

            // keep comments
            newModifiers = newModifiers.Replace(modifier, modifier.WithLeadingTriviaFrom(type));

            return value.WithModifiers(newModifiers)
                        .WithDeclaration(declaration.WithType(type.WithLeadingSpace()));
        }

        /// <summary>
        /// Creates a new field declaration with the specified modifiers.
        /// </summary>
        /// <param name="value">
        /// The field declaration to modify.
        /// </param>
        /// <param name="modifiers">
        /// The modifiers to apply to the field.
        /// </param>
        /// <returns>
        /// A new syntax node with the specified modifiers.
        /// </returns>
        internal static SyntaxNode WithModifiers(this FieldDeclarationSyntax value, params SyntaxKind[] modifiers) => value.WithModifiers((IEnumerable<SyntaxKind>)modifiers);

        /// <summary>
        /// Creates a new member declaration with an additional modifier.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the member declaration.
        /// </typeparam>
        /// <param name="value">
        /// The member declaration to modify.
        /// </param>
        /// <param name="keyword">
        /// One of the enumeration members that specifies the syntax kind of the modifier to add.
        /// </param>
        /// <returns>
        /// A new member declaration with the additional modifier.
        /// </returns>
        internal static T WithAdditionalModifier<T>(this T value, in SyntaxKind keyword) where T : MemberDeclarationSyntax
        {
            var modifiers = value.Modifiers;
            var position = modifiers.IndexOf(SyntaxKind.PartialKeyword);

            var syntaxToken = keyword.AsToken();

            if (modifiers.Count is 0)
            {
                var commentedChild = value.FirstChildToken();

                // remove comment from previous first child
                value = value.ReplaceToken(commentedChild, commentedChild.WithLeadingSpace());

                // add comment to new first child
                syntaxToken = syntaxToken.WithLeadingTriviaFrom(commentedChild);
            }

            var newModifiers = position > -1
                               ? value.Modifiers.Insert(position, syntaxToken)
                               : value.Modifiers.Add(syntaxToken);

            return (T)value.WithModifiers(newModifiers);
        }

        /// <summary>
        /// Removes a node from a syntax tree.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the root node.
        /// </typeparam>
        /// <param name="value">
        /// The root node of the syntax tree.
        /// </param>
        /// <param name="node">
        /// The node to remove.
        /// </param>
        /// <returns>
        /// A new syntax tree with the node removed.
        /// </returns>
        internal static T Without<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var removeOptions = node is DocumentationCommentTriviaSyntax
                                ? SyntaxRemoveOptions.AddElasticMarker
                                : SyntaxRemoveOptions.KeepNoTrivia;

            return value.RemoveNode(node, removeOptions);
        }

        /// <summary>
        /// Removes multiple nodes from a syntax tree.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the root node.
        /// </typeparam>
        /// <param name="value">
        /// The root node of the syntax tree.
        /// </param>
        /// <param name="nodes">
        /// The nodes to remove.
        /// </param>
        /// <returns>
        /// A new syntax tree with the nodes removed.
        /// </returns>
        internal static T Without<T>(this T value, IEnumerable<SyntaxNode> nodes) where T : SyntaxNode => value.RemoveNodes(nodes, SyntaxRemoveOptions.KeepNoTrivia);

        /// <summary>
        /// Removes multiple trivia from a syntax tree.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the root node.
        /// </typeparam>
        /// <param name="value">
        /// The root node of the syntax tree.
        /// </param>
        /// <param name="trivia">
        /// The trivia to remove.
        /// </param>
        /// <returns>
        /// A new syntax tree with the trivia removed.
        /// </returns>
        internal static T Without<T>(this T value, IEnumerable<SyntaxTrivia> trivia) where T : SyntaxNode => value.ReplaceTrivia(trivia, (original, rewritten) => default);

        /// <summary>
        /// Removes multiple nodes from a syntax tree.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the root node.
        /// </typeparam>
        /// <param name="value">
        /// The root node of the syntax tree.
        /// </param>
        /// <param name="nodes">
        /// The nodes to remove.
        /// </param>
        /// <returns>
        /// A new syntax tree with the nodes removed.
        /// </returns>
        internal static T Without<T>(this T value, params SyntaxNode[] nodes) where T : SyntaxNode => value.Without((IEnumerable<SyntaxNode>)nodes);

        /// <summary>
        /// Removes parenthesis from an expression.
        /// </summary>
        /// <param name="value">
        /// The expression to remove parenthesis from.
        /// </param>
        /// <returns>
        /// A new expression without parenthesis.
        /// </returns>
        internal static ExpressionSyntax WithoutParenthesis(this ExpressionSyntax value)
        {
            while (true)
            {
                if (value is ParenthesizedExpressionSyntax parenthesized)
                {
                    value = parenthesized.Expression;

                    continue;
                }

                return value;
            }
        }

        /// <summary>
        /// Adds a using directive for the specified namespace to a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to add the using directive to.
        /// </param>
        /// <param name="usingNamespace">
        /// The namespace to add a using directive for.
        /// </param>
        /// <returns>
        /// A new syntax node with the using directive added.
        /// </returns>
        internal static SyntaxNode WithUsing(this SyntaxNode value, string usingNamespace)
        {
            var usings = value.DescendantNodes<UsingDirectiveSyntax>().ToList();

            if (usings.Exists(_ => _.Name?.ToFullString() == usingNamespace))
            {
                // already set
                return value;
            }

            var directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingNamespace));

            var usingsCount = usings.Count;

            if (usingsCount is 0)
            {
                return value.InsertNodeBefore(value.FirstChild(), directive);
            }

            for (var index = 0; index < usingsCount; index++)
            {
                var usingDirective = usings[index];

                var usingName = usingDirective.Name?.ToFullString();

                if (usingName is "System")
                {
                    // skip 'System' namespace
                    continue;
                }

                if (usingName?.StartsWith("System.", StringComparison.Ordinal) is true)
                {
                    // skip all 'System' sub-namespaces
                    continue;
                }

                if (string.Compare(usingName, usingNamespace, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    // add using at correct place inside the using block
                    return value.InsertNodeBefore(usingDirective, directive);
                }
            }

            return value.InsertNodeAfter(usings[usingsCount - 1], directive);
        }

        /// <summary>
        /// Removes a using directive for the specified namespace from a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to remove the using directive from.
        /// </param>
        /// <param name="usingNamespace">
        /// The namespace to remove the using directive for.
        /// </param>
        /// <returns>
        /// A new syntax node with the using directive removed.
        /// </returns>
        internal static SyntaxNode WithoutUsing(this SyntaxNode value, string usingNamespace)
        {
            var root = value.SyntaxTree.GetRoot();

            return root.DescendantNodes<UsingDirectiveSyntax>(_ => _.Name?.ToFullString() == usingNamespace)
                       .Select(root.Without)
                       .FirstOrDefault();
        }

        /// <summary>
        /// Gets the Fluent Assertions "Should" node from an expression statement.
        /// </summary>
        /// <param name="value">
        /// The expression statement to search in.
        /// </param>
        /// <returns>
        /// The member access expression that represents a Fluent Assertions "Should" call, or <see langword="null"/> if none exists.
        /// </returns>
        internal static MemberAccessExpressionSyntax GetFluentAssertionShouldNode(this ExpressionStatementSyntax value)
        {
            var nodes = value.DescendantNodes<MemberAccessExpressionSyntax>(SyntaxKind.SimpleMemberAccessExpression);

            foreach (var node in nodes)
            {
                var name = node.GetName();

                switch (name)
                {
                    // we might have a lambda expression, so the given statement might not be the correct ancestor statement of the 'Should()' node, hence we have to determine whether it's the specific statement that has an issue
                    case Constants.FluentAssertions.Should when node.FirstAncestor<ExpressionStatementSyntax>() == value:
                        return node;

                    case Constants.FluentAssertions.ShouldBeEquivalentTo when node.FirstAncestor<ExpressionStatementSyntax>() == value:
                        return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Collects parameters that are accessible from an object creation expression.
        /// </summary>
        /// <param name="syntax">
        /// The object creation expression to collect parameters for.
        /// </param>
        /// <returns>
        /// A collection of parameters accessible from the given context.
        /// </returns>
        private static SeparatedSyntaxList<ParameterSyntax> CollectParameters(ObjectCreationExpressionSyntax syntax)
        {
            var method = syntax.GetEnclosing<BaseMethodDeclarationSyntax>();

            if (method != null)
            {
                return method.ParameterList.Parameters;
            }

            var indexer = syntax.GetEnclosing<IndexerDeclarationSyntax>();

            if (indexer != null)
            {
                var parameters = indexer.ParameterList.Parameters;

                // 'value' is a special parameter that is not part of the parameter list
                return parameters.Insert(0, Parameter(indexer.Type));
            }

            var property = syntax.GetEnclosing<PropertyDeclarationSyntax>();

            var result = SyntaxFactory.SeparatedList<ParameterSyntax>();

            return property is null
                   ? result
                   : result.Add(Parameter(property.Type)); // 'value' is a special parameter that is not part of the parameter list

            ParameterSyntax Parameter(TypeSyntax type) => SyntaxFactory.Parameter(default, default, type, SyntaxFactory.Identifier(Constants.Names.DefaultPropertyParameterName), null);
        }

        /// <summary>
        /// Determines whether a binary expression contains a call to the specified method.
        /// </summary>
        /// <param name="value">
        /// The binary expression to check.
        /// </param>
        /// <param name="methodName">
        /// The name of the method to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the binary expression contains a call to the specified method; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsBinaryCallTo(this BinaryExpressionSyntax value, string methodName)
        {
            if (value is null)
            {
                return false;
            }

            if (value.OperatorToken.IsKind(SyntaxKind.AmpersandAmpersandToken))
            {
                if (value.Left.IsCallTo(methodName) || value.Right.IsCallTo(methodName))
                {
                    return true;
                }

                // maybe it is a combined one
                if (value.Left is BinaryExpressionSyntax left && IsBinaryCallTo(left, methodName))
                {
                    return true;
                }

                // maybe it is a combined one
                if (value.Right is BinaryExpressionSyntax right && IsBinaryCallTo(right, methodName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an invocation expression represents a LINQ extension method.
        /// </summary>
        /// <param name="node">
        /// The invocation expression to check.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for analysis.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the invocation represents a LINQ extension method; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsLinqExtensionMethod(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.Expression is MemberAccessExpressionSyntax maes && maes.Expression is PredefinedTypeSyntax)
            {
                return false;
            }

            var name = node.Expression.GetName();

            if (Constants.Names.LinqMethodNames.Contains(name))
            {
                var info = semanticModel.GetSymbolInfo(node);

                if (info.CandidateReason is CandidateReason.None)
                {
                    return info.Symbol.IsLinqExtensionMethod();
                }
                else
                {
                    var candidates = info.CandidateSymbols;

                    return candidates.Length > 0 && candidates.Any(_ => _.IsLinqExtensionMethod());
                }
            }

            return false;
        }
    }
}
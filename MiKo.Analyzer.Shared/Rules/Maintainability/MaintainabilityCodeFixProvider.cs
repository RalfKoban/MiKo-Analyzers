using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityCodeFixProvider : MiKoCodeFixProvider
    {
        private static readonly Dictionary<SyntaxKind, SyntaxKind> OperatorInverseMapping = new Dictionary<SyntaxKind, SyntaxKind>
                                                                                                {
                                                                                                    { SyntaxKind.ExclamationEqualsToken, SyntaxKind.EqualsEqualsToken },
                                                                                                    { SyntaxKind.EqualsEqualsToken, SyntaxKind.ExclamationEqualsToken },
                                                                                                    { SyntaxKind.GreaterThanEqualsToken, SyntaxKind.LessThanToken },
                                                                                                    { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanEqualsToken },
                                                                                                    { SyntaxKind.LessThanEqualsToken, SyntaxKind.GreaterThanToken },
                                                                                                    { SyntaxKind.LessThanToken, SyntaxKind.GreaterThanEqualsToken },
                                                                                                };

        protected static ArgumentSyntax Argument(string identifier) => Argument(SyntaxFactory.IdentifierName(identifier));

        protected static ArgumentSyntax Argument(ParameterSyntax parameter) => Argument(SyntaxFactory.IdentifierName(parameter.GetName()));

        protected static ArgumentSyntax Argument(ExpressionSyntax expression) => SyntaxFactory.Argument(expression);

        protected static ArgumentSyntax Argument(MemberAccessExpressionSyntax expression, params ArgumentSyntax[] arguments)
        {
            var syntax = arguments.Length == 0
                         ? (ExpressionSyntax)expression // we do not want to have any empty argument list
                         : Invocation(expression, arguments);

            return Argument(syntax);
        }

        protected static ArgumentSyntax ArgumentWithCast(SyntaxKind kind, ParameterSyntax parameter) => ArgumentWithCast(PredefinedType(kind), parameter);

        protected static ArgumentSyntax ArgumentWithCast(TypeSyntax type, ParameterSyntax parameter) => ArgumentWithCast(type, SyntaxFactory.IdentifierName(parameter.GetName()));

        protected static ArgumentSyntax ArgumentWithCast(SyntaxKind kind, IdentifierNameSyntax identifier) => ArgumentWithCast(PredefinedType(kind), identifier);

        protected static ArgumentSyntax ArgumentWithCast(TypeSyntax type, IdentifierNameSyntax identifier) => Argument(SyntaxFactory.CastExpression(type, identifier));

        protected static InvocationExpressionSyntax Invocation(string typeName, string methodName, params ArgumentSyntax[] arguments)
        {
            // that's for the method call
            var member = SimpleMemberAccess(typeName, methodName);

            return Invocation(member, arguments);
        }

        protected static InvocationExpressionSyntax Invocation(string typeName, string propertyName, string methodName, params TypeSyntax[] items)
        {
            // that's for the method call
            var member = SimpleMemberAccess(typeName, propertyName, methodName, items);

            return Invocation(member);
        }

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(PredefinedTypeSyntax type, string methodName)
        {
            var method = SyntaxFactory.IdentifierName(methodName);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);
        }

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(string typeName, string methodName)
        {
            var type = SyntaxFactory.IdentifierName(typeName);
            var method = SyntaxFactory.IdentifierName(methodName);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);
        }

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(string typeName, string middlePart, string methodName, TypeSyntax[] items)
        {
            var type = SyntaxFactory.IdentifierName(typeName);
            var method = SyntaxFactory.GenericName(methodName).AddTypeArgumentListArguments(items);

            var expression = SimpleMemberAccess(type, middlePart);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, method);
        }

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(string typeName, params string[] methodNames)
        {
            var start = SimpleMemberAccess(typeName, methodNames[0]);

            var result = methodNames.Skip(1).Aggregate(start, SimpleMemberAccess);

            return result;
        }

        protected static ExpressionSyntax InvertCondition(Document document, ExpressionSyntax condition)
        {
            switch (condition)
            {
                case PrefixUnaryExpressionSyntax prefixed:
                {
                    return prefixed.Operand;
                }

                case BinaryExpressionSyntax binary when binary.Right.IsKind(SyntaxKind.NullLiteralExpression) && binary.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken):
                {
                    return IsNullPattern(binary.Left);
                }

                case BinaryExpressionSyntax binary when OperatorInverseMapping.TryGetValue(binary.OperatorToken.Kind(), out var replacement):
                {
                    return binary.WithOperatorToken(replacement.AsToken());
                }

                case IsPatternExpressionSyntax pattern:
                {
                    if (pattern.Pattern is ConstantPatternSyntax c && c.Expression is LiteralExpressionSyntax literal)
                    {
                        switch (literal.Kind())
                        {
                            case SyntaxKind.NullLiteralExpression:
                                return SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, pattern.Expression, literal);

                            case SyntaxKind.FalseLiteralExpression:
                                return IsNullable(document, pattern)
                                       ? LogicalNot(pattern)
                                       : pattern.Expression.WithoutTrivia();

                            case SyntaxKind.TrueLiteralExpression:
                                return IsNullable(document, pattern)
                                       ? LogicalNot(pattern)
                                       : pattern.Expression.WithoutTrivia();
                        }
                    }

                    break;
                }
            }

            return IsFalsePattern(condition);
        }

        protected static bool IsNullable(Document document, IsPatternExpressionSyntax pattern) => GetSymbol(document, pattern.Expression) is ITypeSymbol typeSymbol && typeSymbol.IsNullable();

        protected static ExpressionSyntax LogicalNot(ExpressionSyntax expression) => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(expression));

        protected static LiteralExpressionSyntax Literal(char value) => Literal(SyntaxFactory.Literal(value));

        protected static LiteralExpressionSyntax Literal(decimal value) => Literal(SyntaxFactory.Literal(value));

        protected static LiteralExpressionSyntax Literal(int value) => Literal(SyntaxFactory.Literal(value));

        protected static LiteralExpressionSyntax Literal(SyntaxToken token) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, token);

        protected static LiteralExpressionSyntax Literal(int value, string valueRepresentation) => Literal(SyntaxFactory.Literal(valueRepresentation, value));

        protected static LiteralExpressionSyntax NullLiteral() => Literal(SyntaxKind.NullLiteralExpression);

        protected static LiteralExpressionSyntax StringLiteral(string text)
        {
            var token = text.SurroundedWithDoubleQuote().AsToken();

            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, token);
        }

        protected static TypeOfExpressionSyntax TypeOf(ParameterSyntax parameter)
        {
            var typeSyntax = parameter.Type;

            return typeSyntax is null
                   ? TypeOf(SyntaxKind.VoidKeyword)
                   : TypeOf(typeSyntax);
        }

        protected static TypeOfExpressionSyntax TypeOf(SyntaxKind kind) => TypeOf(PredefinedType(kind));

        protected static TypeOfExpressionSyntax TypeOf(string typeName) => TypeOf(SyntaxFactory.ParseTypeName(typeName));

        protected static TypeOfExpressionSyntax TypeOf(TypeSyntax type) => SyntaxFactory.TypeOfExpression(type);

        protected static InvocationExpressionSyntax NameOf(LiteralExpressionSyntax literal)
        {
            var identifierName = literal.GetName();

            return NameOf(identifierName);
        }

        protected static InvocationExpressionSyntax NameOf(string identifierName)
        {
            var syntax = SyntaxFactory.IdentifierName(identifierName);

            return NameOf(syntax);
        }

        protected static InvocationExpressionSyntax NameOf(string typeName, string identifierName)
        {
            var syntax = SimpleMemberAccess(typeName, identifierName);

            return NameOf(syntax);
        }

        protected static InvocationExpressionSyntax NameOf(TypeSyntax type, LiteralExpressionSyntax literal)
        {
            var typeName = type.GetNameOnlyPart();
            var identifierName = literal.GetName();

            return NameOf(typeName, identifierName);
        }

        protected static InvocationExpressionSyntax NameOf(ITypeSymbol type, LiteralExpressionSyntax literal)
        {
            var typeName = type.Name;
            var identifierName = literal.GetName();

            return NameOf(typeName, identifierName);
        }

        private static InvocationExpressionSyntax NameOf(ExpressionSyntax syntax)
        {
            // nameof has a special RawContextualKind, hence we have to create it via its specific SyntaxKind
            // (see https://stackoverflow.com/questions/46259039/constructing-nameof-expression-via-syntaxfactory-roslyn)
            var nameofSyntax = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(SyntaxFactory.TriviaList(), SyntaxKind.NameOfKeyword, "nameof", "nameof", SyntaxFactory.TriviaList()));

            return SyntaxFactory.InvocationExpression(nameofSyntax, ArgumentList(SyntaxFactory.Argument(syntax)));
        }
    }
}
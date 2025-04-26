﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityCodeFixProvider : MiKoCodeFixProvider
    {
        private static readonly Dictionary<SyntaxKind, SyntaxKind> OperatorToInverseMap = new Dictionary<SyntaxKind, SyntaxKind>
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

        protected static ArgumentSyntax Argument(string typeName, string methodName, params ArgumentSyntax[] arguments)
        {
            return Argument(SimpleMemberAccess(typeName, methodName), arguments);
        }

        protected static ArgumentSyntax ArgumentWithCast(in SyntaxKind kind, ParameterSyntax parameter) => ArgumentWithCast(PredefinedType(kind), parameter);

        protected static ArgumentSyntax ArgumentWithCast(TypeSyntax type, ParameterSyntax parameter) => ArgumentWithCast(type, SyntaxFactory.IdentifierName(parameter.GetName()));

        protected static ArgumentSyntax ArgumentWithCast(in SyntaxKind kind, IdentifierNameSyntax identifier) => ArgumentWithCast(PredefinedType(kind), identifier);

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
                    return InvertCondition(prefixed);

                case BinaryExpressionSyntax binary:
                    return InvertCondition(document, binary);

                case IsPatternExpressionSyntax patternExpression:
                    return InvertCondition(document, patternExpression);

                default:
                    return IsFalsePattern(condition);
            }
        }

        protected static ExpressionSyntax LogicalNot(ExpressionSyntax expression) => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(expression));

        protected static IsPatternExpressionSyntax UnaryNot(IsPatternExpressionSyntax expression) => expression.WithPattern(UnaryNot(expression.Pattern));

        protected static UnaryPatternSyntax UnaryNot(PatternSyntax pattern) => SyntaxFactory.UnaryPattern(SyntaxKind.NotKeyword.AsToken().WithTrailingSpace(), pattern);

        protected static LiteralExpressionSyntax Literal(in char value) => Literal(SyntaxFactory.Literal(value));

        protected static LiteralExpressionSyntax Literal(decimal value) => Literal(SyntaxFactory.Literal(value));

        protected static LiteralExpressionSyntax Literal(in int value) => Literal(SyntaxFactory.Literal(value));

        protected static LiteralExpressionSyntax Literal(in SyntaxToken token) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, token);

        protected static LiteralExpressionSyntax Literal(in int value, string valueRepresentation) => Literal(SyntaxFactory.Literal(valueRepresentation, value));

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

        protected static TypeOfExpressionSyntax TypeOf(in SyntaxKind kind) => TypeOf(PredefinedType(kind));

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

        private static ExpressionSyntax InvertCondition(PrefixUnaryExpressionSyntax prefixed)
        {
            var operand = prefixed.Operand;

            // remove parenthesis when possible
            if (operand is ParenthesizedExpressionSyntax syntax)
            {
                var expression = syntax.WithoutParenthesis();

                switch (expression.Kind())
                {
                    case SyntaxKind.IsExpression:
                    case SyntaxKind.IsPatternExpression:
                        return expression;
                }
            }

            return operand;
        }

        private static ExpressionSyntax InvertCondition(Document document, BinaryExpressionSyntax condition)
        {
            if (condition.Right.IsKind(SyntaxKind.NullLiteralExpression) && condition.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
            {
                return IsNullPattern(condition.Left);
            }

            if (OperatorToInverseMap.TryGetValue(condition.OperatorToken.Kind(), out var replacement))
            {
                return condition.WithOperatorToken(replacement.AsToken());
            }

            switch (condition.Kind())
            {
                case SyntaxKind.LogicalAndExpression:
                {
                    return SyntaxFactory.BinaryExpression(SyntaxKind.LogicalOrExpression, InvertCondition(document, condition.Left), InvertCondition(document, condition.Right));
                }

                case SyntaxKind.LogicalOrExpression:
                {
                    return SyntaxFactory.BinaryExpression(SyntaxKind.LogicalAndExpression, InvertCondition(document, condition.Left), InvertCondition(document, condition.Right));
                }

                default:
                {
                    return IsFalsePattern(condition);
                }
            }
        }

        private static ExpressionSyntax InvertCondition(Document document, IsPatternExpressionSyntax condition)
        {
            switch (condition.Pattern)
            {
                case ConstantPatternSyntax constant when constant.Expression is LiteralExpressionSyntax literal:
                {
                    switch (literal.Kind())
                    {
                        case SyntaxKind.NullLiteralExpression:
                        {
                            if (document.HasMinimumCSharpVersion(LanguageVersion.CSharp9))
                            {
                                return UnaryNot(condition);
                            }

                            return SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, condition.Expression, literal);
                        }

                        case SyntaxKind.FalseLiteralExpression:
                        case SyntaxKind.TrueLiteralExpression:
                        {
                            if (condition.IsNullable(document))
                            {
                                return document.HasMinimumCSharpVersion(LanguageVersion.CSharp9)
                                       ? UnaryNot(condition)
                                       : LogicalNot(condition);
                            }

                            return condition.Expression.WithoutTrivia();
                        }
                    }

                    return IsFalsePattern(condition);
                }

                case UnaryPatternSyntax pattern:
                {
                    if (pattern.IsKind(SyntaxKind.NotPattern))
                    {
                        if (pattern.IsPatternCheckFor(SyntaxKind.TrueLiteralExpression))
                        {
                            var expression = condition.Expression;

                            var type = expression.GetTypeSymbol(document);

                            if (type.IsNullable() is false)
                            {
                                return expression.WithoutTrivia();
                            }
                        }

                        return condition.WithPattern(pattern.Pattern);
                    }

                    return document.HasMinimumCSharpVersion(LanguageVersion.CSharp9)
                           ? UnaryNot(condition)
                           : LogicalNot(condition);
                }

                default:
                {
                    return IsFalsePattern(condition);
                }
            }
        }
    }
}
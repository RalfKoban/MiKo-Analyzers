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

        /// <summary>
        /// Creates an argument from the specified identifier.
        /// </summary>
        /// <param name="identifier">
        /// The identifier to use for the argument.
        /// </param>
        /// <returns>
        /// The argument syntax node.
        /// </returns>
        protected static ArgumentSyntax Argument(string identifier) => Argument(IdentifierName(identifier));

        /// <summary>
        /// Creates an argument from the specified parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to use for the argument.
        /// </param>
        /// <returns>
        /// The argument syntax node.
        /// </returns>
        protected static ArgumentSyntax Argument(ParameterSyntax parameter) => Argument(IdentifierName(parameter.GetName()));

        /// <summary>
        /// Creates an argument from the specified expression.
        /// </summary>
        /// <param name="expression">
        /// The expression to use for the argument.
        /// </param>
        /// <returns>
        /// The argument syntax node.
        /// </returns>
        protected static ArgumentSyntax Argument(ExpressionSyntax expression) => SyntaxFactory.Argument(expression);

        /// <summary>
        /// Creates an argument from the specified member access expression and arguments.
        /// </summary>
        /// <param name="expression">
        /// The member access expression to use.
        /// </param>
        /// <param name="arguments">
        /// The arguments to pass to the invocation.
        /// </param>
        /// <returns>
        /// The argument syntax node.
        /// </returns>
        protected static ArgumentSyntax Argument(MemberAccessExpressionSyntax expression, params ArgumentSyntax[] arguments)
        {
            var syntax = arguments.Length is 0
                         ? (ExpressionSyntax)expression // we do not want to have any empty argument list
                         : Invocation(expression, arguments);

            return Argument(syntax);
        }

        /// <summary>
        /// Creates an argument from the specified type name, method name and arguments.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        /// <param name="arguments">
        /// The arguments to pass to the invocation.
        /// </param>
        /// <returns>
        /// The argument syntax node.
        /// </returns>
        protected static ArgumentSyntax Argument(string typeName, string methodName, params ArgumentSyntax[] arguments)
        {
            return Argument(Member(typeName, methodName), arguments);
        }

        /// <summary>
        /// Creates an argument with a cast to the specified predefined type.
        /// </summary>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind of the predefined type.
        /// </param>
        /// <param name="parameter">
        /// The parameter to cast.
        /// </param>
        /// <returns>
        /// The argument syntax node with a cast expression that casts the specified parameter/identifier to the specified type.
        /// </returns>
        protected static ArgumentSyntax ArgumentWithCast(in SyntaxKind kind, ParameterSyntax parameter) => ArgumentWithCast(PredefinedType(kind), parameter);

        /// <summary>
        /// Creates an argument with a cast to the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to cast to.
        /// </param>
        /// <param name="parameter">
        /// The parameter to cast.
        /// </param>
        /// <returns>
        /// The argument syntax node with a cast expression.
        /// </returns>
        protected static ArgumentSyntax ArgumentWithCast(TypeSyntax type, ParameterSyntax parameter) => ArgumentWithCast(type, IdentifierName(parameter.GetName()));

        /// <summary>
        /// Creates an argument with a cast to the specified predefined type.
        /// </summary>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind of the predefined type.
        /// </param>
        /// <param name="identifier">
        /// The identifier to cast.
        /// </param>
        /// <returns>
        /// The argument syntax node with a cast expression.
        /// </returns>
        protected static ArgumentSyntax ArgumentWithCast(in SyntaxKind kind, IdentifierNameSyntax identifier) => ArgumentWithCast(PredefinedType(kind), identifier);

        /// <summary>
        /// Creates an argument with a cast to the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to cast to.
        /// </param>
        /// <param name="identifier">
        /// The identifier to cast.
        /// </param>
        /// <returns>
        /// The argument syntax node with a cast expression.
        /// </returns>
        protected static ArgumentSyntax ArgumentWithCast(TypeSyntax type, IdentifierNameSyntax identifier) => Argument(SyntaxFactory.CastExpression(type, identifier));

        /// <summary>
        /// Creates an invocation expression for the specified type name, method name and arguments.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        /// <param name="arguments">
        /// The arguments to pass to the invocation.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(string typeName, string methodName, params ArgumentSyntax[] arguments)
        {
            // that's for the method call
            var member = Member(typeName, methodName);

            return Invocation(member, arguments);
        }

        /// <summary>
        /// Creates an invocation expression for the specified type name, property name, method name and type arguments.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        /// <param name="types">
        /// The generic type arguments.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(string typeName, string propertyName, string methodName, params TypeSyntax[] types)
        {
            // that's for the method call
            var member = Member(typeName, propertyName, methodName, types);

            return Invocation(member);
        }

        /// <summary>
        /// Creates an invocation expression for the specified type name, method name, generic type argument and arguments.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        /// <param name="type">
        /// The generic type argument.
        /// </param>
        /// <param name="arguments">
        /// The arguments to pass to the invocation.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(string typeName, string methodName, TypeSyntax type, params ArgumentSyntax[] arguments)
        {
            return Invocation(typeName, methodName, new[] { type }, arguments);
        }

        /// <summary>
        /// Creates an invocation expression for the specified type name, method name, generic type arguments and arguments.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        /// <param name="types">
        /// The generic type arguments.
        /// </param>
        /// <param name="arguments">
        /// The arguments to pass to the invocation.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(string typeName, string methodName, TypeSyntax[] types, params ArgumentSyntax[] arguments)
        {
            // that's for the method call
            var member = Member(typeName, methodName, types);

            return Invocation(member, arguments);
        }

        /// <summary>
        /// Creates a member access expression for the specified predefined type and method name.
        /// </summary>
        /// <param name="type">
        /// The predefined type.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// The member access expression syntax node.
        /// </returns>
        protected static MemberAccessExpressionSyntax Member(PredefinedTypeSyntax type, string methodName)
        {
            var method = IdentifierName(methodName);

            return Member(type, method);
        }

        /// <summary>
        /// Creates a member access expression for the specified type name and method name.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        /// <returns>
        /// The member access expression syntax node.
        /// </returns>
        protected static MemberAccessExpressionSyntax Member(string typeName, string methodName)
        {
            var type = IdentifierName(typeName);
            var method = IdentifierName(methodName);

            return Member(type, method);
        }

        /// <summary>
        /// Creates a member access expression for the specified type name, middle part, method name and generic type arguments.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <param name="middlePart">
        /// The middle part of the member access chain.
        /// </param>
        /// <param name="methodName">
        /// The name of the method.
        /// </param>
        /// <param name="items">
        /// The generic type arguments.
        /// </param>
        /// <returns>
        /// The member access expression syntax node.
        /// </returns>
        protected static MemberAccessExpressionSyntax Member(string typeName, string middlePart, string methodName, TypeSyntax[] items)
        {
            var type = IdentifierName(typeName);
            var method = GenericName(methodName, items);

            var expression = Member(type, middlePart);

            return Member(expression, method);
        }

        /// <summary>
        /// Creates a member access expression for the specified type name and method names.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <param name="methodNames">
        /// The names of the methods to chain.
        /// </param>
        /// <returns>
        /// The member access expression syntax node.
        /// </returns>
        protected static MemberAccessExpressionSyntax Member(string typeName, params string[] methodNames)
        {
            var start = Member(typeName, methodNames[0]);

            var result = methodNames.Skip(1).Aggregate(start, Member);

            return result;
        }

        /// <summary>
        /// Inverts the specified condition expression.
        /// </summary>
        /// <param name="document">
        /// The document containing the condition.
        /// </param>
        /// <param name="condition">
        /// The condition expression to invert.
        /// </param>
        /// <returns>
        /// The expression syntax node representing the inverted condition.
        /// </returns>
        /// <remarks>
        /// For example, <c>a == b</c> becomes <c>a != b</c>, <c>!x</c> becomes <c>x</c>, and <c>!a || !b</c> becomes <c>a &amp;&amp; b</c>.
        /// </remarks>
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

#pragma warning disable CA1021
        /// <summary>
        /// Creates a local variable declaration statement.
        /// </summary>
        /// <param name="type">
        /// The type of the variable.
        /// </param>
        /// <param name="variableName">
        /// The name of the variable.
        /// </param>
        /// <param name="declarator">
        /// On successful return, contains the variable declarator that is created.
        /// </param>
        /// <returns>
        /// The local declaration statement syntax node.
        /// </returns>
        protected static LocalDeclarationStatementSyntax LocalVariable(TypeSyntax type, string variableName, out VariableDeclaratorSyntax declarator)
        {
            declarator = SyntaxFactory.VariableDeclarator(variableName);

            var declaration = SyntaxFactory.VariableDeclaration(type, declarator.ToSeparatedSyntaxList());

            return SyntaxFactory.LocalDeclarationStatement(declaration);
        }
#pragma warning restore CA1021

        /// <summary>
        /// Creates a local variable declaration statement with an initializer.
        /// </summary>
        /// <param name="type">
        /// The type of the variable.
        /// </param>
        /// <param name="variableName">
        /// The name of the variable.
        /// </param>
        /// <param name="value">
        /// The initial value of the variable.
        /// </param>
        /// <returns>
        /// The local declaration statement syntax node.
        /// </returns>
        protected static LocalDeclarationStatementSyntax LocalVariable(TypeSyntax type, string variableName, ExpressionSyntax value)
        {
            var declarator = SyntaxFactory.VariableDeclarator(variableName).WithInitializer(SyntaxFactory.EqualsValueClause(value));
            var declaration = SyntaxFactory.VariableDeclaration(type, declarator.ToSeparatedSyntaxList());

            return SyntaxFactory.LocalDeclarationStatement(declaration);
        }

        /// <summary>
        /// Creates a logical NOT expression for the specified expression.
        /// </summary>
        /// <param name="expression">
        /// The expression to negate.
        /// </param>
        /// <returns>
        /// The prefix unary expression syntax node representing the logical NOT operation.
        /// The expression is automatically wrapped in parentheses.
        /// </returns>
        protected static ExpressionSyntax LogicalNot(ExpressionSyntax expression) => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(expression));

        /// <summary>
        /// Creates a negated pattern expression from the specified pattern expression.
        /// </summary>
        /// <param name="expression">
        /// The pattern expression to negate.
        /// </param>
        /// <returns>
        /// The pattern expression syntax node with a negated pattern using the <c>not</c> keyword.
        /// </returns>
        protected static IsPatternExpressionSyntax UnaryNot(IsPatternExpressionSyntax expression) => expression.WithPattern(UnaryNot(expression.Pattern));

        /// <summary>
        /// Creates a unary NOT pattern from the specified pattern.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to negate.
        /// </param>
        /// <returns>
        /// The unary pattern syntax node.
        /// </returns>
        protected static UnaryPatternSyntax UnaryNot(PatternSyntax pattern) => SyntaxFactory.UnaryPattern(SyntaxKind.NotKeyword.AsToken().WithTrailingSpace(), pattern);

        /// <summary>
        /// Creates a character literal expression.
        /// </summary>
        /// <param name="value">
        /// The character value.
        /// </param>
        /// <returns>
        /// The literal expression syntax node.
        /// </returns>
        protected static LiteralExpressionSyntax Literal(in char value) => Literal(SyntaxFactory.Literal(value));

        /// <summary>
        /// Creates a decimal literal expression.
        /// </summary>
        /// <param name="value">
        /// The decimal value.
        /// </param>
        /// <returns>
        /// The literal expression syntax node.
        /// </returns>
        protected static LiteralExpressionSyntax Literal(decimal value) => Literal(SyntaxFactory.Literal(value));

        /// <summary>
        /// Creates an integer literal expression.
        /// </summary>
        /// <param name="value">
        /// The integer value.
        /// </param>
        /// <returns>
        /// The literal expression syntax node.
        /// </returns>
        protected static LiteralExpressionSyntax Literal(in int value) => Literal(SyntaxFactory.Literal(value));

        /// <summary>
        /// Creates a literal expression from the specified token.
        /// </summary>
        /// <param name="token">
        /// The syntax token representing the literal.
        /// </param>
        /// <returns>
        /// The literal expression syntax node.
        /// </returns>
        protected static LiteralExpressionSyntax Literal(in SyntaxToken token) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, token);

        /// <summary>
        /// Creates an integer literal expression with a specific text representation.
        /// </summary>
        /// <param name="value">
        /// The integer value.
        /// </param>
        /// <param name="valueRepresentation">
        /// The text representation of the value.
        /// </param>
        /// <returns>
        /// The literal expression syntax node.
        /// </returns>
        protected static LiteralExpressionSyntax Literal(in int value, string valueRepresentation) => Literal(SyntaxFactory.Literal(valueRepresentation, value));

        /// <summary>
        /// Creates a string literal expression.
        /// </summary>
        /// <param name="text">
        /// The string value.
        /// </param>
        /// <returns>
        /// The literal expression syntax node.
        /// </returns>
        protected static LiteralExpressionSyntax StringLiteral(string text)
        {
            var token = text.SurroundedWithDoubleQuote().AsToken();

            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, token);
        }

        /// <summary>
        /// Creates a typeof expression for the specified parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to get the type of.
        /// </param>
        /// <returns>
        /// The typeof expression syntax node.
        /// </returns>
        protected static TypeOfExpressionSyntax TypeOf(ParameterSyntax parameter)
        {
            var typeSyntax = parameter.Type;

            return typeSyntax is null
                   ? TypeOf(SyntaxKind.VoidKeyword)
                   : TypeOf(typeSyntax);
        }

        /// <summary>
        /// Creates a typeof expression for the specified predefined type.
        /// </summary>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind of the predefined type.
        /// </param>
        /// <returns>
        /// The typeof expression syntax node.
        /// </returns>
        protected static TypeOfExpressionSyntax TypeOf(in SyntaxKind kind) => TypeOf(PredefinedType(kind));

        /// <summary>
        /// Creates a typeof expression for the specified type name.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <returns>
        /// The typeof expression syntax node.
        /// </returns>
        protected static TypeOfExpressionSyntax TypeOf(string typeName) => TypeOf(typeName.AsTypeSyntax());

        /// <summary>
        /// Creates a typeof expression for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to get the type of.
        /// </param>
        /// <returns>
        /// The typeof expression syntax node.
        /// </returns>
        protected static TypeOfExpressionSyntax TypeOf(TypeSyntax type) => SyntaxFactory.TypeOfExpression(type);

        /// <summary>
        /// Creates a nameof expression for the specified literal expression.
        /// </summary>
        /// <param name="literal">
        /// The literal expression to get the name of.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node representing the nameof operation.
        /// </returns>
        protected static InvocationExpressionSyntax NameOf(LiteralExpressionSyntax literal)
        {
            var identifierName = literal.GetName();

            return NameOf(identifierName);
        }

        /// <summary>
        /// Creates a nameof expression for the specified identifier name.
        /// </summary>
        /// <param name="identifierName">
        /// The name of the identifier.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node representing the nameof operation.
        /// </returns>
        protected static InvocationExpressionSyntax NameOf(string identifierName)
        {
            var syntax = IdentifierName(identifierName);

            return NameOf(syntax);
        }

        /// <summary>
        /// Creates a nameof expression for the specified type name and identifier name.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type.
        /// </param>
        /// <param name="identifierName">
        /// The name of the identifier.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node representing the nameof operation.
        /// </returns>
        protected static InvocationExpressionSyntax NameOf(string typeName, string identifierName)
        {
            var syntax = Member(typeName, identifierName);

            return NameOf(syntax);
        }

        /// <summary>
        /// Creates a nameof expression for the specified type and identifier name.
        /// </summary>
        /// <param name="type">
        /// The type syntax.
        /// </param>
        /// <param name="identifierName">
        /// The name of the identifier.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node representing the nameof operation.
        /// </returns>
        protected static InvocationExpressionSyntax NameOf(TypeSyntax type, string identifierName)
        {
            var typeName = type.GetNameOnlyPart();

            return NameOf(typeName, identifierName);
        }

        /// <summary>
        /// Creates a nameof expression for the specified type and literal expression.
        /// </summary>
        /// <param name="type">
        /// The type syntax.
        /// </param>
        /// <param name="literal">
        /// The literal expression to get the name of.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node representing the nameof operation.
        /// </returns>
        protected static InvocationExpressionSyntax NameOf(TypeSyntax type, LiteralExpressionSyntax literal)
        {
            var typeName = type.GetNameOnlyPart();
            var identifierName = literal.GetName();

            return NameOf(typeName, identifierName);
        }

        /// <summary>
        /// Creates a nameof expression for the specified type symbol and literal expression.
        /// </summary>
        /// <param name="type">
        /// The type symbol.
        /// </param>
        /// <param name="literal">
        /// The literal expression to get the name of.
        /// </param>
        /// <returns>
        /// The invocation expression syntax node representing the nameof operation.
        /// </returns>
        protected static InvocationExpressionSyntax NameOf(ITypeSymbol type, LiteralExpressionSyntax literal)
        {
            var typeName = type.Name;
            var identifierName = literal.GetName();

            return NameOf(typeName, identifierName);
        }

        /// <summary>
        /// Creates a parenthesized lambda expression with the specified block.
        /// </summary>
        /// <param name="block">
        /// The block to use as the lambda body.
        /// </param>
        /// <returns>
        /// The parenthesized lambda expression syntax node.
        /// </returns>
        protected static ParenthesizedLambdaExpressionSyntax ParenthesizedLambda(BlockSyntax block) => SyntaxFactory.ParenthesizedLambdaExpression(block);

        private static InvocationExpressionSyntax NameOf(ExpressionSyntax syntax)
        {
            // nameof has a special RawContextualKind, hence we have to create it via its specific SyntaxKind
            // (see https://stackoverflow.com/questions/46259039/constructing-nameof-expression-via-syntaxfactory-roslyn)
            var nameofSyntax = SyntaxFactory.Identifier(SyntaxFactory.TriviaList(), SyntaxKind.NameOfKeyword, "nameof", "nameof", SyntaxFactory.TriviaList());

            return Invocation(IdentifierName(nameofSyntax), Argument(syntax));
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
﻿using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityCodeFixProvider : MiKoCodeFixProvider
    {
        protected static ArgumentSyntax Argument(ExpressionSyntax expression) => SyntaxFactory.Argument(expression);

        protected static ArgumentSyntax Argument(MemberAccessExpressionSyntax expression, ArgumentSyntax argument) => Argument(Invocation(expression, argument));

        protected static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments) => SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments));

        protected static InvocationExpressionSyntax Invocation(MemberAccessExpressionSyntax member, params ArgumentSyntax[] arguments)
        {
            // that's for the argument
            var argumentList = ArgumentList(arguments);

            // combine both to complete call
            return SyntaxFactory.InvocationExpression(member, argumentList);
        }

        protected static InvocationExpressionSyntax Invocation(string typeName, string methodName, params ArgumentSyntax[] arguments)
        {
            // that's for the method call
            var member = SimpleMemberAccess(typeName, methodName);

            return Invocation(member, arguments);
        }

        protected static InvocationExpressionSyntax Invocation(string typeName, string methodName, params TypeSyntax[] items)
        {
            // that's for the method call
            var member = SimpleMemberAccess(typeName, methodName, items);

            return Invocation(member);
        }

        protected static InvocationExpressionSyntax Invocation(string typeName, string propertyName, string methodName, params TypeSyntax[] items)
        {
            // that's for the method call
            var member = SimpleMemberAccess(typeName, propertyName, methodName, items);

            return Invocation(member);
        }

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(string typeName, string methodName)
        {
            var type = SyntaxFactory.IdentifierName(typeName);
            var method = SyntaxFactory.IdentifierName(methodName);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);
        }

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(ExpressionSyntax syntax, string name)
        {
            var identifierName = SyntaxFactory.IdentifierName(name);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, syntax, identifierName);
        }

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(string typeName, string methodName, TypeSyntax[] items)
        {
            var type = SyntaxFactory.IdentifierName(typeName);
            var method = SyntaxFactory.GenericName(methodName).AddTypeArgumentListArguments(items);

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

        protected static SyntaxTokenList TokenList(params SyntaxKind[] syntaxKinds) => SyntaxFactory.TokenList(syntaxKinds.Select(SyntaxFactory.Token));

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

        private static InvocationExpressionSyntax NameOf(ExpressionSyntax syntax)
        {
            // nameof has a special RawContextualKind, hence we have to create it via its specific SyntaxKind
            // (see https://stackoverflow.com/questions/46259039/constructing-nameof-expression-via-syntaxfactory-roslyn)
            var nameofSyntax = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(SyntaxFactory.TriviaList(), SyntaxKind.NameOfKeyword, "nameof", "nameof", SyntaxFactory.TriviaList()));

            return SyntaxFactory.InvocationExpression(nameofSyntax, ArgumentList(SyntaxFactory.Argument(syntax)));
        }
    }
}
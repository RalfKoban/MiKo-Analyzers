using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityCodeFixProvider : MiKoCodeFixProvider
    {
        protected static InvocationExpressionSyntax CreateInvocationSyntax(MemberAccessExpressionSyntax member, params ArgumentSyntax[] arguments)
        {
            // that's for the argument
            var argumentList = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments));

            // combine both to complete call
            return SyntaxFactory.InvocationExpression(member, argumentList);
        }

        protected static InvocationExpressionSyntax CreateInvocationSyntax(string typeName, string methodName, params ArgumentSyntax[] arguments)
        {
            // that's for the method call
            var member = CreateSimpleMemberAccessExpressionSyntax(typeName, methodName);

            return CreateInvocationSyntax(member, arguments);
        }

        protected static InvocationExpressionSyntax CreateInvocationSyntax(string typeName, string methodName, params TypeSyntax[] items)
        {
            // that's for the method call
            var member = CreateSimpleMemberAccessExpressionSyntax(typeName, methodName, items);

            return CreateInvocationSyntax(member);
        }

        protected static MemberAccessExpressionSyntax CreateSimpleMemberAccessExpressionSyntax(string typeName, string methodName)
        {
            var type = SyntaxFactory.IdentifierName(typeName);
            var method = SyntaxFactory.IdentifierName(methodName);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);
        }

        protected static MemberAccessExpressionSyntax CreateSimpleMemberAccessExpressionSyntax(ExpressionSyntax syntax, string name)
        {
            var identifierName = SyntaxFactory.IdentifierName(name);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, syntax, identifierName);
        }

        protected static MemberAccessExpressionSyntax CreateSimpleMemberAccessExpressionSyntax(string typeName, string methodName, TypeSyntax[] items)
        {
            var type = SyntaxFactory.IdentifierName(typeName);
            var method = SyntaxFactory.GenericName(methodName).AddTypeArgumentListArguments(items);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);
        }

        protected static MemberAccessExpressionSyntax CreateSimpleMemberAccessExpressionSyntax(string typeName, params string[] methodNames)
        {
            var start = CreateSimpleMemberAccessExpressionSyntax(typeName, methodNames[0]);

            var result = methodNames.Skip(1).Aggregate(start, CreateSimpleMemberAccessExpressionSyntax);
            return result;
        }
    }
}
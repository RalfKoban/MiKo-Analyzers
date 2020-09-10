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

        protected static MemberAccessExpressionSyntax CreateSimpleMemberAccessExpressionSyntax(string typeName, string methodName)
        {
            var type = SyntaxFactory.IdentifierName(typeName);
            var method = SyntaxFactory.IdentifierName(methodName);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);
        }

        protected static MemberAccessExpressionSyntax CreateSimpleMemberAccessExpressionSyntax(string typeName, string methodName, string methodName1)
        {
            var start = CreateSimpleMemberAccessExpressionSyntax(typeName, methodName);
            var method = SyntaxFactory.IdentifierName(methodName1);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, start, method);
        }

        protected static MemberAccessExpressionSyntax CreateSimpleMemberAccessExpressionSyntax(string typeName, string methodName, string methodName1, string methodName2)
        {
            var start = CreateSimpleMemberAccessExpressionSyntax(typeName, methodName, methodName1);
            var method = SyntaxFactory.IdentifierName(methodName2);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, start, method);
        }
    }
}
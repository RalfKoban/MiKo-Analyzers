using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityCodeFixProvider : MiKoCodeFixProvider
    {
        protected static ArgumentListSyntax CreateArgumentList(params ArgumentSyntax[] arguments) => SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments));

        protected static InvocationExpressionSyntax CreateInvocationSyntax(MemberAccessExpressionSyntax member, params ArgumentSyntax[] arguments)
        {
            // that's for the argument
            var argumentList = CreateArgumentList(arguments);

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

        protected static InvocationExpressionSyntax CreateNameofExpression(string identifierName)
        {
            // nameof has a special RawContextualKind, hence we have to create it via its specific SyntaxKind
            // (see https://stackoverflow.com/questions/46259039/constructing-nameof-expression-via-syntaxfactory-roslyn)
            var nameofIdentifier = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(SyntaxFactory.TriviaList(), SyntaxKind.NameOfKeyword, "nameof", "nameof", SyntaxFactory.TriviaList()));

            return SyntaxFactory.InvocationExpression(nameofIdentifier, CreateArgumentList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(identifierName))));
        }
    }
}
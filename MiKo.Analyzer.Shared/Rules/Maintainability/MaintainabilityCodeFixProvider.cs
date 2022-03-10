using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityCodeFixProvider : MiKoCodeFixProvider
    {
        protected static ArgumentSyntax Argument(ExpressionSyntax expression) => SyntaxFactory.Argument(expression);

        protected static ArgumentSyntax Argument(ParameterSyntax parameter) => Argument(SyntaxFactory.IdentifierName(parameter.GetName()));

        protected static ArgumentSyntax Argument(string identifier) => Argument(SyntaxFactory.IdentifierName(identifier));

        protected static ArgumentSyntax Argument(MemberAccessExpressionSyntax expression, ArgumentSyntax argument) => Argument(Invocation(expression, argument));

        protected static ArgumentSyntax ArgumentWithCast(SyntaxKind kind, ParameterSyntax parameter) => ArgumentWithCast(PredefinedType(kind), parameter);

        protected static ArgumentSyntax ArgumentWithCast(TypeSyntax type, ParameterSyntax parameter) => ArgumentWithCast(type, SyntaxFactory.IdentifierName(parameter.GetName()));

        protected static ArgumentSyntax ArgumentWithCast(SyntaxKind kind, IdentifierNameSyntax identifier) => ArgumentWithCast(PredefinedType(kind), identifier);

        protected static ArgumentSyntax ArgumentWithCast(TypeSyntax type, IdentifierNameSyntax identifier) => Argument(SyntaxFactory.CastExpression(type, identifier));

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

        protected static PredefinedTypeSyntax PredefinedType(SyntaxKind kind) => SyntaxFactory.PredefinedType(SyntaxFactory.Token(kind));

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

        protected static LiteralExpressionSyntax Literal(SyntaxKind expressionKind) => SyntaxFactory.LiteralExpression(expressionKind);

        protected static LiteralExpressionSyntax Literal(SyntaxToken token) => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, token);

        protected static LiteralExpressionSyntax StringLiteral(string text)
        {
            var token = text.SurroundedWithDoubleQuote().ToSyntaxToken();

            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, token);
        }

        protected static SyntaxTokenList TokenList(params SyntaxKind[] syntaxKinds) => SyntaxFactory.TokenList(syntaxKinds.Select(SyntaxFactory.Token));

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

        protected static SyntaxNode WithUsing(SyntaxNode root, string usingNamespace)
        {
            var usings = root.DescendantNodes<UsingDirectiveSyntax>().ToList();

            if (usings.Any(_ => _.Name.ToFullString() == usingNamespace))
            {
                // already set
                return root;
            }

            var directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingNamespace));

            if (usings.Count == 0)
            {
                return root.InsertNodeBefore(root.ChildNodes().First(), directive);
            }

            // add using at correct place inside the using block
            var usingOrientation = usings.FirstOrDefault(_ => string.Compare(_.Name.ToFullString(), usingNamespace, StringComparison.OrdinalIgnoreCase) > 0);

            return usingOrientation != null
                       ? root.InsertNodeBefore(usingOrientation, directive)
                       : root.InsertNodeAfter(usings.Last(), directive);
        }

        protected static SyntaxNode WithoutUsing(SyntaxNode node, string usingNamespace)
        {
            var root = node.SyntaxTree.GetRoot();

            return root.DescendantNodes<UsingDirectiveSyntax>(_ => _.Name.ToFullString() == usingNamespace)
                       .Select(root.Without)
                       .FirstOrDefault();
        }

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
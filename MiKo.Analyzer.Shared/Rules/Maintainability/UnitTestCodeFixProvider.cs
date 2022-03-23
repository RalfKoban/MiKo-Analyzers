using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UnitTestCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected static InvocationExpressionSyntax AssertThat(ExpressionSyntax expression, ArgumentSyntax constraint, SeparatedSyntaxList<ArgumentSyntax> arguments)
            => AssertThat(Argument(expression), constraint, arguments, 1); // skip the first argument

        protected static InvocationExpressionSyntax AssertThat(ArgumentSyntax argument, ArgumentSyntax constraint, SeparatedSyntaxList<ArgumentSyntax> arguments, int skip = 2) // skip both arguments in the original call as we have to correct those
        {
            var args = new List<ArgumentSyntax>(Math.Max(2, 2 + arguments.Count - skip));
            args.Add(argument);
            args.Add(constraint);

            if (arguments.Count > skip)
            {
                args.AddRange(arguments.Skip(skip));
            }

            return AssertThat(args.ToArray());
        }

        protected static InvocationExpressionSyntax AssertThat(params ArgumentSyntax[] arguments) => Invocation("Assert", "That", arguments);

        protected static InvocationExpressionSyntax InvocationIs(string name, ArgumentSyntax argument) => Invocation("Is", name, argument);

        protected static ArgumentSyntax Is(string name) => Argument(MemberIs(name));

        protected static ArgumentSyntax Is(string name, ArgumentSyntax argument) => Argument(InvocationIs(name, argument));

        protected static ArgumentSyntax Is(string name, ExpressionSyntax expression) => Is(name, Argument(expression));

        protected static ArgumentSyntax Is(string name, TypeSyntax[] items) => Argument(Invocation("Is", name, items));

        protected static ArgumentSyntax Is(string name, string name1, TypeSyntax[] items) => Argument(Invocation("Is", name, name1, items));

        protected static ArgumentSyntax Is(string name, string name1, ArgumentSyntax argument) => Argument(MemberIs(name, name1), argument);

        protected static ArgumentSyntax Is(string name, ArgumentSyntax argument, string name1)
        {
            var expression = InvocationIs(name, argument);

            return Argument(SimpleMemberAccess(expression, name1));
        }

        protected static ArgumentSyntax Is(string name, string name1, ExpressionSyntax expression) => Is(name, name1, Argument(expression));

        protected static ArgumentSyntax Is(string name, string name1, ArgumentSyntax argument, string name2)
        {
            var expression = MemberIs(name, name1);
            var invocation = Invocation(expression, argument);

            return Argument(SimpleMemberAccess(invocation, name2));
        }

        protected static ArgumentSyntax Is(string name, ArgumentSyntax argument, string name1, ArgumentSyntax argument1)
        {
            var isCall = InvocationIs(name, argument);
            var appendixCall = SimpleMemberAccess(isCall, name1);

            return Argument(appendixCall, argument1);
        }

        protected static ArgumentSyntax Is(params string[] names) => Argument(MemberIs(names));

        protected static bool IsNumeric(ArgumentSyntax argument) => argument.Expression.IsKind(SyntaxKind.NumericLiteralExpression)
                                                                  || (argument.Expression is MemberAccessExpressionSyntax mae && mae.Expression.IsKind(SyntaxKind.PredefinedType));

        protected static ArgumentSyntax Does(string name, ArgumentSyntax argument) => Argument(Invocation("Does", name, argument));

        protected static ArgumentSyntax Does(string name, string name1, ArgumentSyntax argument) => Argument(MemberDoes(name, name1), argument);

        protected static ArgumentSyntax Does(params string[] names) => Argument(MemberDoes(names));

        protected static ArgumentSyntax Has(string name, ArgumentSyntax argument) => Argument(MemberHas(name), argument);

        protected static ArgumentSyntax Has(string name, ArgumentSyntax argument, string name1)
        {
            var hasCall = Invocation(MemberHas(name), argument);
            var appendixCall = SimpleMemberAccess(hasCall, name1);

            return Argument(appendixCall);
        }

        protected static ArgumentSyntax HasCount(string name, ArgumentSyntax argument) => Argument(MemberHas("Count", name), argument);

        protected static ArgumentSyntax HasCount(string name, string name1, ArgumentSyntax argument) => Argument(MemberHas("Count", name, name1), argument);

        protected static ArgumentSyntax HasCount(string name, string name1, ExpressionSyntax expression) => Argument(MemberHas("Count", name, name1), Argument(expression));

        protected static MemberAccessExpressionSyntax MemberDoes(params string[] names) => SimpleMemberAccess("Does", names);

        protected static MemberAccessExpressionSyntax MemberHas(params string[] names) => SimpleMemberAccess("Has", names);

        protected static MemberAccessExpressionSyntax MemberIs(params string[] names) => SimpleMemberAccess("Is", names);

        protected static ArgumentSyntax Throws(string name) => Argument(SimpleMemberAccess("Throws", name));

        protected static ArgumentSyntax Throws(string name, params TypeSyntax[] types) => Argument(Invocation("Throws", name, types));

        protected static TypeSyntax[] GetTypeSyntaxes(InvocationExpressionSyntax i, SimpleNameSyntax name)
        {
            if (name is GenericNameSyntax g)
            {
                return g.TypeArgumentList.Arguments.ToArray();
            }

            var arguments = i.ArgumentList.Arguments;
            if (arguments.Count == 1 && arguments[0].Expression is TypeOfExpressionSyntax t)
            {
                return new[] { t.Type };
            }

            return Array.Empty<TypeSyntax>();
        }
    }
}
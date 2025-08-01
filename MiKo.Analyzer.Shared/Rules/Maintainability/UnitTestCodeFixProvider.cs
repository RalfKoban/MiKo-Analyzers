﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UnitTestCodeFixProvider : MaintainabilityCodeFixProvider
    {
        private const int FormatIdentifierLength = 3; // '{0}' has length of 3

        private static readonly string[] FormatIdentifiers = { "{0}", "{1}", "{2}", "{3}", "{4}", "{5}", "{6}", "{7}", "{8}", "{9}" };

        protected static InvocationExpressionSyntax AssertThat(ExpressionSyntax expression, ArgumentSyntax constraint, in SeparatedSyntaxList<ArgumentSyntax> arguments, in int skip = 1, in bool removeNameColon = false) // skip the first argument
            => AssertThat(Argument(expression), constraint, arguments, skip, removeNameColon);

        protected static InvocationExpressionSyntax AssertThat(ArgumentSyntax argument, ArgumentSyntax constraint, in SeparatedSyntaxList<ArgumentSyntax> arguments, in int skip = 2, in bool removeNameColon = false) // skip both arguments in the original call as we have to correct those
        {
            var args = new List<ArgumentSyntax>(Math.Max(2, 2 + arguments.Count - skip))
                           {
                               argument,
                               constraint,
                           };

            if (arguments.Count > skip)
            {
                var otherArguments = removeNameColon
                                     ? arguments.Skip(skip).Select(_ => _.WithNameColon(null))
                                     : arguments.Skip(skip);

                args.Add(ConvertToInterpolatedStringArgument(otherArguments.ToList()));
            }

            return AssertThat(args.ToArray());
        }

        protected static InvocationExpressionSyntax AssertThat(params ArgumentSyntax[] arguments) => Invocation("Assert", "That", arguments);

        protected static InvocationExpressionSyntax InvocationIs(string name, params ArgumentSyntax[] arguments) => Invocation("Is", name, arguments);

        protected static ArgumentSyntax Is(string name) => Argument(MemberIs(name));

        protected static ArgumentSyntax Is(string name, ArgumentSyntax argument) => Argument(InvocationIs(name, argument));

        protected static ArgumentSyntax Is(string name, ExpressionSyntax expression) => Is(name, Argument(expression));

        protected static ArgumentSyntax Is(string name, TypeSyntax[] items) => Argument(Invocation("Is", name, items));

        protected static ArgumentSyntax Is(string name, ArgumentSyntax argument, ArgumentSyntax argument1) => Argument(InvocationIs(name, argument, argument1));

        protected static ArgumentSyntax Is(string name, string name1, TypeSyntax[] items) => Argument(Invocation("Is", name, name1, items));

        protected static ArgumentSyntax Is(string name, string name1, ArgumentSyntax argument) => Argument(MemberIs(name, name1), argument);

        protected static ArgumentSyntax Is(string name, ArgumentSyntax argument, string name1)
        {
            var expression = InvocationIs(name, argument);

            return Argument(Member(expression, name1));
        }

        protected static ArgumentSyntax Is(string name, string name1, ExpressionSyntax expression) => Is(name, name1, Argument(expression));

        protected static ArgumentSyntax Is(string name, string name1, ArgumentSyntax argument, ArgumentSyntax argument1) => Argument(MemberIs(name, name1), argument, argument1);

        protected static ArgumentSyntax Is(string name, string name1, ArgumentSyntax argument, string name2)
        {
            var expression = MemberIs(name, name1);
            var invocation = Invocation(expression, argument);

            return Argument(Member(invocation, name2));
        }

        protected static ArgumentSyntax Is(string name, ArgumentSyntax argument, string name1, ArgumentSyntax argument1)
        {
            var isCall = InvocationIs(name, argument);
            var appendixCall = Member(isCall, name1);

            return Argument(appendixCall, argument1);
        }

        protected static ArgumentSyntax Is(string name, string name1, ArgumentSyntax argument, string name2, ArgumentSyntax argument1)
        {
            var expression = MemberIs(name, name1);
            var isCall = Invocation(expression, argument);
            var appendixCall = Member(isCall, name2);

            return Argument(appendixCall, argument1);
        }

        protected static ArgumentSyntax Is(params string[] names) => Argument(MemberIs(names));

        protected static bool IsNumeric(ArgumentSyntax argument) => argument.Expression.IsKind(SyntaxKind.NumericLiteralExpression)
                                                                  || (argument.Expression is MemberAccessExpressionSyntax mae && mae.Expression.IsKind(SyntaxKind.PredefinedType));

        protected static ArgumentSyntax Does(string name, ArgumentSyntax argument) => Argument(Invocation("Does", name, argument));

        protected static ArgumentSyntax Does(string name, ArgumentSyntax argument, string name1)
        {
            var doesCall = Invocation(MemberDoes(name), argument);
            var appendixCall = Member(doesCall, name1);

            return Argument(appendixCall);
        }

        protected static ArgumentSyntax Does(string name, string name1, ArgumentSyntax argument) => Argument(MemberDoes(name, name1), argument);

        protected static ArgumentSyntax Does(string name, string name1, ArgumentSyntax argument, string name2)
        {
            var doesCall = Invocation(MemberDoes(name, name1), argument);
            var appendixCall = Member(doesCall, name2);

            return Argument(appendixCall);
        }

        protected static ArgumentSyntax Does(params string[] names) => Argument(MemberDoes(names));

        protected static ArgumentSyntax Has(string name, ArgumentSyntax argument) => Argument(MemberHas(name), argument);

        protected static ArgumentSyntax Has(string name, ArgumentSyntax argument, string name1)
        {
            var hasCall = Invocation(MemberHas(name), argument);
            var appendixCall = Member(hasCall, name1);

            return Argument(appendixCall);
        }

        protected static ArgumentSyntax Has(string name, string name1, ArgumentSyntax argument) => Argument(MemberHas(name, name1), argument);

        protected static ArgumentSyntax Has(string name, string name1, string name2, ArgumentSyntax argument) => Argument(MemberHas(name, name1, name2), argument);

        protected static ArgumentSyntax Has(string name, string name1, string name2, ExpressionSyntax expression) => Argument(MemberHas(name, name1, name2), Argument(expression));

        protected static ArgumentSyntax Has(string name, string name1, ArgumentSyntax argument, params TypeSyntax[] types) => Argument(Member("Has", name, name1, types), argument);

        protected static MemberAccessExpressionSyntax MemberDoes(params string[] names) => Member("Does", names);

        protected static MemberAccessExpressionSyntax MemberHas(params string[] names) => Member("Has", names);

        protected static MemberAccessExpressionSyntax MemberIs(params string[] names) => Member("Is", names);

        protected static ArgumentSyntax Throws(string name) => Argument("Throws", name);

        protected static ArgumentSyntax Throws(string name, ArgumentSyntax argument) => Argument(Invocation("Throws", name, argument));

        protected static ArgumentSyntax Throws(string name, params TypeSyntax[] types) => Argument(Invocation("Throws", name, types));

        protected static TypeSyntax[] GetTypeSyntaxes(InvocationExpressionSyntax i, SimpleNameSyntax name)
        {
            if (name is GenericNameSyntax g)
            {
                return g.TypeArgumentList.Arguments.ToArray();
            }

            var arguments = i.ArgumentList.Arguments;

            if (arguments.Count is 1 && arguments[0].Expression is TypeOfExpressionSyntax t)
            {
                return new[] { t.Type };
            }

            return Array.Empty<TypeSyntax>();
        }

        private static ArgumentSyntax ConvertToInterpolatedStringArgument(List<ArgumentSyntax> otherArguments)
        {
            var argument = otherArguments[0];

            if (argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                var formatMessage = literal.Token.ValueText;

                if (formatMessage.ContainsAny(FormatIdentifiers))
                {
                    return Argument(ConvertToInterpolatedString(formatMessage.AsSpan(), otherArguments));
                }
            }

            return argument;
        }

        private static InterpolatedStringExpressionSyntax ConvertToInterpolatedString(ReadOnlySpan<char> formatMessage, List<ArgumentSyntax> otherArguments)
        {
            var contents = new List<InterpolatedStringContentSyntax>();

            int index;

            while ((index = formatMessage.IndexOfAny(FormatIdentifiers, StringComparison.Ordinal)) > -1)
            {
                if (index > 0)
                {
                    // we have some text at the start, so add this before the other text
                    contents.Add(formatMessage.Slice(0, index).AsInterpolatedString());
                }

                var identifierNumber = formatMessage.Slice(index, FormatIdentifierLength).TrimStart('{').TrimEnd('}').ToString();

                if (int.TryParse(identifierNumber, out var number) && number < otherArguments.Count)
                {
                    contents.Add(SyntaxFactory.Interpolation(otherArguments[number + 1].Expression));
                }

                formatMessage = formatMessage.Slice(index + FormatIdentifierLength);
            }

            // add remaining text
            if (formatMessage.Length > 0)
            {
                contents.Add(formatMessage.AsInterpolatedString());
            }

            return SyntaxFactory.InterpolatedStringExpression(SyntaxKind.InterpolatedStringStartToken.AsToken(), contents.ToSyntaxList());
        }
    }
}
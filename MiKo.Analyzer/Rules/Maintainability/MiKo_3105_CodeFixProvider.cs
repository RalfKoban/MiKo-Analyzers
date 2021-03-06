﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3105_CodeFixProvider)), Shared]
    public sealed class MiKo_3105_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3105_TestMethodsUseAssertThatAnalyzer.Id;

        protected override string Title => Resources.MiKo_3105_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var original = (InvocationExpressionSyntax)syntax;

            if (original.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax type)
            {
                var typeName = type.GetName();
                switch (typeName)
                {
                    case "Assert":
                    case "CollectionAssert":
                    case "StringAssert":
                    {
                        var args = original.ArgumentList.Arguments;

                        var fixedSyntax = UpdatedSyntax(typeName, maes, args);
                        if (fixedSyntax != null)
                        {
                            // ensure that we keep leading trivia, such as comments
                            return fixedSyntax.WithLeadingTrivia(original.GetLeadingTrivia());
                        }

                        break;
                    }
                }
            }

            return original;
        }

        private static ExpressionSyntax UpdatedSyntax(string typeName, MemberAccessExpressionSyntax syntax, SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var methodName = syntax.GetName();
            switch (methodName)
            {
                case "AllItemsAreNotNull": return FixAllItemsAreNotNull(args);
                case "AllItemsAreUnique": return FixAllItemsAreUnique(args);
                case "AreEqual": return FixAreEqual(args);
                case "AreEqualIgnoringCase": return FixAreEqualIgnoringCase(args);
                case "AreEquivalent": return FixAreEquivalent(args);
                case "AreNotEqual": return FixAreNotEqual(args);
                case "AreNotEquivalent": return FixAreNotEquivalent(args);
                case "AreNotSame": return FixAreNotSame(args);
                case "AreSame": return FixAreSame(args);
                case "Contains": return FixContains(typeName, args);
                case "DoesNotContain": return FixDoesNotContain(typeName, args);
                case "DoesNotEndWith": return FixDoesNotEndWith(args);
                case "DoesNotStartWith": return FixDoesNotStartWith(args);
                case "EndsWith": return FixEndsWith(args);
                case "False": return FixIsFalse(args);
                case "Greater": return FixGreater(args);
                case "GreaterOrEqual": return FixGreaterOrEqual(args);
                case "IsEmpty": return FixIsEmpty(args);
                case "IsFalse": return FixIsFalse(args);
                case "IsInstanceOf": return FixIsInstanceOf(args, syntax.Name);
                case "IsNaN": return FixIsNaN(args);
                case "IsNotEmpty": return FixIsNotEmpty(args);
                case "IsNotNull": return FixIsNotNull(args);
                case "IsNotSubsetOf": return FixIsNotSubsetOf(args);
                case "IsNotSupersetOf": return FixIsNotSupersetOf(args);
                case "IsNull": return FixIsNull(args);
                case "IsNullOrEmpty": return FixIsNullOrEmpty(args);
                case "IsOrdered": return FixIsOrdered(args);
                case "IsSubsetOf": return FixIsSubsetOf(args);
                case "IsSupersetOf": return FixIsSupersetOf(args);
                case "IsTrue": return FixIsTrue(args);
                case "Less": return FixLess(args);
                case "LessOrEqual": return FixLessOrEqual(args);
                case "Positive": return FixPositive(args);
                case "Negative": return FixNegative(args);
                case "NotNull": return FixNotNull(args);
                case "NotZero": return FixNotZero(args);
                case "StartsWith": return FixStartsWith(args);
                case "True": return FixIsTrue(args);
                case "Zero": return FixZero(args);
                default: return null;
            }
        }

        private static InvocationExpressionSyntax FixAllItemsAreNotNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("All", "Not", "Null"), 1, args);

        private static InvocationExpressionSyntax FixAllItemsAreUnique(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Unique"), 1, args);

        private static InvocationExpressionSyntax FixAreEqual(SeparatedSyntaxList<ArgumentSyntax> args) => FixAreEqualOrSame(args, "EqualTo");

        private static InvocationExpressionSyntax FixAreEqualOrSame(SeparatedSyntaxList<ArgumentSyntax> args, string call)
        {
            var arg0 = args[0];
            var arg1 = args[1];

            switch (arg0.Expression.Kind())
            {
                case SyntaxKind.FalseLiteralExpression: return AssertThat(arg1, Is("False"), 2, args);
                case SyntaxKind.TrueLiteralExpression: return AssertThat(arg1, Is("True"), 2, args);
                case SyntaxKind.NullLiteralExpression: return AssertThat(arg1, Is("Null"), 2, args);
                case SyntaxKind.StringLiteralExpression: return AssertThat(arg1, Is(call, arg0), 2, args);
                case SyntaxKind.NumericLiteralExpression:
                {
                    if (args.Count > 2)
                    {
                        var arg2 = args[2];

                        if (IsNumeric(arg2))
                        {
                            // seems we have a tolerance parameter
                            return AssertThat(arg1, Is(call, arg0, "Within", arg2), 3, args);
                        }
                    }

                    return AssertThat(arg1, Is(call, arg0), 2, args);
                }
            }

            switch (arg1.Expression.Kind())
            {
                case SyntaxKind.FalseLiteralExpression: return AssertThat(arg0, Is("False"), 2, args);
                case SyntaxKind.TrueLiteralExpression: return AssertThat(arg0, Is("True"), 2, args);
                case SyntaxKind.NullLiteralExpression: return AssertThat(arg0, Is("Null"), 2, args);
                case SyntaxKind.StringLiteralExpression: return AssertThat(arg0, Is(call, arg1), 2, args);
                case SyntaxKind.NumericLiteralExpression:
                {
                    if (args.Count > 2)
                    {
                        var arg2 = args[2];

                        if (IsNumeric(arg2))
                        {
                            // seems we have a tolerance parameter
                            return AssertThat(arg0, Is(call, arg1, "Within", arg2), 3, args);
                        }
                    }

                    return AssertThat(arg0, Is(call, arg1), 2, args);
                }
            }

            return AssertThat(arg1, Is(call, arg0), 2, args);
        }

        private static InvocationExpressionSyntax FixAreEqualIgnoringCase(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("EqualTo", args[0], "IgnoreCase"), 2, args);

        private static InvocationExpressionSyntax FixAreEquivalent(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("EquivalentTo", args[0]), 2, args);

        private static InvocationExpressionSyntax FixAreNotEqual(SeparatedSyntaxList<ArgumentSyntax> args) => FixAreNotEqualOrSame(args, "EqualTo");

        private static InvocationExpressionSyntax FixAreNotEqualOrSame(SeparatedSyntaxList<ArgumentSyntax> args, string call)
        {
            var arg0 = args[0];
            var arg1 = args[1];

            switch (arg0.Expression.Kind())
            {
                case SyntaxKind.FalseLiteralExpression: return AssertThat(arg1, Is("True"), 2, args);
                case SyntaxKind.TrueLiteralExpression: return AssertThat(arg1, Is("False"), 2, args);
                case SyntaxKind.NullLiteralExpression: return AssertThat(arg1, Is("Not", "Null"), 2, args);
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression: return AssertThat(arg1, Is("Not", call, arg0), 2, args);
            }

            switch (arg1.Expression.Kind())
            {
                case SyntaxKind.FalseLiteralExpression: return AssertThat(arg0, Is("True"), 2, args);
                case SyntaxKind.TrueLiteralExpression: return AssertThat(arg0, Is("False"), 2, args);
                case SyntaxKind.NullLiteralExpression: return AssertThat(arg0, Is("Not", "Null"), 2, args);
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression: return AssertThat(arg0, Is("Not", call, arg1), 2, args);
            }

            return AssertThat(arg1, Is("Not", call, arg0), 2, args);
        }

        private static InvocationExpressionSyntax FixAreNotEquivalent(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "EquivalentTo", args[0]), 2, args);

        private static InvocationExpressionSyntax FixAreNotSame(SeparatedSyntaxList<ArgumentSyntax> args) => FixAreNotEqualOrSame(args, "SameAs");

        private static InvocationExpressionSyntax FixAreSame(SeparatedSyntaxList<ArgumentSyntax> args) => FixAreEqualOrSame(args, "SameAs");

        private static InvocationExpressionSyntax FixCollectionAssertContains(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Does("Contain", args[1]), 2, args);

        private static InvocationExpressionSyntax FixCollectionAssertDoesNotContain(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Does("Not", "Contain", args[1]), 2, args);

        private static InvocationExpressionSyntax FixContains(string typeName, SeparatedSyntaxList<ArgumentSyntax> args) => typeName == "CollectionAssert" ? FixCollectionAssertContains(args) : FixStringAssertContains(args);

        private static InvocationExpressionSyntax FixDoesNotContain(string typeName, SeparatedSyntaxList<ArgumentSyntax> args) => typeName == "CollectionAssert" ? FixCollectionAssertDoesNotContain(args) : FixStringAssertDoesNotContain(args);

        private static InvocationExpressionSyntax FixDoesNotEndWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Not", "EndWith", args[0]), 2, args);

        private static InvocationExpressionSyntax FixDoesNotStartWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Not", "StartWith", args[0]), 2, args);

        private static InvocationExpressionSyntax FixEndsWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("EndWith", args[0]), 2, args);

        private static InvocationExpressionSyntax FixGreater(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("GreaterThan", args[1]), 2, args);

        private static InvocationExpressionSyntax FixGreaterOrEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("GreaterThanOrEqualTo", args[1]), 2, args);

        private static InvocationExpressionSyntax FixIsEmpty(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Empty"), 1, args);

        private static InvocationExpressionSyntax FixIsFalse(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var argument = args[0];

            if (argument.Expression is BinaryExpressionSyntax b)
            {
                var leftIsNull = b.Left.IsKind(SyntaxKind.NullLiteralExpression);
                var rightIsNull = b.Right.IsKind(SyntaxKind.NullLiteralExpression);

                switch (b.Kind())
                {
                    case SyntaxKind.EqualsExpression:
                        return leftIsNull || rightIsNull
                                   ? AssertThat(leftIsNull ? b.Right : b.Left, Is("Not", "Null"), 1, args)
                                   : AssertThat(b.Left, Is("Not", "EqualTo", b.Right), 1, args);

                    case SyntaxKind.NotEqualsExpression:
                        return leftIsNull || rightIsNull
                                   ? AssertThat(leftIsNull ? b.Right : b.Left, Is("Null"), 1, args)
                                   : AssertThat(b.Left, Is("EqualTo", b.Right), 1, args);

                    case SyntaxKind.LessThanExpression: return AssertThat(b.Left, Is("GreaterThanOrEqualTo", b.Right), 1, args);
                    case SyntaxKind.LessThanOrEqualExpression: return AssertThat(b.Left, Is("GreaterThan", b.Right), 1, args);
                    case SyntaxKind.GreaterThanExpression: return AssertThat(b.Left, Is("LessThanOrEqualTo", b.Right), 1, args);
                    case SyntaxKind.GreaterThanOrEqualExpression: return AssertThat(b.Left, Is("LessThan", b.Right), 1, args);
                }
            }

            return AssertThat(argument, Is("False"), 1, args);
        }

        private static InvocationExpressionSyntax FixIsInstanceOf(SeparatedSyntaxList<ArgumentSyntax> args, SimpleNameSyntax name)
        {
            var arg0 = args[0];

            if (name is GenericNameSyntax gns)
            {
                return AssertThat(arg0, Is("InstanceOf", gns.TypeArgumentList.Arguments.ToArray()), 1, args);
            }

            if (arg0.Expression is TypeOfExpressionSyntax t)
            {
                return AssertThat(args[1], Is("InstanceOf", new[] { t.Type }), 2, args);
            }

            // TODO: this code is not tested as the case does not exist
            return AssertThat(arg0, Is("InstanceOf"), 1, args);
        }

        private static InvocationExpressionSyntax FixIsOrdered(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Ordered"), 1, args);

        private static InvocationExpressionSyntax FixIsNaN(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("NaN"), 1, args);

        private static InvocationExpressionSyntax FixIsNotEmpty(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Empty"), 1, args);

        private static InvocationExpressionSyntax FixIsNotNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Null"), 1, args);

        private static InvocationExpressionSyntax FixIsNotSubsetOf(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "SubsetOf", args[0]), 2, args);

        private static InvocationExpressionSyntax FixIsNotSupersetOf(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "SupersetOf", args[0]), 2, args);

        private static InvocationExpressionSyntax FixIsNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Null"), 1, args);

        private static InvocationExpressionSyntax FixIsNullOrEmpty(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Null", "Or", "Empty"), 1, args);

        private static InvocationExpressionSyntax FixIsSubsetOf(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("SubsetOf", args[0]), 2, args);

        private static InvocationExpressionSyntax FixIsSupersetOf(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("SupersetOf", args[0]), 2, args);

        private static InvocationExpressionSyntax FixIsTrue(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var argument = args[0];

            if (argument.Expression is BinaryExpressionSyntax b)
            {
                var leftIsNull = b.Left.IsKind(SyntaxKind.NullLiteralExpression);
                var rightIsNull = b.Right.IsKind(SyntaxKind.NullLiteralExpression);

                switch (b.Kind())
                {
                    case SyntaxKind.EqualsExpression:
                        return leftIsNull || rightIsNull
                                   ? AssertThat(leftIsNull ? b.Right : b.Left, Is("Null"), 1, args)
                                   : AssertThat(b.Left, Is("EqualTo", b.Right), 1, args);

                    case SyntaxKind.NotEqualsExpression:
                        return leftIsNull || rightIsNull
                                   ? AssertThat(leftIsNull ? b.Right : b.Left, Is("Not", "Null"), 1, args)
                                   : AssertThat(b.Left, Is("Not", "EqualTo", b.Right), 1, args);

                    case SyntaxKind.LessThanExpression: return AssertThat(b.Left, Is("LessThan", b.Right), 1, args);
                    case SyntaxKind.LessThanOrEqualExpression: return AssertThat(b.Left, Is("LessThanOrEqualTo", b.Right), 1, args);
                    case SyntaxKind.GreaterThanExpression: return AssertThat(b.Left, Is("GreaterThan", b.Right), 1, args);
                    case SyntaxKind.GreaterThanOrEqualExpression: return AssertThat(b.Left, Is("GreaterThanOrEqualTo", b.Right), 1, args);
                }
            }

            return AssertThat(argument, Is("True"), 1, args);
        }

        private static InvocationExpressionSyntax FixLess(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("LessThan", args[1]), 2, args);

        private static InvocationExpressionSyntax FixLessOrEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("LessThanOrEqualTo", args[1]), 2, args);

        private static InvocationExpressionSyntax FixPositive(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Positive"), 1, args);

        private static InvocationExpressionSyntax FixNegative(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Negative"), 1, args);

        private static InvocationExpressionSyntax FixNotNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Null"), 1, args);

        private static InvocationExpressionSyntax FixNotZero(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Zero"), 1, args);

        private static InvocationExpressionSyntax FixStartsWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("StartWith", args[0]), 2, args);

        private static InvocationExpressionSyntax FixStringAssertContains(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Contain", args[0]), 2, args);

        private static InvocationExpressionSyntax FixStringAssertDoesNotContain(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Not", "Contain", args[0]), 2, args);

        private static InvocationExpressionSyntax FixZero(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Zero"), 1, args);

        private static InvocationExpressionSyntax AssertThat(ExpressionSyntax expression, ArgumentSyntax constraint, int skip, SeparatedSyntaxList<ArgumentSyntax> arguments)
            => AssertThat(SyntaxFactory.Argument(expression), constraint, skip, arguments);

        private static InvocationExpressionSyntax AssertThat(ArgumentSyntax argument, ArgumentSyntax constraint, int skip, SeparatedSyntaxList<ArgumentSyntax> arguments)
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

        private static InvocationExpressionSyntax AssertThat(params ArgumentSyntax[] arguments) => CreateInvocationSyntax("Assert", "That", arguments);

        private static ArgumentSyntax Is(string name) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Is", name));

        private static ArgumentSyntax Is(string name, ArgumentSyntax argument) => SyntaxFactory.Argument(InvocationIs(name, argument));

        private static ArgumentSyntax Is(string name, ExpressionSyntax expression) => Is(name, SyntaxFactory.Argument(expression));

        private static ArgumentSyntax Is(string name, TypeSyntax[] items) => SyntaxFactory.Argument(CreateInvocationSyntax("Is", name, items));

        private static ArgumentSyntax Is(string name, string name1, ArgumentSyntax argument)
        {
            return Argument(CreateSimpleMemberAccessExpressionSyntax("Is", name, name1), argument);
        }

        private static ArgumentSyntax Is(string name, ArgumentSyntax argument, string name1, ArgumentSyntax argument1)
        {
            var isCall = InvocationIs(name, argument);
            var appendixCall = CreateSimpleMemberAccessExpressionSyntax(isCall, name1);

            return Argument(appendixCall, argument1);
        }

        private static ArgumentSyntax Is(string name, string name1, ExpressionSyntax expression) => Is(name, name1, SyntaxFactory.Argument(expression));

        private static ArgumentSyntax Is(string name, ArgumentSyntax argument, string name1)
        {
            var expression = InvocationIs(name, argument);

            return SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax(expression, name1));
        }

        private static ArgumentSyntax Is(params string[] names) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Is", names));

        private static InvocationExpressionSyntax InvocationIs(string name, ArgumentSyntax argument) => CreateInvocationSyntax("Is", name, argument);

        private static ArgumentSyntax Does(string name, ArgumentSyntax argument) => SyntaxFactory.Argument(CreateInvocationSyntax("Does", name, argument));

        private static ArgumentSyntax Does(string name, string name1, ArgumentSyntax argument)
        {
            return Argument(CreateSimpleMemberAccessExpressionSyntax("Does", name, name1), argument);
        }

        private static ArgumentSyntax Does(params string[] names) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Does", names));

        private static ArgumentSyntax Argument(MemberAccessExpressionSyntax expression, ArgumentSyntax argument) => SyntaxFactory.Argument(CreateInvocationSyntax(expression, argument));

        private static bool IsNumeric(ArgumentSyntax argument) => argument.Expression.IsKind(SyntaxKind.NumericLiteralExpression)
                                                               || (argument.Expression is MemberAccessExpressionSyntax mae && mae.Expression.IsKind(SyntaxKind.PredefinedType));
    }
}
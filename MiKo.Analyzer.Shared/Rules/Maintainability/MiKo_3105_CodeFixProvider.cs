using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3105_CodeFixProvider)), Shared]
    public sealed class MiKo_3105_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3105_TestMethodsUseAssertThatAnalyzer.Id;

        protected override string Title => Resources.MiKo_3105_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var original = (InvocationExpressionSyntax)syntax;

            if (original.Parent is EqualsValueClauseSyntax)
            {
                // TODO Fix me later because currently we do not know how to fix that situation
                return original;
            }

            if (original.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax type)
            {
                var typeName = type.GetName();
                switch (typeName)
                {
                    case "Assert":
                    case "CollectionAssert":
                    case "DirectoryAssert":
                    case "FileAssert":
                    case "StringAssert":
                    {
                        var args = original.ArgumentList.Arguments;

                        var fixedSyntax = UpdatedSyntax(context, maes, args, typeName);
                        if (fixedSyntax != null)
                        {
                            // ensure that we keep leading trivia, such as comments
                            return fixedSyntax.WithLeadingTriviaFrom(original);
                        }

                        break;
                    }
                }
            }

            return original;
        }

        private static ExpressionSyntax UpdatedSyntax(CodeFixContext context, MemberAccessExpressionSyntax syntax, SeparatedSyntaxList<ArgumentSyntax> args, string typeName)
        {
            var methodName = syntax.GetName();

            switch (methodName)
            {
                case "AllItemsAreInstancesOfType": return FixAllItemsAreInstancesOfType(args, syntax.Name);
                case "AllItemsAreNotNull": return FixAllItemsAreNotNull(args);
                case "AllItemsAreUnique": return FixAllItemsAreUnique(args);
                case "AreEqual": return FixAreEqual(context, args);
                case "AreEqualIgnoringCase": return FixAreEqualIgnoringCase(args);
                case "AreEquivalent": return FixAreEquivalent(args);
                case "AreNotEqual": return FixAreNotEqual(context, args);
                case "AreNotEqualIgnoringCase": return FixAreNotEqualIgnoringCase(args);
                case "AreNotEquivalent": return FixAreNotEquivalent(args);
                case "AreNotSame": return FixAreNotSame(context, args);
                case "AreSame": return FixAreSame(context, args);
                case "Contains": return FixContains(typeName, args);
                case "DoesNotContain": return FixDoesNotContain(args, typeName);
                case "DoesNotMatch": return FixStringAssertDoesNotMatch(args);
                case "DoesNotEndWith": return FixDoesNotEndWith(args);
                case "DoesNotExist": return FixDoesNotExist(args);
                case "DoesNotStartWith": return FixDoesNotStartWith(args);
                case "DoesNotThrow": return FixDoesNotThrow(args);
                case "DoesNotThrowAsync": return FixDoesNotThrow(args);
                case "EndsWith": return FixEndsWith(args);
                case "Exists": return FixExists(args);
                case "False": return FixIsFalse(args);
                case "Greater": return FixGreater(args);
                case "GreaterOrEqual": return FixGreaterOrEqual(args);
                case "IsAssignableFrom": return FixIsAssignableFrom(args, syntax.Name);
                case "IsEmpty": return FixIsEmpty(args);
                case "IsFalse": return FixIsFalse(args);
                case "IsInstanceOf": return FixIsInstanceOf(args, syntax.Name);
                case "IsMatch": return FixStringAssertIsMatch(args);
                case "IsNaN": return FixIsNaN(args);
                case "IsNotAssignableFrom": return FixIsNotAssignableFrom(args, syntax.Name);
                case "IsNotEmpty": return FixIsNotEmpty(args);
                case "IsNotInstanceOf": return FixIsNotInstanceOf(args, syntax.Name);
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
                case "Negative": return FixNegative(args);
                case "NotNull": return FixNotNull(args);
                case "NotZero": return FixNotZero(args);
                case "Null": return FixIsNull(args);
                case "Positive": return FixPositive(args);
                case "StartsWith": return FixStartsWith(args);
                case "True": return FixIsTrue(args);
                case "Throws": return FixThrows(args, syntax.Name);
                case "Zero": return FixZero(args);
                default: return null;
            }
        }

        private static InvocationExpressionSyntax FixAllItemsAreInstancesOfType(SeparatedSyntaxList<ArgumentSyntax> args, SimpleNameSyntax name) => FixGenericIs("All", "InstanceOf", args, name);

        private static InvocationExpressionSyntax FixAllItemsAreNotNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("All", "Not", "Null"), args, 1);

        private static InvocationExpressionSyntax FixAllItemsAreUnique(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Unique"), args, 1);

        private static InvocationExpressionSyntax FixAreEqual(CodeFixContext context, SeparatedSyntaxList<ArgumentSyntax> args) => FixAreEqualOrSame(context, args, "EqualTo");

        private static InvocationExpressionSyntax FixAreEqualIgnoringCase(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("EqualTo", args[0], "IgnoreCase"), args);

        private static InvocationExpressionSyntax FixAreEqualOrSame(CodeFixContext context, SeparatedSyntaxList<ArgumentSyntax> args, string call)
        {
            var arg0 = args[0];
            var arg1 = args[1];

            switch (arg0.Expression.Kind())
            {
                // constants & enums
                case SyntaxKind.SimpleMemberAccessExpression when IsEnum(context, arg0): return AssertThat(arg1, Is(call, arg0), args);
                case SyntaxKind.IdentifierName when IsConst(context, arg0): return AssertThat(arg1, Is(call, arg0), args);

                // literals
                case SyntaxKind.FalseLiteralExpression: return AssertThat(arg1, Is("False"), args);
                case SyntaxKind.TrueLiteralExpression: return AssertThat(arg1, Is("True"), args);
                case SyntaxKind.NullLiteralExpression: return AssertThat(arg1, Is("Null"), args);
                case SyntaxKind.StringLiteralExpression: return AssertThat(arg1, Is(call, arg0), args);
                case SyntaxKind.NumericLiteralExpression:
                {
                    if (args.Count > 2)
                    {
                        var arg2 = args[2];

                        if (IsNumeric(arg2))
                        {
                            // seems we have a tolerance parameter
                            return AssertThat(arg1, Is(call, arg0, "Within", arg2), args, 3);
                        }
                    }

                    return AssertThat(arg1, Is(call, arg0), args);
                }
            }

            switch (arg1.Expression.Kind())
            {
                // constants & enums
                case SyntaxKind.SimpleMemberAccessExpression when IsEnum(context, arg1): return AssertThat(arg0, Is(call, arg1), args);
                case SyntaxKind.IdentifierName when IsConst(context, arg1): return AssertThat(arg0, Is(call, arg1), args);

                // literals
                case SyntaxKind.FalseLiteralExpression: return AssertThat(arg0, Is("False"), args);
                case SyntaxKind.TrueLiteralExpression: return AssertThat(arg0, Is("True"), args);
                case SyntaxKind.NullLiteralExpression: return AssertThat(arg0, Is("Null"), args);
                case SyntaxKind.StringLiteralExpression: return AssertThat(arg0, Is(call, arg1), args);
                case SyntaxKind.NumericLiteralExpression:
                {
                    if (args.Count > 2)
                    {
                        var arg2 = args[2];

                        if (IsNumeric(arg2))
                        {
                            // seems we have a tolerance parameter
                            return AssertThat(arg0, Is(call, arg1, "Within", arg2), args, 3);
                        }
                    }

                    return AssertThat(arg0, Is(call, arg1), args);
                }
            }

            return AssertThat(arg1, Is(call, arg0), args);
        }

        private static InvocationExpressionSyntax FixAreEquivalent(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("EquivalentTo", args[0]), args);

        private static InvocationExpressionSyntax FixAreNotEqual(CodeFixContext context, SeparatedSyntaxList<ArgumentSyntax> args) => FixAreNotEqualOrSame(context, args, "EqualTo");

        private static InvocationExpressionSyntax FixAreNotEqualIgnoringCase(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "EqualTo", args[0], "IgnoreCase"), args);

        private static InvocationExpressionSyntax FixAreNotEqualOrSame(CodeFixContext context, SeparatedSyntaxList<ArgumentSyntax> args, string call)
        {
            var arg0 = args[0];
            var arg1 = args[1];

            switch (arg0.Expression.Kind())
            {
                // constants & enums
                case SyntaxKind.SimpleMemberAccessExpression when IsEnum(context, arg0): return AssertThat(arg1, Is("Not", call, arg0), args);
                case SyntaxKind.IdentifierName when IsConst(context, arg0): return AssertThat(arg1, Is("Not", call, arg0), args);

                // literals
                case SyntaxKind.FalseLiteralExpression: return AssertThat(arg1, Is("True"), args);
                case SyntaxKind.TrueLiteralExpression: return AssertThat(arg1, Is("False"), args);
                case SyntaxKind.NullLiteralExpression: return AssertThat(arg1, Is("Not", "Null"), args);
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression: return AssertThat(arg1, Is("Not", call, arg0), args);
            }

            switch (arg1.Expression.Kind())
            {
                // constants & enums
                case SyntaxKind.SimpleMemberAccessExpression when IsEnum(context, arg1): return AssertThat(arg0, Is("Not", call, arg1), args);
                case SyntaxKind.IdentifierName when IsConst(context, arg1): return AssertThat(arg0, Is("Not", call, arg1), args);

                // literals
                case SyntaxKind.FalseLiteralExpression: return AssertThat(arg0, Is("True"), args);
                case SyntaxKind.TrueLiteralExpression: return AssertThat(arg0, Is("False"), args);
                case SyntaxKind.NullLiteralExpression: return AssertThat(arg0, Is("Not", "Null"), args);
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression: return AssertThat(arg0, Is("Not", call, arg1), args);
            }

            return AssertThat(arg1, Is("Not", call, arg0), args);
        }

        private static InvocationExpressionSyntax FixAreNotEquivalent(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "EquivalentTo", args[0]), args);

        private static InvocationExpressionSyntax FixAreNotSame(CodeFixContext context,  SeparatedSyntaxList<ArgumentSyntax> args) => FixAreNotEqualOrSame(context, args, "SameAs");

        private static InvocationExpressionSyntax FixAreSame(CodeFixContext context,  SeparatedSyntaxList<ArgumentSyntax> args) => FixAreEqualOrSame(context, args, "SameAs");

        private static InvocationExpressionSyntax FixCollectionAssertContains(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Does("Contain", args[1]), args);

        private static InvocationExpressionSyntax FixCollectionAssertDoesNotContain(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Does("Not", "Contain", args[1]), args);

        private static InvocationExpressionSyntax FixContains(string typeName, SeparatedSyntaxList<ArgumentSyntax> args) => typeName == "CollectionAssert" ? FixCollectionAssertContains(args) : FixStringAssertContains(args);

        private static InvocationExpressionSyntax FixDoesNotContain(SeparatedSyntaxList<ArgumentSyntax> args, string typeName) => typeName == "CollectionAssert" ? FixCollectionAssertDoesNotContain(args) : FixStringAssertDoesNotContain(args);

        private static InvocationExpressionSyntax FixDoesNotEndWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Not", "EndWith", args[0]), args);

        private static InvocationExpressionSyntax FixDoesNotExist(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Does("Not", "Exist"), args, 1);

        private static InvocationExpressionSyntax FixDoesNotStartWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Not", "StartWith", args[0]), args);

        private static InvocationExpressionSyntax FixDoesNotThrow(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Throws("Nothing"), args, 1);

        private static InvocationExpressionSyntax FixEndsWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("EndWith", args[0]), args);

        private static InvocationExpressionSyntax FixExists(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Does("Exist"), args, 1);

        private static InvocationExpressionSyntax FixGenericIs(string methodName, SeparatedSyntaxList<ArgumentSyntax> args, SimpleNameSyntax name)
        {
            var arg0 = args[0];

            switch (arg0.Expression)
            {
                case TypeOfExpressionSyntax t:
                    return AssertThat(args[1], Is(methodName, new[] { t.Type }), args);

                case InvocationExpressionSyntax i when i.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax nameSyntax:
                    return AssertThat(Argument(nameSyntax.Identifier.Text), Is(methodName, GetTypeSyntaxes(i, name)), args);
            }

            if (name is GenericNameSyntax gns)
            {
                return AssertThat(arg0, Is(methodName, gns.TypeArgumentList.Arguments.ToArray()), args, 1);
            }

            // TODO: this code is not tested as the case does not exist
            return AssertThat(arg0, Is(methodName), args, 1);
        }

        private static InvocationExpressionSyntax FixGenericIs(string propertyName, string methodName, SeparatedSyntaxList<ArgumentSyntax> args, SimpleNameSyntax name)
        {
            var arg0 = args[0];

            switch (arg0.Expression)
            {
                case TypeOfExpressionSyntax t:
                    return AssertThat(args[1], Is(propertyName, methodName, new[] { t.Type }), args);

                case InvocationExpressionSyntax i when i.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax nameSyntax:
                    return AssertThat(Argument(nameSyntax.Identifier.Text), Is(propertyName, methodName, GetTypeSyntaxes(i, name)), args);
            }

            if (name is GenericNameSyntax gns)
            {
                return AssertThat(arg0, Is(propertyName, methodName, gns.TypeArgumentList.Arguments.ToArray()), args, 1);
            }

            if (args[1].Expression is TypeOfExpressionSyntax t1)
            {
                return AssertThat(arg0, Is(propertyName, methodName, new[] { t1.Type }), args);
            }

            // TODO: this code is not tested as the case does not exist
            return AssertThat(arg0, Is(propertyName, methodName), args, 1);
        }

        private static InvocationExpressionSyntax FixGreater(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("GreaterThan", args[1]), args);

        private static InvocationExpressionSyntax FixGreaterOrEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("GreaterThanOrEqualTo", args[1]), args);

        private static InvocationExpressionSyntax FixIsAssignableFrom(SeparatedSyntaxList<ArgumentSyntax> args, SimpleNameSyntax name) => FixGenericIs("AssignableFrom", args, name);

        private static InvocationExpressionSyntax FixIsEmpty(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Empty"), args, 1);

        private static InvocationExpressionSyntax FixIsFalse(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var argument = args[0];

            switch (argument.Expression)
            {
                case BinaryExpressionSyntax b:
                {
                    var leftIsNull = b.Left.IsKind(SyntaxKind.NullLiteralExpression);
                    var rightIsNull = b.Right.IsKind(SyntaxKind.NullLiteralExpression);

                    switch (b.Kind())
                    {
                        case SyntaxKind.EqualsExpression:
                            return leftIsNull || rightIsNull
                                       ? AssertThat(leftIsNull ? b.Right : b.Left, Is("Not", "Null"), args)
                                       : AssertThat(b.Left, Is("Not", "EqualTo", b.Right), args);

                        case SyntaxKind.NotEqualsExpression:
                            return leftIsNull || rightIsNull
                                       ? AssertThat(leftIsNull ? b.Right : b.Left, Is("Null"), args)
                                       : AssertThat(b.Left, Is("EqualTo", b.Right), args);

                        case SyntaxKind.LessThanExpression: return AssertThat(b.Left, Is("GreaterThanOrEqualTo", b.Right), args);
                        case SyntaxKind.LessThanOrEqualExpression: return AssertThat(b.Left, Is("GreaterThan", b.Right), args);
                        case SyntaxKind.GreaterThanExpression: return AssertThat(b.Left, Is("LessThanOrEqualTo", b.Right), args);
                        case SyntaxKind.GreaterThanOrEqualExpression: return AssertThat(b.Left, Is("LessThan", b.Right), args);
                    }

                    break;
                }

                case InvocationExpressionSyntax i when i.Expression is MemberAccessExpressionSyntax maes:
                {
                    switch (maes.GetName())
                    {
                        case "IsAssignableFrom": return FixIsNotAssignableFrom(args, maes.Name); // reverse logic as we are in the 'IsFalse' case
                        case "IsNotAssignableFrom": return FixIsAssignableFrom(args, maes.Name); // reverse logic as we are in the 'IsFalse' case
                    }

                    break;
                }
            }

            return AssertThat(argument, Is("False"), args, 1);
        }

        private static InvocationExpressionSyntax FixIsInstanceOf(SeparatedSyntaxList<ArgumentSyntax> args, SimpleNameSyntax name) => FixGenericIs("InstanceOf", args, name);

        private static InvocationExpressionSyntax FixIsNaN(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("NaN"), args, 1);

        private static InvocationExpressionSyntax FixIsNotAssignableFrom(SeparatedSyntaxList<ArgumentSyntax> args, SimpleNameSyntax name) => FixGenericIs("Not", "AssignableFrom", args, name);

        private static InvocationExpressionSyntax FixIsNotEmpty(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Empty"), args, 1);

        private static InvocationExpressionSyntax FixIsNotInstanceOf(SeparatedSyntaxList<ArgumentSyntax> args, SimpleNameSyntax name) => FixGenericIs("Not", "InstanceOf", args, name);

        private static InvocationExpressionSyntax FixIsNotNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Null"), args, 1);

        private static InvocationExpressionSyntax FixIsNotSubsetOf(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "SubsetOf", args[0]), args);

        private static InvocationExpressionSyntax FixIsNotSupersetOf(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "SupersetOf", args[0]), args);

        private static InvocationExpressionSyntax FixIsNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Null"), args, 1);

        private static InvocationExpressionSyntax FixIsNullOrEmpty(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Null", "Or", "Empty"), args, 1);

        private static InvocationExpressionSyntax FixIsOrdered(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Ordered"), args, 1);

        private static InvocationExpressionSyntax FixIsSubsetOf(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("SubsetOf", args[0]), args);

        private static InvocationExpressionSyntax FixIsSupersetOf(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("SupersetOf", args[0]), args);

        private static InvocationExpressionSyntax FixIsTrue(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var argument = args[0];

            switch (argument.Expression)
            {
                case BinaryExpressionSyntax b:
                {
                    var leftIsNull = b.Left.IsKind(SyntaxKind.NullLiteralExpression);
                    var rightIsNull = b.Right.IsKind(SyntaxKind.NullLiteralExpression);

                    switch (b.Kind())
                    {
                        case SyntaxKind.EqualsExpression:
                            return leftIsNull || rightIsNull
                                       ? AssertThat(leftIsNull ? b.Right : b.Left, Is("Null"), args)
                                       : AssertThat(b.Left, Is("EqualTo", b.Right), args);

                        case SyntaxKind.NotEqualsExpression:
                            return leftIsNull || rightIsNull
                                       ? AssertThat(leftIsNull ? b.Right : b.Left, Is("Not", "Null"), args)
                                       : AssertThat(b.Left, Is("Not", "EqualTo", b.Right), args);

                        case SyntaxKind.LessThanExpression: return AssertThat(b.Left, Is("LessThan", b.Right), args);
                        case SyntaxKind.LessThanOrEqualExpression: return AssertThat(b.Left, Is("LessThanOrEqualTo", b.Right), args);
                        case SyntaxKind.GreaterThanExpression: return AssertThat(b.Left, Is("GreaterThan", b.Right), args);
                        case SyntaxKind.GreaterThanOrEqualExpression: return AssertThat(b.Left, Is("GreaterThanOrEqualTo", b.Right), args);
                    }

                    break;
                }

                case InvocationExpressionSyntax i when i.Expression is MemberAccessExpressionSyntax maes:
                {
                    switch (maes.GetName())
                    {
                        case "IsAssignableFrom": return FixIsAssignableFrom(args, maes.Name);
                        case "IsNotAssignableFrom": return FixIsNotAssignableFrom(args, maes.Name);
                    }

                    break;
                }
            }

            return AssertThat(argument, Is("True"), args, 1);
        }

        private static InvocationExpressionSyntax FixLess(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("LessThan", args[1]), args);

        private static InvocationExpressionSyntax FixLessOrEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("LessThanOrEqualTo", args[1]), args);

        private static InvocationExpressionSyntax FixNegative(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Negative"), args, 1);

        private static InvocationExpressionSyntax FixNotNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Null"), args, 1);

        private static InvocationExpressionSyntax FixNotZero(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Zero"), args, 1);

        private static InvocationExpressionSyntax FixPositive(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Positive"), args, 1);

        private static InvocationExpressionSyntax FixStartsWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("StartWith", args[0]), args);

        private static InvocationExpressionSyntax FixStringAssertContains(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Contain", args[0]), args);

        private static InvocationExpressionSyntax FixStringAssertDoesNotContain(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Not", "Contain", args[0]), args);

        private static InvocationExpressionSyntax FixStringAssertDoesNotMatch(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Not", "Match", args[0]), args);

        private static InvocationExpressionSyntax FixStringAssertIsMatch(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Match", args[0]), args);

        private static InvocationExpressionSyntax FixThrows(SeparatedSyntaxList<ArgumentSyntax> args, SimpleNameSyntax name)
        {
            if (name is GenericNameSyntax generic)
            {
                var genericArguments = generic.TypeArgumentList.Arguments;
                if (genericArguments.Count == 1)
                {
                    return AssertThat(args[0], GetExceptionArgument(genericArguments[0]), args, 1);
                }
            }

            if (args[0].Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                return AssertThat(args[1], Throws("TypeOf", typeOfExpression.Type), args);
            }

            return AssertThat(args[1], Throws("Nothing"), args);

            ArgumentSyntax GetExceptionArgument(TypeSyntax exceptionType)
            {
                switch (exceptionType.GetName())
                {
                    case nameof(ArgumentNullException): return Throws(nameof(ArgumentNullException));
                    case nameof(ArgumentException): return Throws(nameof(ArgumentException));
                    case nameof(InvalidOperationException): return Throws(nameof(InvalidOperationException));
                    case nameof(TargetInvocationException): return Throws(nameof(TargetInvocationException));
                    default: return Throws("TypeOf", exceptionType);
                }
            }
        }

        private static InvocationExpressionSyntax FixZero(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Zero"), args, 1);
    }
}
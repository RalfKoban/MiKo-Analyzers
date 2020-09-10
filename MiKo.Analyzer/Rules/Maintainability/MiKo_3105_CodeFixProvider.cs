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

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var original = (InvocationExpressionSyntax)syntax;

            if (original.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax i)
            {
                // for the moment only consider Assert and not StringAssert etc.
                switch (i.GetName())
                {
                    case "Assert":
                    case "CollectionAssert":
                    case "StringAssert":
                    {
                        var args = original.ArgumentList.Arguments;

                        switch (maes.GetName())
                        {
                            case "AreEqual": return FixAreEqual(args);
                            case "AreEqualIgnoringCase": return FixAreEqualIgnoringCase(args);
                            case "AreEquivalent": return FixAreEquivalent(args);
                            case "AreNotEqual": return FixAreNotEqual(args);
                            case "AreNotEquivalent": return FixAreNotEquivalent(args);
                            case "AreNotSame": return FixAreNotSame(args);
                            case "AreSame": return FixAreSame(args);
                            case "Contains": return FixContains(args);
                            case "DoesNotContain": return FixDoesNotContain(args);
                            case "EndsWith": return FixEndsWith(args);
                            case "Greater": return FixGreater(args);
                            case "GreaterOrEqual": return FixGreaterOrEqual(args);
                            case "IsEmpty": return FixIsEmpty(args);
                            case "IsFalse": return FixIsFalse(args);
                            case "IsNotEmpty": return FixIsNotEmpty(args);
                            case "IsNotNull": return FixIsNotNull(args);
                            case "IsNull": return FixIsNull(args);
                            case "IsNullOrEmpty": return FixIsNullOrEmpty(args);
                            case "IsTrue": return FixIsTrue(args);
                            case "Less": return FixLess(args);
                            case "LessOrEqual": return FixLessOrEqual(args);
                            case "NotNull": return FixNotNull(args);
                            case "StartsWith": return FixStartsWith(args);
                        }

                        break;
                    }
                }
            }

            return original;
        }

        private static InvocationExpressionSyntax FixAreEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("EqualTo", args[0]), 2, args);

        private static InvocationExpressionSyntax FixAreEqualIgnoringCase(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("EqualTo", args[0], "IgnoreCase"), 2, args);

        private static InvocationExpressionSyntax FixAreEquivalent(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("EquivalentTo", args[0]), 2, args);

        private static InvocationExpressionSyntax FixAreNotEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "EqualTo", args[0]), 2, args);

        private static InvocationExpressionSyntax FixAreNotEquivalent(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "EquivalentTo", args[0]), 2, args);

        private static InvocationExpressionSyntax FixAreNotSame(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("Not", "SameAs", args[0]), 2, args);

        private static InvocationExpressionSyntax FixAreSame(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Is("SameAs", args[0]), 2, args);

        private static InvocationExpressionSyntax FixContains(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Contain", args[0]), 2, args);

        private static InvocationExpressionSyntax FixDoesNotContain(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("Not", "Contain", args[0]), 2, args);

        private static InvocationExpressionSyntax FixEndsWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("EndWith", args[0]), 2, args);

        private static InvocationExpressionSyntax FixGreater(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("GreaterThan", args[1]), 2, args);

        private static InvocationExpressionSyntax FixGreaterOrEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("GreaterThanOrEqualTo", args[1]), 2, args);

        private static InvocationExpressionSyntax FixIsEmpty(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Empty"), 1, args);

        private static InvocationExpressionSyntax FixIsFalse(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("False"), 1, args);

        private static InvocationExpressionSyntax FixIsNotEmpty(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Empty"), 1, args);

        private static InvocationExpressionSyntax FixIsNotNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Null"), 1, args);

        private static InvocationExpressionSyntax FixIsNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Null"), 1, args);

        private static InvocationExpressionSyntax FixIsNullOrEmpty(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Null", "Or", "Empty"), 1, args);

        private static InvocationExpressionSyntax FixIsTrue(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("True"), 1, args);

        private static InvocationExpressionSyntax FixLess(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("LessThan", args[1]), 2, args);

        private static InvocationExpressionSyntax FixLessOrEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("LessThanOrEqualTo", args[1]), 2, args);

        private static InvocationExpressionSyntax FixNotNull(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], Is("Not", "Null"), 1, args);

        private static InvocationExpressionSyntax FixStartsWith(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[1], Does("StartWith", args[0]), 2, args);

        private static InvocationExpressionSyntax AssertThat(ArgumentSyntax argument, ArgumentSyntax constraint, int skip, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var args = new List<ArgumentSyntax>();
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

        private static ArgumentSyntax Is(string name, ArgumentSyntax argument) => SyntaxFactory.Argument(CreateInvocationSyntax("Is", name, argument));

        private static ArgumentSyntax Is(string name, string name1, ArgumentSyntax argument)
        {
            var expression = CreateSimpleMemberAccessExpressionSyntax("Is", name, name1);

            return SyntaxFactory.Argument(CreateInvocationSyntax(expression, argument));
        }

        private static ArgumentSyntax Is(string name, ArgumentSyntax argument, string name1)
        {
            var expression = CreateInvocationSyntax("Is", name, argument);

            return SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax(expression, name1));
        }

        private static ArgumentSyntax Is(params string[] names) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Is", names));

        private static ArgumentSyntax Does(string name, ArgumentSyntax argument) => SyntaxFactory.Argument(CreateInvocationSyntax("Does", name, argument));

        private static ArgumentSyntax Does(string name, string name1, ArgumentSyntax argument)
        {
            var expression = CreateSimpleMemberAccessExpressionSyntax("Does", name, name1);

            return SyntaxFactory.Argument(CreateInvocationSyntax(expression, argument));
        }

        private static ArgumentSyntax Does(params string[] names) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Does", names));
    }
}
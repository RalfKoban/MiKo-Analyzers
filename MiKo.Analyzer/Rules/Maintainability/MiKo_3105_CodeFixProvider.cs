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
                            case "AreEqual": return FixAssertAreEqual(args);
                            case "AreEquivalent": return FixCollectionAssertAreEquivalent(args);
                            case "AreNotEqual": return FixAssertAreNotEqual(args);
                            case "AreNotEquivalent": return FixCollectionAssertAreNotEquivalent(args);
                            case "AreNotSame": return FixAssertAreNotSame(args);
                            case "AreSame": return FixAssertAreSame(args);
                            case "Contains": return FixStringAssertContains(args);
                            case "DoesNotContain": return FixStringAssertDoesNotContain(args);
                            case "EndsWith": return FixStringAssertEndsWith(args);
                            case "Greater": return FixAssertGreater(args);
                            case "GreaterOrEqual": return FixAssertGreaterOrEqual(args);
                            case "IsEmpty": return FixAssertIsEmpty(args);
                            case "IsFalse": return FixAssertIsFalse(args);
                            case "IsNotEmpty": return FixAssertIsNotEmpty(args);
                            case "IsNotNull": return FixAssertIsNotNull(args);
                            case "IsNull": return FixAssertIsNull(args);
                            case "IsNullOrEmpty": return FixAssertIsNullOrEmpty(args);
                            case "IsTrue": return FixAssertIsTrue(args);
                            case "Less": return FixAssertLess(args);
                            case "LessOrEqual": return FixAssertLessOrEqual(args);
                            case "NotNull": return FixAssertNotNull(args);
                            case "StartsWith": return FixStringAssertStartsWith(args);
                        }

                        break;
                    }
                }
            }

            return original;
        }

        private static InvocationExpressionSyntax FixAssertAreEqual(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Is("EqualTo", args[0]), 2, args);
        }

        private static InvocationExpressionSyntax FixAssertAreNotEqual(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Is("Not", "EqualTo", args[0]), 2, args);
        }

        private static InvocationExpressionSyntax FixAssertAreSame(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Is("SameAs", args[0]), 2, args);
        }

        private static InvocationExpressionSyntax FixAssertAreNotSame(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Is("Not", "SameAs", args[0]), 2, args);
        }

        private static InvocationExpressionSyntax FixAssertIsTrue(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("True"), 1, args);
        }

        private static InvocationExpressionSyntax FixAssertIsFalse(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("False"), 1, args);
        }

        private static InvocationExpressionSyntax FixAssertIsNull(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("Null"), 1, args);
        }

        private static InvocationExpressionSyntax FixAssertIsNullOrEmpty(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("Null", "Or", "Empty"), 1, args);
        }

        private static InvocationExpressionSyntax FixAssertNotNull(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("Not", "Null"), 1, args);
        }

        private static InvocationExpressionSyntax FixAssertIsNotNull(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("Not", "Null"), 1, args);
        }

        private static InvocationExpressionSyntax FixAssertIsEmpty(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("Empty"), 1, args);
        }

        private static InvocationExpressionSyntax FixAssertIsNotEmpty(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("Not", "Empty"), 1, args);
        }

        private static InvocationExpressionSyntax FixAssertGreater(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("GreaterThan", args[1]), 2, args);
        }

        private static InvocationExpressionSyntax FixAssertGreaterOrEqual(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("GreaterThanOrEqualTo", args[1]), 2, args);
        }

        private static InvocationExpressionSyntax FixAssertLess(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("LessThan", args[1]), 2, args);
        }

        private static InvocationExpressionSyntax FixAssertLessOrEqual(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("LessThanOrEqualTo", args[1]), 2, args);
        }

        private static InvocationExpressionSyntax FixCollectionAssertAreEquivalent(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Is("EquivalentTo", args[0]), 2, args);
        }

        private static InvocationExpressionSyntax FixCollectionAssertAreNotEquivalent(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Is("Not", "EquivalentTo", args[0]), 2, args);
        }

        private static InvocationExpressionSyntax FixStringAssertStartsWith(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Does("StartWith", args[0]), 2, args);
        }

        private static InvocationExpressionSyntax FixStringAssertEndsWith(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Does("EndWith", args[0]), 2, args);
        }

        private static InvocationExpressionSyntax FixStringAssertDoesNotContain(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Does("Not", "Contain", args[0]), 2, args);
        }

        private static InvocationExpressionSyntax FixStringAssertContains(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[1], Does("Contain", args[0]), 2, args);
        }

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

        private static ArgumentSyntax Is(string name, string name1, ArgumentSyntax argument)
        {
            var expression = CreateSimpleMemberAccessExpressionSyntax("Is", name, name1);

            return SyntaxFactory.Argument(CreateInvocationSyntax(expression, argument));
        }

        private static ArgumentSyntax Is(params string[] names) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Is", names));

        private static ArgumentSyntax Is(string name, params ArgumentSyntax[] arguments) => SyntaxFactory.Argument(CreateInvocationSyntax("Is", name, arguments));

        private static ArgumentSyntax Does(string name, string name1, ArgumentSyntax argument)
        {
            var expression = CreateSimpleMemberAccessExpressionSyntax("Does", name, name1);

            return SyntaxFactory.Argument(CreateInvocationSyntax(expression, argument));
        }

        private static ArgumentSyntax Does(params string[] names) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Does", names));

        private static ArgumentSyntax Does(string name, params ArgumentSyntax[] arguments) => SyntaxFactory.Argument(CreateInvocationSyntax("Does", name, arguments));
    }
}
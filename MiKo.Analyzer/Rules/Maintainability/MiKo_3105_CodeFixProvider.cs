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

        protected override string Title => "Use 'Assert.That'";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var original = (InvocationExpressionSyntax)syntax;

            if (original.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax i)
            {
                // for the moment only consider Assert and not StringAssert etc.
                if (i.GetName() == "Assert")
                {
                    switch (maes.GetName())
                    {
                        case "AreEqual": return FixAssertAreEqual(original.ArgumentList.Arguments);
                        case "AreNotEqual": return FixAssertAreNotEqual(original.ArgumentList.Arguments);
                        case "AreSame": return FixAssertAreSame(original.ArgumentList.Arguments);
                        case "AreNotSame": return FixAssertAreNotSame(original.ArgumentList.Arguments);
                        case "IsTrue": return FixAssertIsTrue(original.ArgumentList.Arguments);
                        case "IsFalse": return FixAssertIsFalse(original.ArgumentList.Arguments);
                        case "IsNull": return FixAssertIsNull(original.ArgumentList.Arguments);
                        case "NotNull": return FixAssertNotNull(original.ArgumentList.Arguments);
                        case "IsNotEmpty": return FixAssertIsNotEmpty(original.ArgumentList.Arguments);
                        case "Greater": return FixAssertGreater(original.ArgumentList.Arguments);
                        case "GreaterOrEqual": return FixAssertGreaterOrEqual(original.ArgumentList.Arguments);
                        case "Less": return FixAssertLess(original.ArgumentList.Arguments);
                        case "LessOrEqual": return FixAssertLessOrEqual(original.ArgumentList.Arguments);
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

        private static InvocationExpressionSyntax FixAssertNotNull(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            return AssertThat(args[0], Is("Not", "Null"), 1, args);
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

        private static ArgumentSyntax Is(string name, string nextName) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Is", name, nextName));

        private static ArgumentSyntax Is(string name, params ArgumentSyntax[] arguments) => SyntaxFactory.Argument(CreateInvocationSyntax("Is", name, arguments));

        private static ArgumentSyntax Is(string name, string nextName, params ArgumentSyntax[] arguments)
        {
            var expression = CreateSimpleMemberAccessExpressionSyntax("Is", name, nextName);
            return SyntaxFactory.Argument(CreateInvocationSyntax(expression, arguments));
        }
    }
}
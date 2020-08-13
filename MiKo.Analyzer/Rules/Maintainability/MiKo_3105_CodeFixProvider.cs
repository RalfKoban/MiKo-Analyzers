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

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
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
                        case "IsTrue": return FixAssertIsTrue(original.ArgumentList.Arguments);
                        case "IsFalse": return FixAssertIsFalse(original.ArgumentList.Arguments);
                        case "IsNull": return FixAssertIsNull(original.ArgumentList.Arguments);
                        case "NotNull": return FixAssertNotNull(original.ArgumentList.Arguments);

                            // Assert.AreSame(1, 2);
                            // Assert.AreNotSame(1, 2);
                            // Assert.Greater(2,3);
                            // Assert.GreaterOrEqual(2,3);
                            // Assert.Less(2,3);
                            // Assert.LessOrEqual(2,3);
                            // Assert.IsNotEmpty();
                    }
                }
            }

            return original;
        }

        private static InvocationExpressionSyntax FixAssertAreEqual(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return AssertThat(arguments[1], Is("EqualTo", arguments[0]), 2, arguments);
        }

        private static InvocationExpressionSyntax FixAssertAreNotEqual(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return AssertThat(arguments[1], Is("Not", "EqualTo", arguments[0]), 2, arguments);
        }

        private static InvocationExpressionSyntax FixAssertIsTrue(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return AssertThat(arguments[0], Is("True"), 1, arguments);
        }

        private static InvocationExpressionSyntax FixAssertIsFalse(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return AssertThat(arguments[0], Is("False"), 1, arguments);
        }

        private static InvocationExpressionSyntax FixAssertIsNull(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return AssertThat(arguments[0], Is("Null"), 1, arguments);
        }

        private static InvocationExpressionSyntax FixAssertNotNull(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return AssertThat(arguments[0], Is("Not", "Null"), 1, arguments);
        }

        private static InvocationExpressionSyntax AssertThat(params ArgumentSyntax[] arguments) => CreateInvocationSyntax("Assert", "That", arguments);

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

        private static ArgumentSyntax Is(string name) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Is", name));

        private static ArgumentSyntax Is(string name, string nextName) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Is", name, nextName));

        private static ArgumentSyntax Is(string name, string nextName, params ArgumentSyntax[] arguments)
        {
            var expression = CreateSimpleMemberAccessExpressionSyntax("Is", name, nextName);
            return SyntaxFactory.Argument(CreateInvocationSyntax(expression, arguments));
        }

        private static ArgumentSyntax Is(string name, params ArgumentSyntax[] arguments) => SyntaxFactory.Argument(CreateInvocationSyntax("Is", name, arguments));
    }
}
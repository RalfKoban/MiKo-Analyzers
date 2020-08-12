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
                        case "AreEqual":
                            return FixAssertAreEqual(original.ArgumentList.Arguments);

                        case "IsTrue":
                            return FixAssertIsTrue(original.ArgumentList.Arguments);

                        case "IsFalse":
                            return FixAssertIsFalse(original.ArgumentList.Arguments);
                    }
                }
            }

            return original;
        }

        private static InvocationExpressionSyntax FixAssertAreEqual(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var args = PrepareArguments(arguments[1], Is("EqualTo", arguments[0]));

            if (arguments.Count > 2)
            {
                args.AddRange(arguments.Skip(2));
            }

            return AssertThat(args.ToArray());
        }

        private static InvocationExpressionSyntax FixAssertIsTrue(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var args = PrepareArguments(arguments[0], Is("True"));

            if (arguments.Count > 1)
            {
                args.AddRange(arguments.Skip(1));
            }

            return AssertThat(args.ToArray());
        }

        private static InvocationExpressionSyntax FixAssertIsFalse(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var args = PrepareArguments(arguments[0], Is("False"));

            if (arguments.Count > 1)
            {
                args.AddRange(arguments.Skip(1));
            }

            return AssertThat(args.ToArray());
        }

        private static InvocationExpressionSyntax AssertThat(params ArgumentSyntax[] arguments) => CreateInvocationSyntax("Assert", "That", arguments);

        private static ArgumentSyntax Is(string name, params ArgumentSyntax[] arguments) => SyntaxFactory.Argument(CreateInvocationSyntax("Is", name, arguments));

        private static ArgumentSyntax Is(string name) => SyntaxFactory.Argument(CreateSimpleMemberAccessExpressionSyntax("Is", name));

        private static List<ArgumentSyntax> PrepareArguments(ArgumentSyntax argument, ArgumentSyntax constraint)
        {
            var args = new List<ArgumentSyntax>();
            args.Add(argument);
            args.Add(constraint);
            return args;
        }
    }
}
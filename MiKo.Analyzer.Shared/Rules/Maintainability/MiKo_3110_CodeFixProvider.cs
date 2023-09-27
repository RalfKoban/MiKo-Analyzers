using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3110_CodeFixProvider)), Shared]
    public sealed class MiKo_3110_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3110_TestAssertsDoNotUseCountAnalyzer.Id;

        protected override string Title => Resources.MiKo_3110_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var original = (ExpressionStatementSyntax)syntax;

            if (original.Expression is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax type)
            {
                var typeName = type.GetName();

                if (typeName == "Assert")
                {
                    var args = invocation.ArgumentList.Arguments;

                    var fixedInvocation = UpdatedSyntax(maes, args, issue.Properties[MiKo_3110_TestAssertsDoNotUseCountAnalyzer.Marker]);

                    if (fixedInvocation != null)
                    {
                        // ensure that we keep leading trivia, such as comments
                        return original.ReplaceNode(invocation, fixedInvocation.WithLeadingTriviaFrom(invocation));
                    }
                }
            }

            return original;
        }

        private static ExpressionSyntax UpdatedSyntax(MemberAccessExpressionSyntax syntax, SeparatedSyntaxList<ArgumentSyntax> args, string text)
        {
            var methodName = syntax.GetName();

            switch (methodName)
            {
                case "AreEqual": return FixAreEqual(args);
                case "AreNotEqual": return FixAreNotEqual(args, text);
                case "Greater": return FixGreater(args, text);
                case "GreaterOrEqual": return FixGreaterOrEqual(args, text);
                case "Less": return FixLess(args, text);
                case "LessOrEqual": return FixLessOrEqual(args, text);
                case "That": return FixThat(args);
                default: return null;
            }
        }

        private static InvocationExpressionSyntax FixAreEqual(SeparatedSyntaxList<ArgumentSyntax> args) => FixAreEqualOrSame(args);

        private static InvocationExpressionSyntax FixAreEqualOrSame(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var arg0 = args[0];
            var arg1 = args[1];

            if (arg0.Expression.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                return AssertThat(arg1, Has("Exactly", arg0, "Items"), args);
            }

            if (arg1.Expression.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                return AssertThat(arg0, Has("Exactly", arg1, "Items"), args);
            }

            return AssertThat(arg1, Has("Exactly", arg0, "Items"), args);
        }

        private static InvocationExpressionSyntax FixAreNotEqual(SeparatedSyntaxList<ArgumentSyntax> args, string text) => FixAreNotEqualOrSame(args, text);

        private static InvocationExpressionSyntax FixAreNotEqualOrSame(SeparatedSyntaxList<ArgumentSyntax> args, string text)
        {
            var arg0 = args[0];
            var arg1 = args[1];

            if (arg0.Expression.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                return AssertThat(arg1, Has(text, "Not", "EqualTo", arg0.Expression), args);
            }

            if (arg1.Expression.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                return AssertThat(arg0, Has(text, "Not", "EqualTo", arg1.Expression), args);
            }

            return AssertThat(arg1, Has(text, "Not", "EqualTo", arg0), args);
        }

        private static InvocationExpressionSyntax FixGreater(SeparatedSyntaxList<ArgumentSyntax> args, string text) => AssertThat(args[0], Has(text, "GreaterThan", args[1]), args);

        private static InvocationExpressionSyntax FixGreaterOrEqual(SeparatedSyntaxList<ArgumentSyntax> args, string text) => AssertThat(args[0], Has(text, "GreaterThanOrEqualTo", args[1]), args);

        private static InvocationExpressionSyntax FixLess(SeparatedSyntaxList<ArgumentSyntax> args, string text) => AssertThat(args[0], Has(text, "LessThan", args[1]), args);

        private static InvocationExpressionSyntax FixLessOrEqual(SeparatedSyntaxList<ArgumentSyntax> args, string text) => AssertThat(args[0], Has(text, "LessThanOrEqualTo", args[1]), args);

        private static InvocationExpressionSyntax FixThat(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var args1 = args[1];

            switch (args1.Expression.ToString())
            {
                case "Is.Zero":
                case "Is.EqualTo(0)":
                    return AssertThat(GetFixedArgument(args[0]), Is("Empty"));
            }

            foreach (var descendant in args1.DescendantNodes())
            {
                switch (descendant)
                {
                    case ArgumentSyntax argument: // seems like we have a method call
                    {
                        return AssertThat(args[0], Has("Exactly", argument, "Items"), args);
                    }
                }
            }

            return null;
        }

        private static InvocationExpressionSyntax AssertThat(ArgumentSyntax argument, ArgumentSyntax constraint, SeparatedSyntaxList<ArgumentSyntax> arguments) => UnitTestCodeFixProvider.AssertThat(GetFixedArgument(argument), constraint, arguments);

        private static ArgumentSyntax GetFixedArgument(ArgumentSyntax argument)
        {
            switch (argument.Expression)
            {
                case InvocationExpressionSyntax invocation when invocation.Expression is MemberAccessExpressionSyntax syntax:
                    return Argument(syntax.Expression);

                case MemberAccessExpressionSyntax syntax:
                    return Argument(syntax.Expression);

                default:
                    return argument;
            }
        }
    }
}
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

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var original = (ExpressionStatementSyntax)syntax;

            if (original.Expression is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax type)
            {
                var typeName = type.GetName();

                if (typeName == "Assert")
                {
                    var args = invocation.ArgumentList.Arguments;

                    var fixedInvocation = UpdatedSyntax(maes, args);

                    if (fixedInvocation != null)
                    {
                        // ensure that we keep leading trivia, such as comments
                        return original.ReplaceNode(invocation, fixedInvocation.WithLeadingTriviaFrom(invocation));
                    }
                }
            }

            return original;
        }

        private static ExpressionSyntax UpdatedSyntax(MemberAccessExpressionSyntax syntax, SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var methodName = syntax.GetName();

            switch (methodName)
            {
                case "AreEqual": return FixAreEqual(args);
                case "AreNotEqual": return FixAreNotEqual(args);
                case "Greater": return FixGreater(args);
                case "GreaterOrEqual": return FixGreaterOrEqual(args);
                case "Less": return FixLess(args);
                case "LessOrEqual": return FixLessOrEqual(args);
                case "That": return FixThat(args);
                default: return null;
            }
        }

        private static InvocationExpressionSyntax FixAreEqual(SeparatedSyntaxList<ArgumentSyntax> args) => FixAreEqualOrSame(args);

        private static InvocationExpressionSyntax FixAreEqualOrSame(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var arg0 = args[0];
            var arg1 = args[1];

            if (arg0.Expression.Kind() == SyntaxKind.NumericLiteralExpression)
            {
                return AssertThat(arg1, Has("Exactly", arg0, "Items"), args);
            }

            if (arg1.Expression.Kind() == SyntaxKind.NumericLiteralExpression)
            {
                return AssertThat(arg0, Has("Exactly", arg1, "Items"), args);
            }

            return AssertThat(arg1, Has("Exactly", arg0, "Items"), args);
        }

        private static InvocationExpressionSyntax FixAreNotEqual(SeparatedSyntaxList<ArgumentSyntax> args) => FixAreNotEqualOrSame(args);

        private static InvocationExpressionSyntax FixAreNotEqualOrSame(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var arg0 = args[0];
            var arg1 = args[1];

            if (arg0.Expression.Kind() == SyntaxKind.NumericLiteralExpression)
            {
                return AssertThat(arg1, HasCount("Not", "EqualTo", arg0.Expression), args);
            }

            if (arg1.Expression.Kind() == SyntaxKind.NumericLiteralExpression)
            {
                return AssertThat(arg0, HasCount("Not", "EqualTo", arg1.Expression), args);
            }

            return AssertThat(arg1, HasCount("Not", "EqualTo", arg0), args);
        }

        private static InvocationExpressionSyntax FixGreater(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], HasCount("GreaterThan", args[1]), args);

        private static InvocationExpressionSyntax FixGreaterOrEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], HasCount("GreaterThanOrEqualTo", args[1]), args);

        private static InvocationExpressionSyntax FixLess(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], HasCount("LessThan", args[1]), args);

        private static InvocationExpressionSyntax FixLessOrEqual(SeparatedSyntaxList<ArgumentSyntax> args) => AssertThat(args[0], HasCount("LessThanOrEqualTo", args[1]), args);

        private static InvocationExpressionSyntax FixThat(SeparatedSyntaxList<ArgumentSyntax> args)
        {
            var args1 = args[1];

            foreach (var descendant in args1.DescendantNodes())
            {
                switch (descendant)
                {
                    case ExpressionSyntax expression when expression.IsKind(SyntaxKind.NumericLiteralExpression):
                        return AssertThat(args[0], Has("Exactly", Argument(expression), "Items"), args);

                    case ArgumentSyntax argument: // seems like we have a method call
                        return AssertThat(args[0], Has("Exactly", argument, "Items"), args);
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
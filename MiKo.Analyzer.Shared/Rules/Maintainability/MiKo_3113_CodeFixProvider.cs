using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3113_CodeFixProvider)), Shared]
    public sealed class MiKo_3113_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3113";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().FirstOrDefault();

        protected override async Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = syntax;

            if (syntax is ExpressionStatementSyntax statement)
            {
                updatedSyntax = await ConvertAsync(statement, document, cancellationToken).ConfigureAwait(false);
            }

            return updatedSyntax;
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntaxRoot = GetUpdatedSyntaxRoot(root);

            return Task.FromResult(updatedSyntaxRoot);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root)
        {
            // only remove assertions if there are no more diagnostics
            // return root.WithUsing(Constants.Names.DefaultNUnitNamespace).WithoutUsing(Constants.FluentAssertions.Namespace);
            return root.WithUsing(Constants.Names.DefaultNUnitNamespace);
        }

        private static async Task<ExpressionStatementSyntax> ConvertAsync(ExpressionStatementSyntax statement, Document document, CancellationToken cancellationToken)
        {
            var shouldNode = statement.GetFluentAssertionShouldNode();
            var shouldName = shouldNode.GetName();

            var assertThat = shouldName is Constants.FluentAssertions.ShouldBeEquivalentTo
                             ? ConvertShouldBeEquivalentTo(shouldNode)
                             : await ConvertShouldAsync(shouldNode, document, cancellationToken).ConfigureAwait(false);

            if (assertThat is null)
            {
                return statement;
            }

            // find lambda
            var lambda = shouldNode.FirstAncestor<LambdaExpressionSyntax>();

            if (lambda != null && lambda.AncestorsWithinMethods<ExpressionStatementSyntax>().Any(_ => _ == statement))
            {
                // we have a lambda expression, so replace that one
                return statement.ReplaceNode(lambda, lambda.WithExpressionBody(assertThat));
            }

            return statement.WithExpression(assertThat).WithTriviaFrom(statement);
        }

        private static InvocationExpressionSyntax ConvertShouldBeEquivalentTo(MemberAccessExpressionSyntax shouldNode)
        {
            var expression = shouldNode.Expression.WithoutLeadingTrivia();

            var invocation = shouldNode.FirstAncestor<InvocationExpressionSyntax>();
            var arguments = invocation.ArgumentList.Arguments;

            return AssertThat(expression, Is("EquivalentTo", arguments[0]), arguments, removeNameColon: true);
        }

        private static async Task<InvocationExpressionSyntax> ConvertShouldAsync(MemberAccessExpressionSyntax shouldNode, Document document, CancellationToken cancellationToken)
        {
            var originalExpression = shouldNode.Expression;

            var expression = originalExpression.WithoutLeadingTrivia();

            var constraintNode = shouldNode.FirstAncestor<MemberAccessExpressionSyntax>();
            var invocation = constraintNode.FirstAncestor<InvocationExpressionSyntax>();
            var arguments = invocation.ArgumentList.Arguments;

            switch (constraintNode.GetName())
            {
                case "BeTrue": return AssertThat(expression, Is("True"), arguments, 0, removeNameColon: true);
                case "BeFalse": return AssertThat(expression, Is("False"), arguments, 0, removeNameColon: true);

                case "BeNull": return AssertThat(expression, Is("Null"), arguments, 0, removeNameColon: true);
                case "NotBeNull": return AssertThat(expression, Is("Not", "Null"), arguments, 0, removeNameColon: true);

                case "BeEmpty": return await ConvertBeEmptyAsync(originalExpression, expression, arguments, document, cancellationToken).ConfigureAwait(false);
                case "NotBeEmpty": return await ConvertNotBeEmptyAsync(originalExpression, expression, arguments, document, cancellationToken).ConfigureAwait(false);

                case "BeNullOrEmpty": return AssertThat(expression, Is("Null", "Or", "Empty"), arguments, 0, removeNameColon: true);
                case "NotBeNullOrEmpty": return AssertThat(expression, Is("Not", "Null", "And", "Not", "Empty"), arguments, 0, removeNameColon: true);

                case "Be": return ConvertBe(expression, arguments);
                case "NotBe": return ConvertNotBe(expression, arguments);

                case "Equal": return AssertThat(expression, Is("EqualTo", arguments[0]), arguments, removeNameColon: true);

                case "BeEquivalentTo": return await ConvertBeEquivalentToAsync(expression, arguments, document, cancellationToken).ConfigureAwait(false);
                case "NotBeEquivalentTo": return await ConvertNotBeEquivalentToAsync(expression, arguments, document, cancellationToken).ConfigureAwait(false);

                case "BeGreaterThan": return AssertThat(expression, Is("GreaterThan", arguments[0]), arguments, removeNameColon: true);
                case "BeGreaterOrEqualTo":
                case "BeGreaterThanOrEqualTo": return AssertThat(expression, Is("GreaterThanOrEqualTo", arguments[0]), arguments, removeNameColon: true);

                case "BeLessThan": return AssertThat(expression, Is("LessThan", arguments[0]), arguments, removeNameColon: true);
                case "BeLessOrEqualTo":
                case "BeLessThanOrEqualTo": return AssertThat(expression, Is("LessThanOrEqualTo", arguments[0]), arguments, removeNameColon: true);

                case "BePositive": return AssertThat(expression, Is("Positive"), arguments, 0, removeNameColon: true);
                case "BeNegative": return AssertThat(expression, Is("Negative"), arguments, 0, removeNameColon: true);

                case "BeSameAs": return AssertThat(expression, Is("SameAs", arguments[0]), arguments, removeNameColon: true);
                case "NotBeSameAs": return AssertThat(expression, Is("Not", "SameAs", arguments[0]), arguments, removeNameColon: true);

                case "HaveValue": return AssertThat(expression, Is("Not", "Null"), arguments, 0, removeNameColon: true);
                case "NotHaveValue": return AssertThat(expression, Is("Null"), arguments, 0, removeNameColon: true);

                case "HaveCount": return AssertThat(expression, Has("Count", "EqualTo", arguments[0]), arguments, removeNameColon: true);
                case "HaveCountGreaterThan": return AssertThat(expression, Has("Count", "GreaterThan", arguments[0]), arguments, removeNameColon: true);
                case "HaveCountGreaterThanOrEqualTo": return AssertThat(expression, Has("Count", "GreaterThanOrEqualTo", arguments[0]), arguments, removeNameColon: true);
                case "HaveCountLessThan": return AssertThat(expression, Has("Count", "LessThan", arguments[0]), arguments, removeNameColon: true);
                case "HaveCountLessThanOrEqualTo": return AssertThat(expression, Has("Count", "LessThanOrEqualTo", arguments[0]), arguments, removeNameColon: true);
                case "NotHaveCount": return AssertThat(expression, Has("Count", "Not", "EqualTo", arguments[0]), arguments, removeNameColon: true);

                case "BeInRange": return AssertThat(expression, Is("InRange", arguments[0], arguments[1]), arguments, 2, removeNameColon: true);
                case "NotBeInRange": return AssertThat(expression, Is("Not", "InRange", arguments[0], arguments[1]), arguments, 2, removeNameColon: true);

                case "BeOneOf": return AssertThat(expression, Is("AnyOf", arguments[0]), arguments, removeNameColon: true);
                case "BeSubsetOf": return AssertThat(expression, Is("SubsetOf", arguments[0]), arguments, removeNameColon: true);
                case "NotBeSubsetOf": return AssertThat(expression, Is("Not", "SubsetOf", arguments[0]), arguments, removeNameColon: true);

                case "BeBlank":
                case "BeNullOrWhiteSpace":
                    return AssertThat(Argument(Invocation(nameof(String), nameof(string.IsNullOrWhiteSpace), Argument(expression))), Is("True"), arguments, 0, removeNameColon: true);

                case "NotBeBlank":
                case "NotBeNullOrWhiteSpace":
                    return AssertThat(Argument(Invocation(nameof(String), nameof(string.IsNullOrWhiteSpace), Argument(expression))), Is("False"), arguments, 0, removeNameColon: true);

                case "StartWith": return AssertThat(expression, Does("StartWith", arguments[0]), arguments, removeNameColon: true);
                case "StartWithEquivalentOf": return AssertThat(expression, Does("StartWith", arguments[0], "IgnoreCase"), arguments, removeNameColon: true);
                case "NotStartWith": return AssertThat(expression, Does("Not", "StartWith", arguments[0]), arguments, removeNameColon: true);
                case "NotStartWithEquivalentOf": return AssertThat(expression, Does("Not", "StartWith", arguments[0], "IgnoreCase"), arguments, removeNameColon: true);
                case "EndWith": return AssertThat(expression, Does("EndWith", arguments[0]), arguments, removeNameColon: true);
                case "EndWithEquivalentOf": return AssertThat(expression, Does("EndWith", arguments[0], "IgnoreCase"), arguments, removeNameColon: true);
                case "NotEndWith": return AssertThat(expression, Does("Not", "EndWith", arguments[0]), arguments, removeNameColon: true);
                case "NotEndWithEquivalentOf": return AssertThat(expression, Does("Not", "EndWith", arguments[0], "IgnoreCase"), arguments, removeNameColon: true);

                case "Contain" when arguments[0].Expression is SimpleLambdaExpressionSyntax: return await AssertThatHasMatchesAsync("Some", originalExpression, expression, arguments, document, cancellationToken).ConfigureAwait(false);
                case "Contain": return await ConvertContainAsync(expression, arguments, document, cancellationToken).ConfigureAwait(false);

                case "OnlyContain": return await AssertThatHasMatchesAsync("All", originalExpression, expression, arguments, document, cancellationToken).ConfigureAwait(false);
                case "ContainSingle" when arguments.Count > 0: return await AssertThatHasMatchesAsync("One", originalExpression, expression, arguments, document, cancellationToken).ConfigureAwait(false);
                case "ContainSingle": return AssertThat(expression, Has("Exactly", Argument(Literal(1)), "Items"), arguments, removeNameColon: true);
                case "NotContain" when arguments[0].Expression is SimpleLambdaExpressionSyntax: return await AssertThatHasMatchesAsync("None", originalExpression, expression, arguments, document, cancellationToken).ConfigureAwait(false);
                case "NotContain": return AssertThat(expression, Does("Not", "Contain", arguments[0]), arguments, removeNameColon: true);
                case "NotContainEquivalentOf": return AssertThat(expression, Does("Not", "Contain", arguments[0], "IgnoreCase"), arguments, removeNameColon: true);

                case "OnlyHaveUniqueItems": return AssertThat(expression, Is("Unique"), arguments, 0, removeNameColon: true);

                case "BeApproximately": return AssertThat(expression, Is("EqualTo", arguments[0], "Within", arguments[1]), arguments, 2, removeNameColon: true);
                case "NotBeApproximately": return AssertThat(expression, Is("Not", "EqualTo", arguments[0], "Within", arguments[1]), arguments, 2, removeNameColon: true);

                case "BeOfType" when constraintNode.Name is GenericNameSyntax g: return AssertThat(expression, Is("TypeOf", g.TypeArgumentList.Arguments.ToArray()), arguments, 0, removeNameColon: true);
                case "BeOfType": return AssertThat(expression, Is("TypeOf", arguments[0]), arguments, removeNameColon: true);

                case "BeXmlSerializable": return AssertThat(expression, Is("XmlSerializable"), arguments, 0, removeNameColon: true);
                case "BeBinarySerializable": return AssertThat(expression, Is("BinarySerializable"), arguments, 0, removeNameColon: true);

                case "BeInAscendingOrder": return AssertThat(expression, Is("Ordered", "Ascending"), arguments, 0, removeNameColon: true);
                case "NotBeInAscendingOrder": return AssertThat(expression, Is("Not", "Ordered", "Ascending"), arguments, 0, removeNameColon: true);
                case "BeInDescendingOrder": return AssertThat(expression, Is("Ordered", "Descending"), arguments, 0, removeNameColon: true);
                case "NotBeInDescendingOrder": return AssertThat(expression, Is("Not", "Ordered", "Descending"), arguments, 0, removeNameColon: true);

                default:
                    return null;
            }
        }

        private static async Task<InvocationExpressionSyntax> AssertThatHasMatchesAsync(
                                                                                    string match,
                                                                                    ExpressionSyntax originalExpression,
                                                                                    ExpressionSyntax expression,
                                                                                    SeparatedSyntaxList<ArgumentSyntax> arguments,
                                                                                    Document document,
                                                                                    CancellationToken cancellationToken)
        {
            var argument = arguments[0];

            if (argument.Expression is SimpleLambdaExpressionSyntax)
            {
                // we need to find out the type
                var type = await originalExpression.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

                if (type != null && type.TryGetGenericArguments(out var genericArguments))
                {
                    var types = new TypeSyntax[genericArguments.Length];

                    for (var i = 0; i < genericArguments.Length; i++)
                    {
                        types[i] = genericArguments[i].FullyQualifiedName().AsTypeSyntax();
                    }

                    return AssertThat(expression, Has(match, "Matches", argument, types), arguments, removeNameColon: true);
                }
            }

            return null;
        }

        private static InvocationExpressionSyntax ConvertBe(ExpressionSyntax expression, in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var argument = arguments[0];

            if (argument.Expression is LiteralExpressionSyntax literal)
            {
                switch (literal.Kind())
                {
                    case SyntaxKind.NullLiteralExpression:
                        return AssertThat(expression, Is("Null"), arguments, removeNameColon: true);

                    case SyntaxKind.TrueLiteralExpression:
                        return AssertThat(expression, Is("True"), arguments, removeNameColon: true);

                    case SyntaxKind.FalseLiteralExpression:
                        return AssertThat(expression, Is("False"), arguments, removeNameColon: true);
                }
            }

            return AssertThat(expression, Is("EqualTo", arguments[0]), arguments, removeNameColon: true);
        }

        private static InvocationExpressionSyntax ConvertNotBe(ExpressionSyntax expression, in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var argument = arguments[0];

            if (argument.Expression is LiteralExpressionSyntax literal)
            {
                switch (literal.Kind())
                {
                    case SyntaxKind.NullLiteralExpression:
                        return AssertThat(expression, Is("Not", "Null"), arguments, removeNameColon: true);

                    case SyntaxKind.TrueLiteralExpression:
                        return AssertThat(expression, Is("False"), arguments, removeNameColon: true);

                    case SyntaxKind.FalseLiteralExpression:
                        return AssertThat(expression, Is("True"), arguments, removeNameColon: true);
                }
            }

            return AssertThat(expression, Is("Not", "EqualTo", argument), arguments, removeNameColon: true);
        }

        private static async Task<InvocationExpressionSyntax> ConvertBeEmptyAsync(
                                                                              ExpressionSyntax originalExpression,
                                                                              ExpressionSyntax expression,
                                                                              SeparatedSyntaxList<ArgumentSyntax> arguments,
                                                                              Document document,
                                                                              CancellationToken cancellationToken)
        {
            var type = await originalExpression.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            if (type != null && type.IsGuid())
            {
                return AssertThat(expression, Is("EqualTo", Argument(nameof(Guid), nameof(Guid.Empty))), arguments, 0, removeNameColon: true);
            }

            return AssertThat(expression, Is("Empty"), arguments, 0, removeNameColon: true);
        }

        private static async Task<InvocationExpressionSyntax> ConvertNotBeEmptyAsync(
                                                                                 ExpressionSyntax originalExpression,
                                                                                 ExpressionSyntax expression,
                                                                                 SeparatedSyntaxList<ArgumentSyntax> arguments,
                                                                                 Document document,
                                                                                 CancellationToken cancellationToken)
        {
            var type = await originalExpression.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            if (type != null && type.IsGuid())
            {
                return AssertThat(expression, Is("Not", "EqualTo", Argument(nameof(Guid), nameof(Guid.Empty))), arguments, 0, removeNameColon: true);
            }

            return AssertThat(expression, Is("Not", "Empty"), arguments, 0, removeNameColon: true);
        }

        private static async Task<InvocationExpressionSyntax> ConvertBeEquivalentToAsync(ExpressionSyntax expression, SeparatedSyntaxList<ArgumentSyntax> arguments, Document document, CancellationToken cancellationToken)
        {
            var count = arguments.Count;

            if (count is 0)
            {
                return null;
            }

            var argument = arguments[0];

            var type = await argument.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            if (type != null)
            {
                if (count is 1 && type.IsString())
                {
                    // we have found the extension method that uses strings
                    return AssertThat(expression, Is("EqualTo", argument, "IgnoreCase"), arguments, removeNameColon: true);
                }

                if (type.IsEnumerable())
                {
                    // we have found the extension method that already gets an IEnumerable as first argument
                    return AssertThat(expression, Is("EquivalentTo", argument), arguments, removeNameColon: true);
                }
            }

            // seems like we have found the extension method that uses a params array as arguments
            return AssertThat(expression, Is("EquivalentTo", GetAsArray(arguments)), arguments, count, removeNameColon: true);
        }

        private static async Task<InvocationExpressionSyntax> ConvertNotBeEquivalentToAsync(ExpressionSyntax expression, SeparatedSyntaxList<ArgumentSyntax> arguments, Document document, CancellationToken cancellationToken)
        {
            var count = arguments.Count;

            if (count is 0)
            {
                return null;
            }

            var argument = arguments[0];

            var type = await argument.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            if (type != null)
            {
                if (count is 1 && type.IsString())
                {
                    // we have found the extension method that uses strings
                    return AssertThat(expression, Is("Not", "EqualTo", argument, "IgnoreCase"), arguments, removeNameColon: true);
                }

                if (type.IsEnumerable())
                {
                    // we have found the extension method that already gets an IEnumerable as first argument
                    return AssertThat(expression, Is("Not", "EquivalentTo", argument), arguments, removeNameColon: true);
                }
            }

            // seems like we have found the extension method that uses a params array as arguments
            return AssertThat(expression, Is("Not", "EquivalentTo", GetAsArray(arguments)), arguments, count, removeNameColon: true);
        }

        private static async Task<InvocationExpressionSyntax> ConvertContainAsync(ExpressionSyntax expression, SeparatedSyntaxList<ArgumentSyntax> arguments, Document document, CancellationToken cancellationToken)
        {
            var argument = arguments[0];

            var type = await argument.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            if (type != null && type.IsEnumerable())
            {
                // we have found the extension method that seems to compare 2 IEnumerables
                return AssertThat(expression, Is("SupersetOf", argument), arguments, removeNameColon: true);
            }

            return AssertThat(expression, Does("Contain", argument), arguments, removeNameColon: true);
        }

        private static ImplicitArrayCreationExpressionSyntax GetAsArray(in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var parameters = arguments.Select(_ => _.Expression).ToSeparatedSyntaxList();
            var initializer = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, parameters);
            var array = SyntaxFactory.ImplicitArrayCreationExpression(initializer);

            return array;
        }
    }
}
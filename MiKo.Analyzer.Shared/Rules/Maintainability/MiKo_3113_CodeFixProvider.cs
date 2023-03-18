using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3113_CodeFixProvider)), Shared]
    public class MiKo_3113_CodeFixProvider : UnitTestCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_3113_TestsDoNotUseFluentAssertionsAnalyzer.Id;

        protected sealed override string Title => Resources.MiKo_3113_CodeFixTitle;

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().First();

        protected sealed override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var statement = (ExpressionStatementSyntax)syntax;

            var shouldNode = MiKo_3113_TestsDoNotUseFluentAssertionsAnalyzer.GetIssue(statement);
            var assertThat = ConvertToAssertThat(context, shouldNode);

            if (assertThat is null)
            {
                return statement;
            }

            // find lambda
            var lambda = shouldNode.FirstAncestor<LambdaExpressionSyntax>();

            if (lambda != null && lambda.Ancestors().Any(_ => _ == statement))
            {
                // we have a lambda expression, so replace that one
                return statement.ReplaceNode(lambda, lambda.WithExpressionBody(assertThat));
            }

            return statement.WithExpression(assertThat).WithTriviaFrom(statement);
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic issue)
        {
            // only remove assertions if there are no more diagnostics
            // return root.WithUsing("NUnit.Framework").WithoutUsing("FluentAssertions");
            return root.WithUsing("NUnit.Framework");
        }

        private static InvocationExpressionSyntax ConvertToAssertThat(CodeFixContext context, MemberAccessExpressionSyntax shouldNode)
        {
            var originalExpression = shouldNode.Expression;

            var expression = originalExpression.WithoutLeadingTrivia();

            var constraintNode = shouldNode.FirstAncestor<MemberAccessExpressionSyntax>();
            var invocation = constraintNode.FirstAncestor<InvocationExpressionSyntax>();
            var arguments = invocation.ArgumentList.Arguments;

            var name = constraintNode.GetName();

            switch (name)
            {
                case "BeTrue": return AssertThat(expression, Is("True"), arguments, 0, removeNameColon: true);
                case "BeFalse": return AssertThat(expression, Is("False"), arguments, 0, removeNameColon: true);
                case "BeNull": return AssertThat(expression, Is("Null"), arguments, 0, removeNameColon: true);
                case "NotBeNull": return AssertThat(expression, Is("Not", "Null"), arguments, 0, removeNameColon: true);
                case "BeEmpty":
                {
                    var semanticModel = GetSemanticModel(context);
                    var type = originalExpression.GetTypeSymbol(semanticModel);

                    if (type != null)
                    {
                        if (type.IsGuid())
                        {
                            return AssertThat(expression, Is("EqualTo", Argument(SimpleMemberAccess(nameof(Guid), nameof(Guid.Empty)))), arguments, 0, removeNameColon: true);
                        }
                    }

                    return AssertThat(expression, Is("Empty"), arguments, 0, removeNameColon: true);
                }

                case "NotBeEmpty":
                {
                    var semanticModel = GetSemanticModel(context);
                    var type = originalExpression.GetTypeSymbol(semanticModel);

                    if (type != null)
                    {
                        if (type.IsGuid())
                        {
                            return AssertThat(expression, Is("Not", "EqualTo", Argument(SimpleMemberAccess(nameof(Guid), nameof(Guid.Empty)))), arguments, 0, removeNameColon: true);
                        }
                    }

                    return AssertThat(expression, Is("Not", "Empty"), arguments, 0, removeNameColon: true);
                }

                case "BeNullOrEmpty": return AssertThat(expression, Is("Null", "Or", "Empty"), arguments, 0, removeNameColon: true);
                case "NotBeNullOrEmpty": return AssertThat(expression, Is("Not", "Null", "And", "Not", "Empty"), arguments, 0, removeNameColon: true);
                case "Be": return AssertThat(expression, Is("EqualTo", arguments.First()), arguments, removeNameColon: true);
                case "NotBe": return AssertThat(expression, Is("Not", "EqualTo", arguments.First()), arguments, removeNameColon: true);
                case "Equal": return AssertThat(expression, Is("EqualTo", arguments.First()), arguments, removeNameColon: true);
                case "BeEquivalentTo":
                {
                    var count = arguments.Count;

                    if (count > 0)
                    {
                        var argument = arguments[0];

                        var semanticModel = GetSemanticModel(context);
                        var type = argument.GetTypeSymbol(semanticModel);

                        if (type != null)
                        {
                            if (count == 1 && type.IsString())
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

                    return null;
                }

                case "NotBeEquivalentTo":
                {
                    var count = arguments.Count;

                    if (count > 0)
                    {
                        var argument = arguments[0];

                        var semanticModel = GetSemanticModel(context);
                        var type = argument.GetTypeSymbol(semanticModel);

                        if (type != null)
                        {
                            if (count == 1 && type.IsString())
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

                    return null;
                }

                case "BeGreaterThan": return AssertThat(expression, Is("GreaterThan", arguments.First()), arguments, removeNameColon: true);
                case "BeGreaterOrEqualTo":
                case "BeGreaterThanOrEqualTo": return AssertThat(expression, Is("GreaterThanOrEqualTo", arguments.First()), arguments, removeNameColon: true);
                case "BeLessThan": return AssertThat(expression, Is("LessThan", arguments.First()), arguments, removeNameColon: true);
                case "BeLessOrEqualTo":
                case "BeLessThanOrEqualTo": return AssertThat(expression, Is("LessThanOrEqualTo", arguments.First()), arguments, removeNameColon: true);
                case "BePositive": return AssertThat(expression, Is("Positive"), arguments, 0, removeNameColon: true);
                case "BeNegative": return AssertThat(expression, Is("Negative"), arguments, 0, removeNameColon: true);
                case "BeSameAs": return AssertThat(expression, Is("SameAs", arguments.First()), arguments, removeNameColon: true);
                case "NotBeSameAs": return AssertThat(expression, Is("Not", "SameAs", arguments.First()), arguments, removeNameColon: true);
                case "HaveValue": return AssertThat(expression, Is("Not", "Null"), arguments, 0, removeNameColon: true);
                case "NotHaveValue": return AssertThat(expression, Is("Null"), arguments, 0, removeNameColon: true);
                case "HaveCount": return AssertThat(expression, HasCount("EqualTo", arguments.First()), arguments, removeNameColon: true);
                case "HaveCountGreaterThan": return AssertThat(expression, HasCount("GreaterThan", arguments.First()), arguments, removeNameColon: true);
                case "HaveCountGreaterThanOrEqualTo": return AssertThat(expression, HasCount("GreaterThanOrEqualTo", arguments.First()), arguments, removeNameColon: true);
                case "HaveCountLessThan": return AssertThat(expression, HasCount("LessThan", arguments.First()), arguments, removeNameColon: true);
                case "HaveCountLessThanOrEqualTo": return AssertThat(expression, HasCount("LessThanOrEqualTo", arguments.First()), arguments, removeNameColon: true);
                case "NotHaveCount": return AssertThat(expression, HasCount("Not", "EqualTo", arguments.First()), arguments, removeNameColon: true);
                case "BeInRange": return AssertThat(expression, Is("InRange", arguments[0], arguments[1]), arguments, 2, removeNameColon: true);
                case "NotBeInRange": return AssertThat(expression, Is("Not", "InRange", arguments[0], arguments[1]), arguments, 2, removeNameColon: true);
                case "BeOneOf": return AssertThat(expression, Is("AnyOf", arguments.First()), arguments, removeNameColon: true);
                case "BeSubsetOf": return AssertThat(expression, Is("SubsetOf", arguments.First()), arguments, removeNameColon: true);
                case "NotBeSubsetOf": return AssertThat(expression, Is("Not", "SubsetOf", arguments.First()), arguments, removeNameColon: true);

                case "BeBlank":
                case "BeNullOrWhiteSpace":
                    return AssertThat(Argument(Invocation(SimpleMemberAccess(nameof(String), nameof(string.IsNullOrWhiteSpace)), Argument(expression))), Is("True"), arguments, 0, removeNameColon: true);

                case "NotBeBlank":
                case "NotBeNullOrWhiteSpace":
                    return AssertThat(Argument(Invocation(SimpleMemberAccess(nameof(String), nameof(string.IsNullOrWhiteSpace)), Argument(expression))), Is("False"), arguments, 0, removeNameColon: true);

                case "StartWith": return AssertThat(expression, Does("StartWith", arguments.First()), arguments, removeNameColon: true);
                case "StartWithEquivalentOf": return AssertThat(expression, Does("StartWith", arguments.First(), "IgnoreCase"), arguments, removeNameColon: true);
                case "NotStartWith": return AssertThat(expression, Does("Not", "StartWith", arguments.First()), arguments, removeNameColon: true);
                case "NotStartWithEquivalentOf": return AssertThat(expression, Does("Not", "StartWith", arguments.First(), "IgnoreCase"), arguments, removeNameColon: true);
                case "EndWith": return AssertThat(expression, Does("EndWith", arguments.First()), arguments, removeNameColon: true);
                case "EndWithEquivalentOf": return AssertThat(expression, Does("EndWith", arguments.First(), "IgnoreCase"), arguments, removeNameColon: true);
                case "NotEndWith": return AssertThat(expression, Does("Not", "EndWith", arguments.First()), arguments, removeNameColon: true);
                case "NotEndWithEquivalentOf": return AssertThat(expression, Does("Not", "EndWith", arguments.First(), "IgnoreCase"), arguments, removeNameColon: true);

                case "Contain" when arguments.First().Expression is SimpleLambdaExpressionSyntax: return AssertThatHasMatches("Some");
                case "Contain":
                {
                    var argument = arguments.First();

                    var semanticModel = GetSemanticModel(context);
                    var type = argument.GetTypeSymbol(semanticModel);

                    if (type != null)
                    {
                        if (type.IsEnumerable())
                        {
                            // we have found the extension method that seems to compare 2 IEnumerables
                            return AssertThat(expression, Is("SupersetOf", argument), arguments, removeNameColon: true);
                        }
                    }

                    return AssertThat(expression, Does("Contain", argument), arguments, removeNameColon: true);
                }
                case "OnlyContain": return AssertThatHasMatches("All");
                case "ContainSingle" when arguments.Count > 0: return AssertThatHasMatches("One");
                case "ContainSingle": return AssertThat(expression, Has("Exactly", Argument(Literal(1)), "Items"), arguments, removeNameColon: true);
                case "NotContain" when arguments.First().Expression is SimpleLambdaExpressionSyntax: return AssertThatHasMatches("None");
                case "NotContain": return AssertThat(expression, Does("Not", "Contain", arguments[0]), arguments, removeNameColon: true);
                case "NotContainEquivalentOf": return AssertThat(expression, Does("Not", "Contain", arguments.First(), "IgnoreCase"), arguments, removeNameColon: true);

                case "OnlyHaveUniqueItems": return AssertThat(expression, Is("Unique"), arguments, 0, removeNameColon: true);

                case "BeApproximately": return AssertThat(expression, Is("EqualTo", arguments[0], "Within", arguments[1]), arguments, 2, removeNameColon: true);
                case "NotBeApproximately": return AssertThat(expression, Is("Not", "EqualTo", arguments[0], "Within", arguments[1]), arguments, 2, removeNameColon: true);

                case "BeOfType" when constraintNode.Name is GenericNameSyntax g: return AssertThat(expression, Is("TypeOf", g.TypeArgumentList.Arguments.ToArray()), arguments, 0, removeNameColon: true);
                case "BeOfType": return AssertThat(expression, Is("TypeOf", arguments[0]), arguments, 1, removeNameColon: true);

                case "BeXmlSerializable": return AssertThat(expression, Is("XmlSerializable"), arguments, 0, removeNameColon: true);
                case "BeBinarySerializable": return AssertThat(expression, Is("BinarySerializable"), arguments, 0, removeNameColon: true);

                case "BeInAscendingOrder": return AssertThat(expression, Is("Ordered", "Ascending"), arguments, 0, removeNameColon: true);
                case "NotBeInAscendingOrder": return AssertThat(expression, Is("Not", "Ordered", "Ascending"), arguments, 0, removeNameColon: true);
                case "BeInDescendingOrder": return AssertThat(expression, Is("Ordered", "Descending"), arguments, 0, removeNameColon: true);
                case "NotBeInDescendingOrder": return AssertThat(expression, Is("Not", "Ordered", "Descending"), arguments, 0, removeNameColon: true);

                default:
                    return null;
            }

            InvocationExpressionSyntax AssertThatHasMatches(string match)
            {
                var argument = arguments[0];

                if (argument.Expression is SimpleLambdaExpressionSyntax)
                {
                    // we need to find out the type
                    var semanticModel = GetSemanticModel(context);
                    var type = originalExpression.GetTypeSymbol(semanticModel);

                    if (type != null && type.TryGetGenericArgumentCount(out var genericArgumentsCount))
                    {
                        var types = new TypeSyntax[genericArgumentsCount];

                        for (var i = 0; i < genericArgumentsCount; i++)
                        {
                            if (type.TryGetGenericArgumentType(out var genericType, i))
                            {
                                types[i] = SyntaxFactory.ParseTypeName(genericType.FullyQualifiedName());
                            }
                        }

                        return AssertThat(expression, Has(match, "Matches", argument, types), arguments, removeNameColon: true);
                    }
                }

                return null;
            }
        }

        private static ImplicitArrayCreationExpressionSyntax GetAsArray(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var parameters = SyntaxFactory.SeparatedList(arguments.Select(_ => _.Expression));
            var initializer = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, parameters);
            var array = SyntaxFactory.ImplicitArrayCreationExpression(initializer);

            return array;
        }
    }
}
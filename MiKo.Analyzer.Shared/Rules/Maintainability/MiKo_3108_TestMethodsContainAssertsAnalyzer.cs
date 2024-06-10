using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3108_TestMethodsContainAssertsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3108";

        private const int MaximumNestingLevel = 5;

        public MiKo_3108_TestMethodsContainAssertsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => (symbol.ReturnsVoid || symbol.ReturnType.IsTask()) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => ContainsAssertion(symbol, compilation, 0)
                                                                                                             ? Enumerable.Empty<Diagnostic>()
                                                                                                             : new[] { Issue(symbol) };

        private static bool ContainsAssertion(IMethodSymbol method, Compilation compilation, int nestingLevel)
        {
            if (nestingLevel > MaximumNestingLevel)
            {
                // we did not find any and break up because call tree is too deep
                return false;
            }

            // increase level
            nestingLevel++;

            if (method.GetSyntax() is MethodDeclarationSyntax syntax)
            {
                if (ContainsAssertionDirectly(syntax))
                {
                    return true;
                }

                var type = method.ContainingType;

                // maybe it is a nested call, so check for invocation expressions or expression statements
                var arrowClause = syntax.ExpressionBody;

                if (arrowClause != null)
                {
                    return arrowClause.Expression is InvocationExpressionSyntax i && ContainsAssertion(type, compilation, i, nestingLevel);
                }

                var body = syntax.Body;

                if (body != null)
                {
                    foreach (var statement in body.DescendantNodes<ExpressionStatementSyntax>())
                    {
                        if (statement.Expression is InvocationExpressionSyntax i && ContainsAssertion(type, compilation, i, nestingLevel))
                        {
                            return true;
                        }
                    }

                    // we did not find any assertion
                    return false;
                }
            }

            // no method, so ignore
            return true;
        }

        private static bool ContainsAssertion(ITypeSymbol type, Compilation compilation, InvocationExpressionSyntax invocation, int nestingLevel)
        {
            var name = invocation.GetName();

            var allMethods = type.GetMembersIncludingInherited<IMethodSymbol>(name).ToList();

            if (allMethods.Count > 0)
            {
                if (invocation.GetSymbol(compilation) is IMethodSymbol invokedMethod)
                {
                    if (allMethods.Contains(invokedMethod, SymbolEqualityComparer.Default))
                    {
                        // search for the method
                        return ContainsAssertion(invokedMethod, compilation, nestingLevel);
                    }

                    // we might have a method that has been constructed from another method, so inspect that one
                    var constructed = invokedMethod.ConstructedFrom;

                    if (allMethods.Contains(constructed, SymbolEqualityComparer.Default))
                    {
                        // search for the method
                        return ContainsAssertion(constructed, compilation, nestingLevel);
                    }
                }
            }

            return false;
        }

        private static bool ContainsAssertionDirectly(MethodDeclarationSyntax syntax)
        {
            var nodes = syntax.DescendantNodes<MemberAccessExpressionSyntax>(SyntaxKind.SimpleMemberAccessExpression);

            foreach (var node in nodes)
            {
                var argumentsCount = node.Parent is InvocationExpressionSyntax i
                                     ? i.ArgumentList.Arguments.Count
                                     : -1;

                switch (node.GetName())
                {
                    // we assume that this is an fluent assertion
                    case "Should" when argumentsCount == 0:
                    case "ShouldNotRaise":
                    case "ShouldRaise":
                    case "ShouldRaisePropertyChangeFor":
                    case "ShouldThrow":
                    {
                        return true;
                    }

                    // we assume that this is a Moq call
                    case Constants.Moq.VerifyGet when argumentsCount > 0:
                    case Constants.Moq.VerifySet when argumentsCount > 0:
                    case Constants.Moq.VerifyAll when argumentsCount == 0:
                    case Constants.Moq.Verify when argumentsCount > 0:
                    {
                        return true;
                    }

                    case Constants.Moq.Verify when argumentsCount == 0:
                    {
                        if (node.Expression is IdentifierNameSyntax ins)
                        {
                            var mockName = ins.GetName();

                            // no arguments, so check for a 'Verifiable' call on the same mock object
                            return nodes.Where(_ => _.GetName() == Constants.Moq.Verifiable && _.Parent is InvocationExpressionSyntax)
                                        .SelectMany(_ => _.DescendantNodes<MemberAccessExpressionSyntax>())
                                        .Any(_ => _.Expression is IdentifierNameSyntax e && e.GetName() == mockName);
                        }

                        // seems like another call, so investigate further
                        continue;
                    }

                    // we assume that this is a NSubstitute call
                    case "Received" when argumentsCount == 0 || argumentsCount == 1:
                    case "ReceivedWithAnyArgs" when argumentsCount == 0:
                    case "DidNotReceive" when argumentsCount == 0:
                    case "DidNotReceiveWithAnyArgs" when argumentsCount == 0:
                    {
                        return true;
                    }
                }

                if (node.Expression is IdentifierNameSyntax invokedClass && Constants.Names.AssertionTypes.Contains(invokedClass.GetName()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
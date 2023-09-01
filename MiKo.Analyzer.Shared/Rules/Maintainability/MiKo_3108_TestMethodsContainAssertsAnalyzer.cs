﻿using System.Collections.Generic;
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

        public MiKo_3108_TestMethodsContainAssertsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => (symbol.ReturnsVoid || symbol.ReturnType.IsTask()) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (ContainsAssertion(symbol, compilation) is false)
            {
                yield return Issue(symbol);
            }
        }

        private static bool ContainsAssertion(IMethodSymbol method, Compilation compilation)
        {
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
                    return arrowClause.Expression is InvocationExpressionSyntax i && ContainsAssertion(type, compilation, i);
                }

                var body = syntax.Body;

                if (body != null)
                {
                    foreach (var statement in body.DescendantNodes<ExpressionStatementSyntax>())
                    {
                        if (statement.Expression is InvocationExpressionSyntax i && ContainsAssertion(type, compilation, i))
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

        private static bool ContainsAssertion(ITypeSymbol type, Compilation compilation, InvocationExpressionSyntax invocation)
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
                        return ContainsAssertion(invokedMethod, compilation);
                    }

                    // we might have a method that has been constructed from another method, so inspect that one
                    var constructed = invokedMethod.ConstructedFrom;

                    if (allMethods.Contains(constructed, SymbolEqualityComparer.Default))
                    {
                        // search for the method
                        return ContainsAssertion(constructed, compilation);
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
                switch (node.GetName())
                {
                    // we assume that this is an fluent assertion
                    case "Should" when node.Parent is InvocationExpressionSyntax i && i.ArgumentList.Arguments.Count == 0:
                    case "ShouldNotRaise":
                    case "ShouldRaise":
                    case "ShouldRaisePropertyChangeFor":
                    case "ShouldThrow":
                    {
                        return true;
                    }

                    // we assume that this is a Moq call
                    case "Verify" when node.Parent is InvocationExpressionSyntax i:
                    {
                        var argumentsCount = i.ArgumentList.Arguments.Count;

                        if (argumentsCount > 0)
                        {
                            return true;
                        }

                        if (node.Expression is IdentifierNameSyntax ins)
                        {
                            var mockName = ins.GetName();

                            // no arguments, so check for a 'Verifiable' call on the same mock object
                            return nodes.Where(_ => _.GetName() == "Verifiable" && _.Parent is InvocationExpressionSyntax)
                                        .SelectMany(_ => _.DescendantNodes<MemberAccessExpressionSyntax>())
                                        .Any(_ => _.Expression is IdentifierNameSyntax e && e.GetName() == mockName);
                        }

                        return false;
                    }

                    case "VerifyGet" when node.Parent is InvocationExpressionSyntax i1 && i1.ArgumentList.Arguments.Count > 0:
                    case "VerifySet" when node.Parent is InvocationExpressionSyntax i2 && i2.ArgumentList.Arguments.Count > 0:
                    case "VerifyAll" when node.Parent is InvocationExpressionSyntax i3 && i3.ArgumentList.Arguments.Count == 0:
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
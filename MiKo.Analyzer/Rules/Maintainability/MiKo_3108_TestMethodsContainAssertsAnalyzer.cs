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

        public MiKo_3108_TestMethodsContainAssertsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => (symbol.ReturnsVoid || symbol.ReturnType.IsTask()) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => ContainsAssertion(symbol)
                                                                                                                 ? Enumerable.Empty<Diagnostic>()
                                                                                                                 : new[] { Issue(symbol) };

        private static bool ContainsAssertion(IMethodSymbol symbol)
        {
            var syntax = symbol.GetSyntax();

            var nodes = syntax.DescendantNodes().Where(_ => _.IsKind(SyntaxKind.SimpleMemberAccessExpression)).OfType<MemberAccessExpressionSyntax>();
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
                                        .SelectMany(_ => _.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
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
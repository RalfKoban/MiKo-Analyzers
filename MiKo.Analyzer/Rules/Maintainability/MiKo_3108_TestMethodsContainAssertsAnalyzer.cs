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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => (symbol.ReturnsVoid || symbol.ReturnType.IsTask()) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol) => ContainsAssertion(symbol)
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
                    case "Should" when node.Parent is InvocationExpressionSyntax i && i.ArgumentList.Arguments.Count == 0:
                    case "ShouldNotRaise":
                    case "ShouldRaise":
                    case "ShouldRaisePropertyChangeFor":
                    case "ShouldThrow":
                    {
                        // we assume that this is an fluent assertion
                        return true;
                    }

                    case "Verify" when node.Parent is InvocationExpressionSyntax i && i.ArgumentList.Arguments.Count > 0:
                    case "VerifyGet" when node.Parent is InvocationExpressionSyntax i1 && i1.ArgumentList.Arguments.Count > 0:
                    case "VerifySet" when node.Parent is InvocationExpressionSyntax i2 && i2.ArgumentList.Arguments.Count > 0:
                    case "VerifyAll" when node.Parent is InvocationExpressionSyntax i3 && i3.ArgumentList.Arguments.Count == 0:
                    {
                        // we assume that this is a Moq call
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
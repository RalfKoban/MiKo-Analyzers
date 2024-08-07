﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3109_TestAssertsHaveMessageAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3109";

        private static readonly int[] Zero = { 0 };
        private static readonly int[] One = { 1 };
        private static readonly int[] Two = { 2 };
        private static readonly int[] OneTwo = { 1, 2 };
        private static readonly int[] TwoThree = { 2, 3 };

        public MiKo_3109_TestAssertsHaveMessageAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool SupportsXUnit => false; // Xunit does not support assertion messages, see https://github.com/xunit/xunit/issues/350

        protected override bool ShallAnalyze(IMethodSymbol symbol) => (symbol.ReturnsVoid || symbol.ReturnType.IsTask()) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var assertions = GetAllAssertions(symbol.GetSyntax());

            if (assertions.Count > 1)
            {
                foreach (var assertion in assertions.Where(_ => HasMessageParameter(_, compilation) is false))
                {
                    yield return Issue(assertion);
                }
            }
        }

        private static IReadOnlyCollection<MemberAccessExpressionSyntax> GetAllAssertions(SyntaxNode methodSyntax) => methodSyntax.DescendantNodes<MemberAccessExpressionSyntax>(SyntaxKind.SimpleMemberAccessExpression)
                                                                                                                                  .Where(IsAssertionMethod)
                                                                                                                                  .ToList();

        private static bool IsAssertionMethod(MemberAccessExpressionSyntax node) => node.Expression is IdentifierNameSyntax invokedType && Constants.Names.AssertionTypes.Contains(invokedType.GetName()) && node.GetName() != "Multiple";

        private static bool HasMessageParameter(MemberAccessExpressionSyntax assertion, Compilation compilation) => assertion.Parent is InvocationExpressionSyntax i && HasMessageParameter(i, GetExpectedMessageParameterIndices(assertion.GetName()), compilation);

        private static bool HasMessageParameter(InvocationExpressionSyntax i, IReadOnlyList<int> expectedParameterIndices, Compilation compilation)
        {
            // last parameter must be a string, and it must be at least the 3rd parameter (for assert.AreEqual)
            var arguments = i.ArgumentList.Arguments;
            var count = arguments.Count;

            if (count == 0)
            {
                // we have no message
                return false;
            }

            if (expectedParameterIndices.Count == 1)
            {
                var expectedParameterIndex = expectedParameterIndices[0];

                var index = count - 1;

                if (expectedParameterIndex == index)
                {
                    // we have a message
                    return arguments[index].IsStringLiteral();
                }

                if (expectedParameterIndex > index)
                {
                    // we have no message
                    return false;
                }

                // we have some parameters and some arguments
                return arguments[expectedParameterIndex].IsStringLiteral();
            }

            // we are unsure, so we have to test
            foreach (var expectedParameterIndex in expectedParameterIndices)
            {
                if (expectedParameterIndex >= count)
                {
                    // we for sure do not have a message
                    return false;
                }

                var argument = arguments[expectedParameterIndex];

                if (argument.IsStringLiteral())
                {
                    return true;
                }

                // we seem to concatenate strings
                if (argument.Expression.IsStringConcatenation())
                {
                    return true;
                }

                // we are still unsure, so we have to test via semantics
                if (argument.Expression is IdentifierNameSyntax && argument.GetTypeSymbol(compilation).IsString())
                {
                    return true;
                }
            }

            return false;
        }

        private static int[] GetExpectedMessageParameterIndices(string methodName)
        {
            switch (methodName)
            {
                case "Fail":
                case "Ignore":
                case "Inconclusive":
                case "Pass":
                    return Zero;

                case "AllItemsAreNotNull":
                case "AllItemsAreUnique":
                case "DoesNotExist":
                case "DoesNotThrow":
                case "DoesNotThrowAsync":
                case "Exists":
                case "False":
                case "IsEmpty":
                case "IsFalse":
                case "IsNaN":
                case "IsNotEmpty":
                case "IsNotNull":
                case "IsNull":
                case "IsTrue":
                case "Negative":
                case "NotNull":
                case "NotZero":
                case "Null":
                case "Positive":
                case "True":
                case "Zero":
                    return One;

                case "AllItemsAreInstancesOfType":
                case "AreEqualIgnoringCase":
                case "AreEquivalent":
                case "AreNotEqual":
                case "AreNotEqualIgnoringCase":
                case "AreNotEquivalent":
                case "AreNotSame":
                case "AreSame":
                case "Contains":
                case "DoesNotContain":
                case "DoesNotEndWith":
                case "DoesNotMatch":
                case "DoesNotStartsWith":
                case "EndsWith":
                case "Greater":
                case "GreaterOrEqual":
                case "IsMatch":
                case "IsNotSubsetOf":
                case "IsSubsetOf":
                case "Less":
                case "LessOrEqual":
                case "StartsWith":
                    return Two;

                case "AreEqual": // 2 or 3, cannot determine for sure
                    return TwoThree;

                case "Catch": // 1 or 2, cannot determine for sure
                case "CatchAsync": // 1 or 2, cannot determine for sure
                case "IsAssignableFrom": // 1 or 2, cannot determine for sure (based on Generics)
                case "IsInstanceOf": // 1 or 2, cannot determine for sure (based on Generics)
                case "IsNotAssignableFrom": // 1 or 2, cannot determine for sure (based on Generics)
                case "IsNotInstanceOf": // 1 or 2, cannot determine for sure (based on Generics)
                case "IsOrdered": // 1 or 2, cannot determine for sure (based on comparer as last parameter)
                case "That": // 1 or 2, cannot determine for sure (based on comparer as last parameter)
                case "Throws": // 1 or 2, cannot determine for sure (based on Generics)
                case "ThrowsAsync": // 1 or 2, cannot determine for sure (based on Generics)
                    return OneTwo;

                default:
                    return Zero;
            }
        }
    }
}
using System.Collections.Generic;
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

        public MiKo_3109_TestAssertsHaveMessageAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => (symbol.ReturnsVoid || symbol.ReturnType.IsTask()) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var assertions = GetAllAssertions(symbol.GetSyntax());
            if (assertions.Count > 1)
            {
                foreach (var assertion in assertions.Where(_ => HasMessageParameter(_) is false))
                {
                    yield return Issue(assertion);
                }
            }
        }

        private static IReadOnlyCollection<MemberAccessExpressionSyntax> GetAllAssertions(SyntaxNode methodSyntax) => methodSyntax.DescendantNodes()
                                                                                                                                  .Where(_ => _.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                                                                                                                                  .OfType<MemberAccessExpressionSyntax>()
                                                                                                                                  .Where(IsAssertionMethod)
                                                                                                                                  .ToList();

        private static bool IsAssertionMethod(MemberAccessExpressionSyntax node) => node.Expression is IdentifierNameSyntax invokedType && Constants.Names.AssertionTypes.Contains(invokedType.GetName()) && node.GetName() != "Multiple";

        private static bool HasMessageParameter(MemberAccessExpressionSyntax assertion) => assertion.Parent is InvocationExpressionSyntax i && HasMessageParameter(i, GetExpectedMessageParameterIndices(assertion.GetName()));

        private static bool HasMessageParameter(InvocationExpressionSyntax i, int[] expectedParameterNames)
        {
            // last parameter must be a string and it must be at least the 3rd parameter (for assert.AreEqual)
            var arguments = i.ArgumentList.Arguments;
            var count = arguments.Count;

            if (count == 0)
            {
                // we have no message
                return false;
            }

            if (expectedParameterNames.Length == 1)
            {
                var expectedParameterName = expectedParameterNames[0];

                var index = count - 1;
                if (expectedParameterName == index)
                {
                    // we have a message
                    return IsString(arguments[index]);
                }

                if (expectedParameterName > index)
                {
                    // we have no message
                    return false;
                }

                // we have some parameters and some arguments
                return IsString(arguments[expectedParameterName]);
            }

            // we are unsure, so we have to test
            foreach (var expectedParameterName in expectedParameterNames)
            {
                if (expectedParameterName >= count)
                {
                    // we for sure do not have a message
                    return false;
                }

                if (IsString(arguments[expectedParameterName]))
                {
                    // TODO: we might have a message, but maybe it is also just a normal string parameter
                    return true;
                }
            }

            return false;
        }

        private static bool IsString(ArgumentSyntax syntax)
        {
            switch (syntax.Expression.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.InterpolatedStringExpression:
                    return true;

                default:
                    return false;
            }
        }

#pragma warning disable CA1502 // Avoid excessive complexity
        private static int[] GetExpectedMessageParameterIndices(string methodName)
        {
            switch (methodName)
            {
                case "Fail":
                case "Ignore":
                case "Inconclusive":
                case "Pass":
                    return new[] { 0 };

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
                    return new[] { 1 };

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
                    return new[] { 2 };

                case "AreEqual": // 2 or 3, cannot determine for sure
                    return new[] { 2, 3 };

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
                    return new[] { 1, 2 };

                default:
                    return new[] { 0 };
            }
        }
#pragma warning restore CA1502
    }
}
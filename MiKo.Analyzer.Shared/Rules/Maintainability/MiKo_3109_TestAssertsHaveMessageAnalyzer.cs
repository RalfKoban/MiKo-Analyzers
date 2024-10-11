using System.Collections.Concurrent;
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

        private static readonly int[] Zero = { 0 };
        private static readonly int[] One = { 1 };
        private static readonly int[] Two = { 2 };
        private static readonly int[] OneTwo = { 1, 2 };
        private static readonly int[] TwoThree = { 2, 3 };

        private static readonly ConcurrentDictionary<string, int[]> MethodNameToArgumentIndicesMapping = new ConcurrentDictionary<string, int[]>(new[]
                                                                                                                                                     {
                                                                                                                                                         // 0 arguments
                                                                                                                                                         new KeyValuePair<string, int[]>("Fail", Zero),
                                                                                                                                                         new KeyValuePair<string, int[]>("Ignore", Zero),
                                                                                                                                                         new KeyValuePair<string, int[]>("Inconclusive", Zero),
                                                                                                                                                         new KeyValuePair<string, int[]>("Pass", Zero),

                                                                                                                                                         // 1 argument
                                                                                                                                                         new KeyValuePair<string, int[]>("AllItemsAreNotNull", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("AllItemsAreUnique", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("DoesNotExist", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("DoesNotThrow", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("DoesNotThrowAsync", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("Exists", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("False", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsEmpty", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsFalse", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsNaN", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsNotEmpty", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsNotNull", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsNull", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsTrue", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("Negative", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("NotNull", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("NotZero", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("Null", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("Positive", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("True", One),
                                                                                                                                                         new KeyValuePair<string, int[]>("Zero", One),

                                                                                                                                                         // 2 arguments
                                                                                                                                                         new KeyValuePair<string, int[]>("AllItemsAreInstancesOfType", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("AreEqualIgnoringCase", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("AreEquivalent", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("AreNotEqual", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("AreNotEqualIgnoringCase", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("AreNotEquivalent", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("AreNotSame", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("AreSame", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("Contains", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("DoesNotContain", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("DoesNotEndWith", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("DoesNotMatch", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("DoesNotStartsWith", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("EndsWith", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("Greater", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("GreaterOrEqual", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsMatch", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsNotSubsetOf", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsSubsetOf", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("Less", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("LessOrEqual", Two),
                                                                                                                                                         new KeyValuePair<string, int[]>("StartsWith", Two),

                                                                                                                                                         // 2 or 3 arguments, cannot determine for sure
                                                                                                                                                         new KeyValuePair<string, int[]>("AreEqual", TwoThree),

                                                                                                                                                         // 1 or 2 arguments, cannot determine for sure
                                                                                                                                                         new KeyValuePair<string, int[]>("Catch", OneTwo),
                                                                                                                                                         new KeyValuePair<string, int[]>("CatchAsync", OneTwo),
                                                                                                                                                         new KeyValuePair<string, int[]>("IsAssignableFrom", OneTwo), // (based on Generics)
                                                                                                                                                         new KeyValuePair<string, int[]>("IsInstanceOf", OneTwo), // (based on Generics)
                                                                                                                                                         new KeyValuePair<string, int[]>("IsNotAssignableFrom", OneTwo), // (based on Generics)
                                                                                                                                                         new KeyValuePair<string, int[]>("IsNotInstanceOf", OneTwo), // (based on Generics)
                                                                                                                                                         new KeyValuePair<string, int[]>("IsOrdered", OneTwo), // (based on comparer as last parameter)
                                                                                                                                                         new KeyValuePair<string, int[]>("That", OneTwo), // (based on comparer as last parameter)
                                                                                                                                                         new KeyValuePair<string, int[]>("Throws", OneTwo), // (based on Generics)
                                                                                                                                                         new KeyValuePair<string, int[]>("ThrowsAsync", OneTwo), // (based on Generics)
                                                                                                                                                     });

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

        private static bool HasMessageParameter(MemberAccessExpressionSyntax assertion, Compilation compilation)
        {
            if (assertion.Parent is InvocationExpressionSyntax invocation)
            {
                var arguments = invocation.ArgumentList.Arguments;

                if (arguments.Count == 0)
                {
                    // we have no message
                    return false;
                }

                var methodName = assertion.GetName();
                var expectedArgumentIndices = MethodNameToArgumentIndicesMapping.TryGetValue(methodName, out var result) ? result : Zero;

                return HasMessageParameter(arguments, expectedArgumentIndices, compilation);
            }

            return false;
        }

        private static bool HasMessageParameter(SeparatedSyntaxList<ArgumentSyntax> arguments, IReadOnlyList<int> expectedArgumentIndices, Compilation compilation)
        {
            var count = arguments.Count;

            if (expectedArgumentIndices.Count == 1)
            {
                var expectedArgumentIndex = expectedArgumentIndices[0];

                var index = count - 1;

                if (expectedArgumentIndex == index)
                {
                    // we have a message
                    return arguments[index].IsStringLiteral();
                }

                if (expectedArgumentIndex > index)
                {
                    // we have no message
                    return false;
                }

                // we have some parameters and some arguments
                return arguments[expectedArgumentIndex].IsStringLiteral();
            }

            // we are unsure, so we have to test
            foreach (var expectedParameterIndex in expectedArgumentIndices)
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
    }
}
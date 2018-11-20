using System.Collections.Generic;

using NUnit.Framework;

namespace TestHelper
{
    partial class CodeFixVerifier
    {
        public static readonly IEnumerable<string> TestFixtures = new HashSet<string>
                                                                      {
                                                                          nameof(TestFixtureAttribute),
                                                                          "TestFixture",
                                                                          "TestClassAttribute",
                                                                          "TestClass",
                                                                      };

        public static readonly IEnumerable<string> Tests = new HashSet<string>
                                                               {
                                                                   nameof(TestAttribute),
                                                                   nameof(TestCaseAttribute),
                                                                   nameof(TestCaseSourceAttribute),
                                                                   nameof(TheoryAttribute),
                                                                   "Test",
                                                                   "TestCase",
                                                                   "TestCaseSource",
                                                                   "Theory",
                                                                   "TestMethodAttribute",
                                                                   "TestMethod",
                                                               };
    }
}
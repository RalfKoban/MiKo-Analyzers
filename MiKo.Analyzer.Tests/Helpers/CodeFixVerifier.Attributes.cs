using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace TestHelper
{
    partial class CodeFixVerifier
    {
        public static readonly IEnumerable<string> TestFixtures = new HashSet<string>
                                                                      {
                                                                          nameof(TestFixtureAttribute),
                                                                          "TestFixture",
                                                                          "TestFixture()",
                                                                          "TestFixtureAttribute()",
                                                                          "TestClassAttribute",
                                                                          "TestClassAttribute()",
                                                                          "TestClass",
                                                                          "TestClass()",
                                                                      };

        public static readonly IEnumerable<string> TestSetUps = new HashSet<string>
                                                                    {
                                                                        nameof(SetUpAttribute),
                                                                        "SetUp",
                                                                        "SetUp()",
                                                                        "SetUpAttribute()",
                                                                        "TestInitialize",
                                                                        "TestInitialize()",
                                                                        "TestInitializeAttribute",
                                                                        "TestInitializeAttribute()",
                                                                    };

        public static readonly IEnumerable<string> TestTearDowns = new HashSet<string>
                                                                       {
                                                                           "TearDown",
                                                                           "TearDown()",
                                                                           nameof(TearDownAttribute),
                                                                           "TearDownAttribute()",
                                                                           "TestCleanup",
                                                                           "TestCleanup()",
                                                                           "TestCleanupAttribute",
                                                                           "TestCleanupAttribute()",
                                                                       };

        public static readonly IEnumerable<string> TestsExceptSetUpTearDowns = new HashSet<string>
                                                                                   {
                                                                                       nameof(TestAttribute),
                                                                                       nameof(TestCaseAttribute),
                                                                                       nameof(TestCaseSourceAttribute),
                                                                                       nameof(TheoryAttribute),
                                                                                       "Fact",
                                                                                       "Fact()",
                                                                                       "Test",
                                                                                       "Test()",
                                                                                       "TestCase",
                                                                                       "TestCase()",
                                                                                       "TestCaseAttribute",
                                                                                       "TestCaseAttribute()",
                                                                                       "TestCaseSource",
                                                                                       "TestCaseSource()",
                                                                                       "TestCaseSourceAttribute()",
                                                                                       "Theory",
                                                                                       "Theory()",
                                                                                       "TheoryAttribute",
                                                                                       "TheoryAttribute()",
                                                                                       "TestMethod",
                                                                                       "TestMethod()",
                                                                                       "TestMethodAttribute",
                                                                                       "TestMethodAttribute()",
                                                                                   };

        public static readonly IEnumerable<string> Tests = TestsExceptSetUpTearDowns.Concat(TestSetUps).Concat(TestTearDowns).ToHashSet();
        public static readonly IEnumerable<string> TestsExceptSetUps = Tests.Except(TestSetUps).ToHashSet();
        public static readonly IEnumerable<string> TestsExceptTearDowns = Tests.Except(TestTearDowns).ToHashSet();
    }
}
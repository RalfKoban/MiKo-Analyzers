using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace TestHelper
{
    partial class CodeFixVerifier
    {
        public static readonly IEnumerable<string> TestFixtures = new HashSet<string>
                                                                      {
                                                                          "TestFixture",
                                                                          "TestFixture()",
                                                                          nameof(TestFixtureAttribute),
                                                                          nameof(TestFixtureAttribute) + "()",
                                                                          "TestClassAttribute",
                                                                          "TestClassAttribute()",
                                                                          "TestClass",
                                                                          "TestClass()",
                                                                      };

        public static readonly IEnumerable<string> TestSetUps = new HashSet<string>
                                                                    {
                                                                        "SetUp",
                                                                        "SetUp()",
                                                                        nameof(SetUpAttribute),
                                                                        nameof(SetUpAttribute) + "()",
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
                                                                           nameof(TearDownAttribute) + "()",
                                                                           "TestCleanup",
                                                                           "TestCleanup()",
                                                                           "TestCleanupAttribute",
                                                                           "TestCleanupAttribute()",
                                                                       };

        public static readonly IEnumerable<string> TestsExceptSetUpTearDowns = new HashSet<string>
                                                                                   {
                                                                                       nameof(TestAttribute),
                                                                                       nameof(TestAttribute) + "()",
                                                                                       nameof(TestCaseAttribute),
                                                                                       nameof(TestCaseAttribute) + "()",
                                                                                       nameof(TestCaseSourceAttribute),
                                                                                       nameof(TestCaseSourceAttribute) + "()",
                                                                                       nameof(TheoryAttribute),
                                                                                       nameof(TheoryAttribute) + "()",
                                                                                       "Fact",
                                                                                       "Fact()",
                                                                                       "Test",
                                                                                       "Test()",
                                                                                       "TestCase",
                                                                                       "TestCase()",
                                                                                       "TestCaseSource",
                                                                                       "TestCaseSource()",
                                                                                       "Theory",
                                                                                       "Theory()",
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
using System.Collections.Generic;

using NUnit.Framework;

namespace TestHelper
{
    public partial class CodeFixVerifier
    {
        public static readonly IEnumerable<string> TestFixtures = new[]
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

        public static readonly IEnumerable<string> TestSetUps = new[]
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

        public static readonly IEnumerable<string> TestTearDowns = new[]
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

        public static readonly IEnumerable<string> TestOneTimeSetUps = new[]
                                                                       {
                                                                           "OneTimeSetUp",
                                                                           "OneTimeSetUp()",
                                                                           nameof(OneTimeSetUpAttribute),
                                                                           nameof(OneTimeSetUpAttribute) + "()",
                                                                           "TestFixtureSetUp", // deprecated NUnit 2.6
                                                                       };

        public static readonly IEnumerable<string> TestOneTimeTearDowns = new[]
                                                                          {
                                                                              "OneTimeTearDown",
                                                                              "OneTimeTearDown()",
                                                                              nameof(OneTimeTearDownAttribute),
                                                                              nameof(OneTimeTearDownAttribute) + "()",
                                                                              "TestFixtureTearDown", // deprecated NUnit 2.6
                                                                          };

        public static readonly IEnumerable<string> Tests = new[]
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
    }
}
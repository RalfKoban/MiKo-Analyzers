using System.Collections.Generic;

using NUnit.Framework;

//// ncrunch: rdi off
namespace TestHelper
{
    public partial class CodeFixVerifier
    {
        public static readonly IEnumerable<string> TestFixtures = new[]
                                                                      {
                                                                          "TestFixture",
                                                                          "TestFixture()",
                                                                          nameof(TestFixtureAttribute),
                                                                          "TestClassAttribute",
                                                                          "TestClass",
                                                                      };

        public static readonly IEnumerable<string> TestSetUps = new[]
                                                                    {
                                                                        "SetUp",
                                                                        "SetUp()",
                                                                        nameof(SetUpAttribute),
                                                                        "TestInitialize",
                                                                        "TestInitializeAttribute",
                                                                    };

        public static readonly IEnumerable<string> TestTearDowns = new[]
                                                                       {
                                                                           "TearDown",
                                                                           "TearDown()",
                                                                           nameof(TearDownAttribute),
                                                                           "TestCleanup",
                                                                           "TestCleanupAttribute",
                                                                       };

        public static readonly IEnumerable<string> TestOneTimeSetUps = new[]
                                                                           {
                                                                               "OneTimeSetUp",
                                                                               "OneTimeSetUp()",
                                                                               nameof(OneTimeSetUpAttribute),
                                                                               "TestFixtureSetUp", // deprecated NUnit 2.6
                                                                           };

        public static readonly IEnumerable<string> TestOneTimeTearDowns = new[]
                                                                              {
                                                                                  "OneTimeTearDown",
                                                                                  "OneTimeTearDown()",
                                                                                  nameof(OneTimeTearDownAttribute),
                                                                                  "TestFixtureTearDown", // deprecated NUnit 2.6
                                                                              };

        public static readonly IEnumerable<string> Tests = new[]
                                                               {
                                                                   "Test",
                                                                   "Test()",
                                                                   nameof(TestAttribute),
                                                                   nameof(TestCaseAttribute),
                                                                   nameof(TestCaseSourceAttribute),
                                                                   nameof(TheoryAttribute),
                                                                   "Fact",
                                                                   "TestCase",
                                                                   "TestCaseSource",
                                                                   "Theory",
                                                                   "TestMethod",
                                                                   "TestMethodAttribute",
                                                               };
    }
}
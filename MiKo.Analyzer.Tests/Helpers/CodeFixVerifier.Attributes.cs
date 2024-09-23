using System.Collections.Generic;

using NUnit.Framework;

//// ncrunch: rdi off
// ReSharper disable CheckNamespace
namespace TestHelper
{
    public partial class CodeFixVerifier
    {
        public static readonly IEnumerable<string> TestFixtures =
                                                                  [
                                                                      "TestFixture",
                                                                      "TestFixture()",
                                                                      nameof(TestFixtureAttribute),
                                                                      "TestClassAttribute",
                                                                      "TestClass",
                                                                  ];

        public static readonly IEnumerable<string> TestSetUps =
                                                                [
                                                                    "SetUp",
                                                                    "SetUp()",
                                                                    nameof(SetUpAttribute),
                                                                    "TestInitialize",
                                                                    "TestInitializeAttribute",
                                                                ];

        public static readonly IEnumerable<string> TestTearDowns =
                                                                   [
                                                                       "TearDown",
                                                                       "TearDown()",
                                                                       nameof(TearDownAttribute),
                                                                       "TestCleanup",
                                                                       "TestCleanupAttribute",
                                                                   ];

        public static readonly IEnumerable<string> TestOneTimeSetUps =
                                                                       [
                                                                           "OneTimeSetUp",
                                                                           "OneTimeSetUp()",
                                                                           nameof(OneTimeSetUpAttribute),
                                                                           "TestFixtureSetUp", // deprecated NUnit 2.6
                                                                       ];

        public static readonly IEnumerable<string> TestOneTimeTearDowns =
                                                                          [
                                                                              "OneTimeTearDown",
                                                                              "OneTimeTearDown()",
                                                                              nameof(OneTimeTearDownAttribute),
                                                                              "TestFixtureTearDown", // deprecated NUnit 2.6
                                                                          ];

        public static readonly IEnumerable<string> Tests =
                                                           [
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
                                                           ];
    }
}
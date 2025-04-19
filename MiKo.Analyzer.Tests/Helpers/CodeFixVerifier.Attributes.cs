using System.Collections.Generic;

//// ncrunch: rdi off
// ReSharper disable CheckNamespace
namespace TestHelper
{
    public partial class CodeFixVerifier
    {
        public static readonly IEnumerable<string> TestFixtures =
                                                                  [
                                                                      "TestFixture",
                                                                      //// "TestFixture()", // disabled to limit amount of tests
                                                                      //// nameof(TestFixtureAttribute), // disabled to limit amount of tests
                                                                      //// "TestClassAttribute", // disabled to limit amount of tests
                                                                      "TestClass",
                                                                  ];

        public static readonly IEnumerable<string> TestSetUps =
                                                                [
                                                                    "SetUp",
                                                                    //// "SetUp()", // disabled to limit amount of tests
                                                                    //// nameof(SetUpAttribute), // disabled to limit amount of tests
                                                                    "TestInitialize",
                                                                    //// "TestInitializeAttribute", // disabled to limit amount of tests
                                                                ];

        public static readonly IEnumerable<string> TestTearDowns =
                                                                   [
                                                                       "TearDown",
                                                                       //// "TearDown()", // disabled to limit amount of tests
                                                                       //// nameof(TearDownAttribute), // disabled to limit amount of tests
                                                                       "TestCleanup",
                                                                       //// "TestCleanupAttribute", // disabled to limit amount of tests
                                                                   ];

        public static readonly IEnumerable<string> TestOneTimeSetUps =
                                                                       [
                                                                           "OneTimeSetUp",
                                                                           //// "OneTimeSetUp()", // disabled to limit amount of tests
                                                                           //// nameof(OneTimeSetUpAttribute), // disabled to limit amount of tests
                                                                           "TestFixtureSetUp", // deprecated NUnit 2.6
                                                                       ];

        public static readonly IEnumerable<string> TestOneTimeTearDowns =
                                                                          [
                                                                              "OneTimeTearDown",
                                                                              //// "OneTimeTearDown()", // disabled to limit amount of tests
                                                                              //// nameof(OneTimeTearDownAttribute), // disabled to limit amount of tests
                                                                              "TestFixtureTearDown", // deprecated NUnit 2.6
                                                                          ];

        public static readonly IEnumerable<string> Tests =
                                                           [
                                                               "Test",
                                                               //// "Test()", // disabled to limit amount of tests
                                                               //// nameof(TestAttribute), // disabled to limit amount of tests
                                                               //// nameof(TestCaseAttribute), // disabled to limit amount of tests
                                                               //// nameof(TestCaseSourceAttribute), // disabled to limit amount of tests
                                                               //// nameof(TheoryAttribute), // disabled to limit amount of tests
                                                               "Fact",
                                                               "TestCase",
                                                               "TestCaseSource",
                                                               //// "Theory", // disabled to limit amount of tests
                                                               "TestMethod",
                                                               //// "TestMethodAttribute", // disabled to limit amount of tests
                                                           ];
    }
}
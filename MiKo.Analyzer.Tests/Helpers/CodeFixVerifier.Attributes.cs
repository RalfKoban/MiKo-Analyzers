using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace TestHelper
{
    partial class CodeFixVerifier
    {
        public static readonly IEnumerable<string> TestFixtures = new[]
                                                                      {
                                                                          nameof(TestFixtureAttribute),
                                                                          "TestFixture",
                                                                          "TestFixture()",
                                                                          "TestFixtureAttribute()",
                                                                          "TestClassAttribute",
                                                                          "TestClassAttribute()",
                                                                          "TestClass",
                                                                          "TestClass()",
                                                                      }.OrderBy(_ => _).Distinct().ToList();

        public static readonly IEnumerable<string> TestSetUps = new[]
                                                                    {
                                                                        nameof(SetUpAttribute),
                                                                        "SetUp",
                                                                        "SetUp()",
                                                                        "SetUpAttribute()",
                                                                        "TestInitialize",
                                                                        "TestInitialize()",
                                                                        "TestInitializeAttribute",
                                                                        "TestInitializeAttribute()",
                                                                    }.OrderBy(_ => _).Distinct().ToList();

        public static readonly IEnumerable<string> TestTearDowns = new[]
                                                                       {
                                                                           "TearDown",
                                                                           "TearDown()",
                                                                           nameof(TearDownAttribute),
                                                                           "TearDownAttribute()",
                                                                           "TestCleanup",
                                                                           "TestCleanup()",
                                                                           "TestCleanupAttribute",
                                                                           "TestCleanupAttribute()",
                                                                       }.OrderBy(_ => _).Distinct().ToList();

        public static readonly IEnumerable<string> TestsExceptSetUpTearDowns = new[]
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
                                                                                   }.OrderBy(_ => _).Distinct().ToList();

        public static readonly IEnumerable<string> Tests = TestsExceptSetUpTearDowns.Concat(TestSetUps).Concat(TestTearDowns).OrderBy(_ => _).Distinct().ToList();
        public static readonly IEnumerable<string> TestsExceptSetUps = Tests.Except(TestSetUps).OrderBy(_ => _).Distinct().ToList();
        public static readonly IEnumerable<string> TestsExceptTearDowns = Tests.Except(TestTearDowns).OrderBy(_ => _).Distinct().ToList();
    }
}
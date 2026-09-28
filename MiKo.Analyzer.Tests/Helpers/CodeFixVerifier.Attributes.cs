using System.Linq;

//// ncrunch: rdi off
// ReSharper disable CheckNamespace
namespace TestHelper
{
    public partial class CodeFixVerifier
    {
        public static readonly string[] ObjectUnderTestPropertyNames =
                                                                       [
                                                                           "ObjectUnderTest",
                                                                           "SubjectUnderTest",
                                                                           "UnitUnderTest",
                                                                           "Sut",
                                                                           "SuT",
                                                                           "SUT",
                                                                           "UUT",
                                                                           "UuT",
                                                                           "Uut",
                                                                           "TestCandidate",
                                                                           "TestObject",
                                                                       ];

        public static readonly string[] ObjectUnderTestFieldNames =
                                                                    [
                                                                        "ObjectUnderTest",
                                                                        "_ObjectUnderTest",
                                                                        "m_ObjectUnderTest",
                                                                        "s_ObjectUnderTest",
                                                                        "objectUnderTest",
                                                                        "_objectUnderTest",
                                                                        "m_objectUnderTest",
                                                                        "s_objectUnderTest",
                                                                        "subjectUnderTest",
                                                                        "_subjectUnderTest",
                                                                        "m_subjectUnderTest",
                                                                        "s_subjectUnderTest",
                                                                        "SubjectUnderTest",
                                                                        "_SubjectUnderTest",
                                                                        "m_SubjectUnderTest",
                                                                        "s_SubjectUnderTest",
                                                                        "unitUnderTest",
                                                                        "_unitUnderTest",
                                                                        "m_unitUnderTest",
                                                                        "s_unitUnderTest",
                                                                        "UnitUnderTest",
                                                                        "_UnitUnderTest",
                                                                        "m_UnitUnderTest",
                                                                        "s_UnitUnderTest",
                                                                        "sut",
                                                                        "_sut",
                                                                        "m_sut",
                                                                        "s_sut",
                                                                        "Sut",
                                                                        "_Sut",
                                                                        "m_Sut",
                                                                        "s_Sut",
                                                                        "uut",
                                                                        "_uut",
                                                                        "m_uut",
                                                                        "s_uut",
                                                                        "Uut",
                                                                        "_Uut",
                                                                        "m_Uut",
                                                                        "s_Uut",
                                                                        "TestCandidate",
                                                                        "testCandidate",
                                                                        "_testCandidate",
                                                                        "m_testCandidate",
                                                                        "s_testCandidate",
                                                                        "TestObject",
                                                                        "testObject",
                                                                        "_testObject",
                                                                        "m_testObject",
                                                                        "s_testObject",
                                                                    ];

        public static readonly string[] ObjectUnderTestVariableNames =
                                                                       [
                                                                           "objectUnderTest",
                                                                           "subjectUnderTest",
                                                                           "unitUnderTest",
                                                                           "testCandidate",
                                                                           "testObject",
                                                                           "sut",
                                                                           "uut",
                                                                       ];

        public static readonly string[] ObjectUnderTestNames = [.. ObjectUnderTestPropertyNames.Concat(ObjectUnderTestFieldNames).Concat(ObjectUnderTestVariableNames).Distinct()];

        public static readonly string[] TestFixtures =
                                                       [
                                                           "TestFixture",
                                                           //// "TestFixture()", // disabled to limit amount of tests
                                                           //// nameof(TestFixtureAttribute), // disabled to limit amount of tests
                                                           //// "TestClassAttribute", // disabled to limit amount of tests
                                                           "TestClass",
                                                       ];

        public static readonly string[] TestSetUps =
                                                     [
                                                         "SetUp",
                                                         //// "SetUp()", // disabled to limit amount of tests
                                                         //// nameof(SetUpAttribute), // disabled to limit amount of tests
                                                         "TestInitialize",
                                                         //// "TestInitializeAttribute", // disabled to limit amount of tests
                                                     ];

        public static readonly string[] TestTearDowns =
                                                        [
                                                            "TearDown",
                                                            //// "TearDown()", // disabled to limit amount of tests
                                                            //// nameof(TearDownAttribute), // disabled to limit amount of tests
                                                            "TestCleanup",
                                                            //// "TestCleanupAttribute", // disabled to limit amount of tests
                                                        ];

        public static readonly string[] TestOneTimeSetUps =
                                                            [
                                                                "OneTimeSetUp",
                                                                //// "OneTimeSetUp()", // disabled to limit amount of tests
                                                                //// nameof(OneTimeSetUpAttribute), // disabled to limit amount of tests
                                                                "TestFixtureSetUp", // deprecated NUnit 2.6
                                                                "ClassInitialize", // MSTest
                                                            ];

        public static readonly string[] TestOneTimeTearDowns =
                                                               [
                                                                   "OneTimeTearDown",
                                                                   //// "OneTimeTearDown()", // disabled to limit amount of tests
                                                                   //// nameof(OneTimeTearDownAttribute), // disabled to limit amount of tests
                                                                   "TestFixtureTearDown", // deprecated NUnit 2.6
                                                                   "ClassCleanup", // MSTest
                                                               ];

        public static readonly string[] TestAssemblySetUps =
                                                             [
                                                                 "AssemblyInitialize", // MSTest
                                                             ];

        public static readonly string[] TestAssemblyTearDowns =
                                                                [
                                                                    "AssemblyCleanup", // MSTest
                                                                ];

        public static readonly string[] Tests =
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

        public static readonly string[] XmlTags = ["example", "exception", "note", "overloads", "para", "param", "permission", "remarks", "returns", "summary", "typeparam", "value"];
    }
}

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] PropertyNames =
            {
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
            };

        private static readonly string[] FieldNames =
            {
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
            };

        private static readonly string[] VariableNames =
            {
                "objectUnderTest",
                "subjectUnderTest",
                "unitUnderTest",
                "testCandidate",
                "testObject",
                "sut",
                "uut",
            };

        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
namespace BlaBla
{
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_class([ValueSource(nameof(TestFixtures))] string testFixture) => No_issue_is_reported_for(@"
namespace BlaBla
{
    [" + testFixture + @"]
    public class TestMe
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_if_test_class_and_class_under_test_are_in_same_namespace(
                                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                            [ValueSource(nameof(PropertyNames))] string propertyName)
            => No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe " + propertyName + @" { get; set; }
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_field_if_test_class_and_class_under_test_are_in_same_namespace(
                                                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                                        [ValueSource(nameof(FieldNames))] string fieldName)
            => No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe " + fieldName + @";
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_localVariable_if_test_class_and_class_under_test_are_in_same_namespace(
                                                                                                                [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                                                [ValueSource(nameof(Tests))] string test,
                                                                                                                [ValueSource(nameof(VariableNames))] string variableName)
            => No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        [" + test + @"]
        public void DoSomething()
        {
            var " + variableName + @" = new TestMe();
        }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_property_if_test_class_and_class_under_test_are_in_different_namespaces(
                                                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                                            [ValueSource(nameof(PropertyNames))] string propertyName)
            => An_issue_is_reported_for(@"
namespace BlaBla
{
    public class TestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    using BlaBla;

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe " + propertyName + @" { get; set; }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_field_if_test_class_and_class_under_test_are_in_different_namespaces(
                                                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                                            [ValueSource(nameof(FieldNames))] string fieldName)
            => An_issue_is_reported_for(@"
namespace BlaBla
{
    public class TestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    using BlaBla;

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe " + fieldName + @";
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_local_variable_if_test_class_and_class_under_test_are_in_different_namespaces(
                                                                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                                                        [ValueSource(nameof(Tests))] string test,
                                                                                                                        [ValueSource(nameof(VariableNames))] string variableName)
            => An_issue_is_reported_for(@"
namespace BlaBla
{
    public class TestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    using BlaBla;

    [" + testFixture + @"]
    public class TestMeTests
    {
        [" + test + @"]
        public void DoSomething()
        {
            var " + variableName + @" = new TestMe();
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer();
    }
}

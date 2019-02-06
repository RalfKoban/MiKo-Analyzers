
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzerTests : CodeFixVerifier
    {
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

        [Test]
        public void No_issue_is_reported_if_test_class_and_class_under_test_are_in_same_namespace([ValueSource(nameof(TestFixtures))] string testFixture) => No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; set; }
    }
}
");

        [Test]
        public void An_issue_is_reported_if_test_class_and_class_under_test_are_in_different_namespaces([ValueSource(nameof(TestFixtures))] string testFixture) => An_issue_is_reported_for(@"
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
        private TestMe ObjectUnderTest { get; set; }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer();
    }
}

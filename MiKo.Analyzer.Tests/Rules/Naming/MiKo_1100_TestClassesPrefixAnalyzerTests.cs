using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1100_TestClassesPrefixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_class_without_object_under_test([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
namespace Bla
{
    [" + testFixture + @"]
    public class TestMeTests
    {
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_class_with_correct_prefix([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_class_with_wrong_prefix([ValueSource(nameof(TestFixtures))] string testFixture) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
    }

    [" + testFixture + @"]
    public class WhateverTests
    {
        private TestMe ObjectUnderTest { get; set; }

        private void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_class_of_generic_type_with_correct_prefix([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
namespace Bla
{
    public class ATestMe<T>
    {
    }

    [" + testFixture + @"]
    public class ATestMeTests<T>
    {
        private ATestMe<T> ObjectUnderTest { get; set; }

        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_class_of_generic_type_and_where_clause_with_correct_prefix([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
namespace Bla
{
    public class ATestMe
    {
    }

    [" + testFixture + @"]
    public class ATestMeTests<T> where T : ATestMe
    {
        private T ObjectUnderTest { get; set; }

        public void DoSomething() { }
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_class_of_generic_type_and_typed_where_clause_constraint_with_correct_prefix([ValueSource(nameof(TestFixtures))]string testFixture, [Values("class", "struct")] string constraint)
            => No_issue_is_reported_for(@"
namespace Bla
{
    public class ATestMe
    {
    }

    [" + testFixture + @"]
    public class ATestMeTests<T> where T : " + constraint + @"
    {
        private T ObjectUnderTest { get; set; }

        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_class_of_generic_type_with_wrong_prefix([ValueSource(nameof(TestFixtures))] string testFixture) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe<T>
    {
    }

    [" + testFixture + @"]
    public class WhateverTests<T>
    {
        private TestMe<T> ObjectUnderTest { get; set; }

        private void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_class_of_generic_type_and_where_clause_with_wrong_prefix([ValueSource(nameof(TestFixtures))]string testFixture) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
    }

    [" + testFixture + @"]
    public class WhateverTests<T> where T : TestMe
    {
        private T ObjectUnderTest { get; set; }

        public void DoSomething() { }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1100_TestClassesPrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1100_TestClassesPrefixAnalyzer();
    }
}
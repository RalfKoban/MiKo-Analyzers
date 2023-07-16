using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1100_TestClassesPrefixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] VariableNames =
                                                         {
                                                             "objectUnderTest",
                                                             "subjectUnderTest",
                                                             "unitUnderTest",
                                                             "testCandidate",
                                                             "testObject",
                                                             "sut",
                                                             "uut",
                                                             "testee",
                                                             "candidateToTest",
                                                         };

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
        public void No_issue_is_reported_for_test_class_without_object_under_test_([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_test_class_with_correct_prefix_([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_test_class_with_wrong_prefix_([ValueSource(nameof(TestFixtures))] string testFixture) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_test_class_of_generic_type_with_correct_prefix_([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_test_class_of_generic_type_and_where_clause_with_correct_prefix_([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_test_class_of_generic_type_and_typed_where_clause_constraint_with_correct_prefix() => Assert.Multiple(() =>
                                                                                                                                                        {
                                                                                                                                                            foreach (var constraint in new[] { "class", "struct" })
                                                                                                                                                            {
                                                                                                                                                                foreach (var testFixture in TestFixtures)
                                                                                                                                                                {
                                                                                                                                                                    No_issue_is_reported_for(@"
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
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                        });

        [Test]
        public void An_issue_is_reported_for_test_class_of_generic_type_with_wrong_prefix_([ValueSource(nameof(TestFixtures))] string testFixture) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_test_class_of_generic_type_and_where_clause_with_wrong_prefix_([ValueSource(nameof(TestFixtures))]string testFixture) => An_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_test_class_if_factory_method_returns_concrete_type_but_has_base_type_as_return_type_([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
namespace Bla
{
    public class BaseTestMe
    {
    }
}

namespace Bla.Blubb
{
    public class TestMe : BaseTestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private BaseTestMe CreateObjectUnderTest() => new TestMe();

        private BaseTestMe GetObjectUnderTest() => new TestMe();

        private BaseTestMe CreateTestCandidate()
        {
            return new TestMe();
        }

        private BaseTestMe GetTestCandidate()
        {
            return new TestMe();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_class_if_factory_method_returns_different_concrete_type_but_has_base_type_as_return_type_([ValueSource(nameof(TestFixtures))]string testFixture) => An_issue_is_reported_for(@"
namespace Bla
{
    public class BaseTestMe
    {
    }
}

namespace Bla.Blubb
{
    public class TestMe : BaseTestMe
    {
    }

    public class OtherTestMe : BaseTestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private BaseTestMe CreateObjectUnderTest() => new OtherTestMe();

        private BaseTestMe GetObjectUnderTest() => new OtherTestMe();

        private BaseTestMe CreateTestCandidate()
        {
            return new OtherTestMe();
        }

        private BaseTestMe GetTestCandidate()
        {
            return new OtherTestMe();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_class_if_class_under_test_as_local_variable_has_wrong_prefix() => Assert.Multiple(() =>
                                                                                                                                         {
                                                                                                                                             foreach (var testFixture in TestFixtures)
                                                                                                                                             {
                                                                                                                                                 foreach (var test in Tests)
                                                                                                                                                 {
                                                                                                                                                     foreach (var variableName in VariableNames)
                                                                                                                                                     {
                                                                                                                                                         An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
    }

    [" + testFixture + @"]
    public class WhateverTestsTests
    {
        [" + test + @"]
        public void DoSomething()
        {
            var " + variableName + @" = new TestMe();
        }
    }
}
");
                                                                                                                                                     }
                                                                                                                                                 }
                                                                                                                                             }
                                                                                                                                         });

        protected override string GetDiagnosticId() => MiKo_1100_TestClassesPrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1100_TestClassesPrefixAnalyzer();
    }
}
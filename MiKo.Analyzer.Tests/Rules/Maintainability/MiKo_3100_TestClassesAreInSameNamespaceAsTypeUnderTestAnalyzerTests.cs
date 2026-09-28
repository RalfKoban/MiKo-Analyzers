using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MethodPrefixes =
                                                          [
                                                              "Get",
                                                              "Create",
                                                          ];

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
        public void No_issue_is_reported_for_empty_test_class_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
namespace BlaBla
{
    [" + fixture + @"]
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_if_test_class_and_class_under_test_are_in_same_namespace_(
                                                                                                            [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                            [ValueSource(nameof(ObjectUnderTestPropertyNames))] string propertyName)
            => No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private TestMe " + propertyName + @" { get; set; }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_if_test_class_and_class_under_test_are_in_same_namespace_(
                                                                                                          [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                          [ValueSource(nameof(ObjectUnderTestPropertyNames))] string propertyName,
                                                                                                          [ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private TestMe " + methodPrefix + propertyName + @"() => null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_field_if_test_class_and_class_under_test_are_in_same_namespace_(
                                                                                                         [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                         [ValueSource(nameof(ObjectUnderTestFieldNames))] string fieldName)
            => No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private TestMe " + fieldName + @";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_localVariable_if_test_class_and_class_under_test_are_in_same_namespace_(
                                                                                                                 [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                 [ValueSource(nameof(Tests))] string test,
                                                                                                                 [ValueSource(nameof(ObjectUnderTestVariableNames))] string variableName)
            => No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
    {
    }

    [" + fixture + @"]
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

        [Test]
        public void No_issue_is_reported_for_method_if_test_class_and_returned_class_under_test_are_in_same_namespace_(
                                                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                   [ValueSource(nameof(ObjectUnderTestPropertyNames))] string propertyName,
                                                                                                                   [ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => No_issue_is_reported_for(@"
namespace BlaBla
{
    public class BaseTestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    public class TestMe : BaseTestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private BaseTestMe " + methodPrefix + propertyName + @"() => new TestMe();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_if_variable_that_is_test_class_and_returned_class_under_test_are_in_same_namespace_(
                                                                                                                                    [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                    [ValueSource(nameof(ObjectUnderTestPropertyNames))] string propertyName,
                                                                                                                                    [ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => No_issue_is_reported_for(@"
namespace BlaBla
{
    public class BaseTestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    public class TestMe : BaseTestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private BaseTestMe " + methodPrefix + propertyName + @"()
        {
            BaseTestMe me = new TestMe();
            return me;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_expression_body_test_method_(
                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                      [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
public static class TestMe
{
    public static TestMe Parse(string s) => null;
}

[" + fixture + @"]
public class TestMeTests
{
    [" + test + @"]
    public static string Parse_parses_text() => TestMe.Parse(123.ToString());
}
");

        [Test]
        public void An_issue_is_reported_for_method_if_test_class_and_class_under_test_are_in_different_namespaces_(
                                                                                                                [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                [ValueSource(nameof(ObjectUnderTestPropertyNames))] string propertyName,
                                                                                                                [ValueSource(nameof(MethodPrefixes))] string methodPrefix)
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

    [" + fixture + @"]
    public class TestMeTests
    {
        private TestMe " + methodPrefix + propertyName + @"() => null;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_if_test_class_and_class_under_test_are_in_different_namespaces_(
                                                                                                                  [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                  [ValueSource(nameof(ObjectUnderTestPropertyNames))] string propertyName)
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

    [" + fixture + @"]
    public class TestMeTests
    {
        private TestMe " + propertyName + @" { get; set; }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_field_if_test_class_and_class_under_test_are_in_different_namespaces_(
                                                                                                               [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                               [ValueSource(nameof(ObjectUnderTestFieldNames))] string fieldName)
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

    [" + fixture + @"]
    public class TestMeTests
    {
        private TestMe " + fieldName + @";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_variable_if_test_class_and_class_under_test_are_in_different_namespaces_(
                                                                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                        [ValueSource(nameof(Tests))] string test,
                                                                                                                        [ValueSource(nameof(ObjectUnderTestVariableNames))] string variableName)
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

    [" + fixture + @"]
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

        [Test]
        public void An_issue_is_reported_for_method_if_test_class_and_returned_class_under_test_are_in_different_namespace_(
                                                                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                        [ValueSource(nameof(ObjectUnderTestPropertyNames))] string propertyName,
                                                                                                                        [ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => An_issue_is_reported_for(@"
namespace BlaBla
{
    public class BaseTestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    public class TestMe : BaseTestMe
    {
    }

}

namespace BlaBla.BlaBlubb.Tests
{
    [" + fixture + @"]
    public class TestMeTests
    {
        private BaseTestMe " + methodPrefix + propertyName + @"() => new TestMe();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_if_variable_that_is_test_class_and_returned_class_under_test_are_in_different_namespace_(
                                                                                                                                         [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                         [ValueSource(nameof(ObjectUnderTestPropertyNames))] string propertyName,
                                                                                                                                         [ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => An_issue_is_reported_for(@"
namespace BlaBla
{
    public class BaseTestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    public class TestMe : BaseTestMe
    {
    }

}

namespace BlaBla.BlaBlubb.Tests
{
    [" + fixture + @"]
    public class TestMeTests
    {
        private BaseTestMe " + methodPrefix + propertyName + @"()
        {
            BaseTestMe me = new TestMe();
            return me;
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzer();
    }
}

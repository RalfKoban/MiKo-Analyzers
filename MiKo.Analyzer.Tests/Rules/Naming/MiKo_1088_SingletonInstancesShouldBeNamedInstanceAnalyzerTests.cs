using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1088_SingletonInstancesShouldBeNamedInstanceAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] FieldPrefixes = { "m_", "s_", "t_", "_", string.Empty, };

        [Test]
        public void No_issue_is_reported_for_test_class() => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_struct() => No_issue_is_reported_for(@"

public struct TestMe
{
    public static TestMe Something { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_enum_class() => No_issue_is_reported_for(@"

public enum TestMe
{
    Nothing = 0,
    Something = 1,
}
");

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_type_without_properties_or_fields() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_only_non_static_properties_and_fields() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool SomeProperty { get; set; }

    private int _someField;
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_static_property_that_returns_some_other_type() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool SomeProperty { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_static_field_that_is_of_some_other_type() => No_issue_is_reported_for(@"
public class TestMe
{
    private static int _someField;
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_correctly_named_static_singleton_property_([Values("Instance", "Empty", "Default", "Zero")] string propertyName) => No_issue_is_reported_for(@"
public class TestMe
{
    public static TestMe " + propertyName + @" { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_correctly_named_static_singleton_field_(
                                                                                           [ValueSource(nameof(FieldPrefixes))] string prefix,
                                                                                           [Values("instance", "empty", "default", "zero", "Instance", "Empty", "Default", "Zero")] string fieldName)
            => No_issue_is_reported_for(@"
public class TestMe
{
    public static TestMe " + prefix + fieldName + @";
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_incorrectly_named_static_singleton_property() => An_issue_is_reported_for(@"
public class TestMe
{
    public static TestMe Singleton { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_incorrectly_named_static_singleton_field_([ValueSource(nameof(FieldPrefixes))] string prefix) => An_issue_is_reported_for(@"
public class TestMe
{
    public static TestMe " + prefix + @"singleton;
}
");

        protected override string GetDiagnosticId() => MiKo_1088_SingletonInstancesShouldBeNamedInstanceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1088_SingletonInstancesShouldBeNamedInstanceAnalyzer();
    }
}
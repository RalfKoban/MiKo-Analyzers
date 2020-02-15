using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1093_ObjectSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_wrong_name() => An_issue_is_reported_for(@"

public class MyObject
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_property() => No_issue_is_reported_for(@"

public class TestMe
{
    public int SomeProperty { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_wrong_name() => An_issue_is_reported_for(@"

public class TestMe
{
    public int SomePropertyObject { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_field() => No_issue_is_reported_for(@"

public class TestMe
{
    private int m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_wrong_name() => An_issue_is_reported_for(@"

public class TestMe
{
    private int m_fieldObject;
}
");

        protected override string GetDiagnosticId() => MiKo_1093_ObjectSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1093_ObjectSuffixAnalyzer();
    }
}
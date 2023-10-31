using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
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

        [TestCase("class TestMeObject { }", "class TestMe { }")]
        [TestCase("class TestMeStruct { }", "class TestMe { }")]
        [TestCase("interface TestMeObject { }", "interface TestMe { }")]
        [TestCase("interface TestMeStruct { }", "interface TestMe { }")]
        [TestCase("class TestMe { int m_fieldObject; }", "class TestMe { int m_field; }")]
        [TestCase("class TestMe { int m_fieldStruct; }", "class TestMe { int m_field; }")]
        [TestCase("class TestMe { int SomePropertyObject { get; set; } }", "class TestMe { int SomeProperty { get; set; } }")]
        [TestCase("class TestMe { int SomePropertyStruct { get; set; } }", "class TestMe { int SomeProperty { get; set; } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1093_ObjectSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1093_ObjectSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1093_CodeFixProvider();
    }
}
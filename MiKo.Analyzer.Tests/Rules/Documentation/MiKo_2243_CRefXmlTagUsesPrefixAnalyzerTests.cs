using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2243_CRefXmlTagUsesPrefixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_type() => No_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""TestMe""/>.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_accessor() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.get_SomeProperty""/>.
    /// </summary>
    public int SomeProperty { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_constructor() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.TestMe""/>.
    /// </summary>
    public TestMe()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_enum_member() => No_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""TestMe.Some""/>.
/// </summary>
public enum TestMe
{
    Some
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_event() => No_issue_is_reported_for(@"
using System;

public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.SomeEvent""/>.
    /// </summary>
    public event EventHandler SomeEvent;
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_field() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.m_field""/>.
    /// </summary>
    private int m_field;
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_method() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.DoSomething""/>.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_namespace() => No_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""System""/>.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_operator() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.op_Addition""/>.
    /// </summary>
    public static TestMe operator +(TestMe a, TestMe b) => a;
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_property() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.SomeProperty""/>.
    /// </summary>
    public int SomeProperty { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_type_as_delegate() => No_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""TestMe""/>.
/// </summary>
public delegate void TestMe();
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_type_as_enum() => No_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""TestMe""/>.
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_type_as_interface() => No_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""ITestMe""/>.
/// </summary>
public interface ITestMe
{
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_type_as_record() => No_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""TestMe""/>.
/// </summary>
public sealed record TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_on_documentation_with_XML_cref_without_prefix_for_type_as_struct() => No_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""TestMe""/>.
/// </summary>
public struct TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_type() => An_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""T:TestMe""/>.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_accessor() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""M:TestMe.get_SomeProperty""/>.
    /// </summary>
    public int SomeProperty { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_constructor() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""M:TestMe.TestMe""/>.
    /// </summary>
    public TestMe()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_enum_member() => An_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""F:TestMe.Some""/>.
/// </summary>
public enum TestMe
{
    Some
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_event() => An_issue_is_reported_for(@"
using System;

public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""E:TestMe.SomeEvent""/>.
    /// </summary>
    public event EventHandler SomeEvent;
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_field() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""F:TestMe.m_field""/>.
    /// </summary>
    private int m_field;
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_method() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""M:TestMe.DoSomething""/>.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_namespace() => An_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""N:System""/>.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_operator() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""M:TestMe.op_Addition""/>.
    /// </summary>
    public static TestMe operator +(TestMe a, TestMe b) => a;
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_property() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""P:TestMe.SomeProperty""/>.
    /// </summary>
    public int SomeProperty { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_type_as_delegate() => An_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""T:TestMe""/>.
/// </summary>
public delegate void TestMe();
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_type_as_enum() => An_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""T:TestMe""/>.
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_type_as_interface() => An_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""T:ITestMe""/>.
/// </summary>
public interface ITestMe
{
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_type_as_record() => An_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""T:TestMe""/>.
/// </summary>
public sealed record TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_on_documentation_with_XML_cref_with_prefix_for_type_as_struct() => An_issue_is_reported_for(@"
/// <summary>
/// Does something for <see cref=""T:TestMe""/>.
/// </summary>
public struct TestMe
{
}
");

        [Test]
        public void Code_gets_fixed_for_documentation_with_XML_cref_with_prefix_for_type()
        {
            const string OriginalCode = @"
/// <summary>
/// Does something for <see cref=""T:TestMe""/>.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something for <see cref=""TestMe""/>.
/// </summary>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_XML_cref_with_prefix_for_event()
        {
            const string OriginalCode = @"
using System;

public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""E:TestMe.SomeEvent""/>.
    /// </summary>
    public event EventHandler SomeEvent;
}
";

            const string FixedCode = @"
using System;

public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.SomeEvent""/>.
    /// </summary>
    public event EventHandler SomeEvent;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_XML_cref_with_prefix_for_field()
        {
            const string OriginalCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""F:TestMe.m_field""/>.
    /// </summary>
    private int m_field;
}
";

            const string FixedCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.m_field""/>.
    /// </summary>
    private int m_field;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_XML_cref_with_prefix_for_method()
        {
            const string OriginalCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""M:TestMe.DoSomething""/>.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.DoSomething""/>.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_XML_cref_with_prefix_for_namespace()
        {
            const string OriginalCode = @"
/// <summary>
/// Does something for <see cref=""N:System""/>.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something for <see cref=""System""/>.
/// </summary>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_with_XML_cref_with_prefix_for_property()
        {
            const string OriginalCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""P:TestMe.SomeProperty""/>.
    /// </summary>
    public int SomeProperty { get; set; }
}
";

            const string FixedCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something for <see cref=""TestMe.SomeProperty""/>.
    /// </summary>
    public int SomeProperty { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2243_CRefXmlTagUsesPrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2243_CRefXmlTagUsesPrefixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2243_CodeFixProvider();
    }
}
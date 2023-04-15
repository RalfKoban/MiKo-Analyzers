using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2000_MalformedDocumentationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correct_XML_on_class() => No_issue_is_reported_for(@"
/// <summary>
/// Something valid.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correct_XML_on_class_with_escaped_XML_entities() => No_issue_is_reported_for(@"
/// <summary>
/// Something &amp; valid.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correct_XML_on_class_inside_code_tag() => No_issue_is_reported_for(@"
/// <summary>
/// Something valid.
/// <code>
/// Test &amp; other stuff.
/// </code>
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correct_XML_on_method() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    public void Valid() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_XML_on_property() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    public int Valid { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_XML_on_event() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    public event EventHandler Valid;
}
");

        [Test]
        public void No_issue_is_reported_for_correct_XML_on_field() => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    private string Valid;
}
");

        [Test]
        public void An_issue_is_reported_for_malformed_XML_on_class() => An_issue_is_reported_for(@"
/// <summary>
/// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_malformed_XML_on_method() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    public void Malform() { }
}
");

        [Test]
        public void An_issue_is_reported_for_malformed_XML_on_property() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    public int Malform { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_malformed_XML_on_event() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    public event EventHandler Malform;
}
");

        [Test]
        public void An_issue_is_reported_for_malformed_XML_on_field() => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    private string Malform;
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
/// <summary>
/// Saves & loads something on <see cref=""TestMe""/>.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Saves &amp; loads something on <see cref=""TestMe""/>.
/// </summary>
public sealed class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2000_MalformedDocumentationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2000_MalformedDocumentationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2000_CodeFixProvider();
    }
}
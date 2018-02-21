using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2020_InheritdocSummaryAnalyzerTests : CodeFixVerifier
    {
        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("enum")]
        public void An_issue_is_reported_for_XML_summary_of_named_type(string type) => An_issue_is_reported_for(@"

/// <summary>
/// <see cref='bla' />
/// </summary>
public " + type + @" TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_method() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <see cref='bla' />
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_property() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <see cref='bla' />
    /// </summary>
    public int DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_field() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <see cref='bla' />
    /// </summary>
    private int doSomething;
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_event() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <see cref='bla' />
    /// </summary>
    public event EventHandler MyEvent;
}
");

        protected override string GetDiagnosticId() => MiKo_2020_InheritdocSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2020_InheritdocSummaryAnalyzer();
    }
}
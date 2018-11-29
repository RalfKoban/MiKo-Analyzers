using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2005_EventArgsInDocumentationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_commented_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public event EventHandler MyEvent;

    public int SomeProperty { get; set; }

    public void DoSomething() { }

    private int SomeField;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_class() => No_issue_is_reported_for(@"
/// <summary>
/// This is a comment.
/// </summary>
public class TestMe
{
    /// <summary>
    /// This is a comment.
    /// </summary>
    public event EventHandler MyEvent;

    /// <summary>
    /// This is a comment.
    /// </summary>
    public int SomeProperty { get; set; }

    /// <summary>
    /// This is a comment.
    /// </summary>
    public void DoSomething() { }

    /// <summary>
    /// This is a comment.
    /// </summary>
    private int SomeField;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_class() => An_issue_is_reported_for(@"
/// <summary>
/// This is a event arg comment.
/// </summary>
public class TestMe
{
}
");
        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_event() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a event arg comment.
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_property() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a event arg comment.
    /// </summary>
    public int SomeProperty { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_method() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a event arg comment.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_field() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a event arg comment.
    /// </summary>
    private int SomeField;
}
");

        protected override string GetDiagnosticId() => MiKo_2005_EventArgsInDocumentationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2005_EventArgsInDocumentationAnalyzer();
    }
}
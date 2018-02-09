using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2001_EventAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_commented_event_on_class() => No_issue_is_reported(@"
public class TestMe
{
    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_non_commented_event_on_interface() => No_issue_is_reported(@"
public interface TestMe
{
    event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_event_on_class() => No_issue_is_reported(@"
public class TestMe
{
    /// <summary>
    /// Occurs always.
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [TestCase("Occur")]
        [TestCase("The")]
        [TestCase("Whatever that comment means")]
        public void An_issue_is_reported_for_wrong_comment(string comment) => Issue_is_reported(@"
public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_empty_comment() => Issue_is_reported(@"
public class TestMe
{
    /// <summary>
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_inherited_comment() => No_issue_is_reported(@"
public class TestMe
{
    /// <inheritdoc />
    public event EventHandler MyEvent;
}
");

        protected override string GetDiagnosticId() => MiKo_2001_EventAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2001_EventAnalyzer();
    }
}
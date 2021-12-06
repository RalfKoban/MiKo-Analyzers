using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2215_SentencesShallBeShortAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_containing_short_sentences() => No_issue_is_reported_for(@"
/// <summary>
/// This is a short sentence. And yet another one. As well as this one. Maybe not the best but still sufficient.
/// </summary>
/// <remarks>
/// Believe it or not but it works.
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_containing_medium_to_almost_long_sentences() => No_issue_is_reported_for(@"
/// <summary>
/// Specifies that the test fixture(s) marked with this attribute are considered to be <i>atomic</i> by NCrunch, meaning that their child tests cannot
/// be run separately from each other.
/// </summary>
/// <remarks>
/// A test being queued for execution under an atomic fixture will result in the entire fixture being queued with its child tests all executed in the
/// same task/batch.
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_summary_containing_long_sentences() => An_issue_is_reported_for(@"
/// <summary>
/// This is a very long sentence that is hard to understand and difficult to read,
/// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_enum_member_summary_containing_long_sentences() => An_issue_is_reported_for(@"
public enum TestMe
{
    /// <summary>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </summary>
    None,
}
");

        [Test]
        public void An_issue_is_reported_for_event_field_summary_containing_long_sentences() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </summary>
    event EventHandler MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_event_declaration_summary_containing_long_sentences() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </summary>
    event EventHandler MyEvent
    {
        add { }
        remove { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_field_summary_containing_long_sentences() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// This is a very long sentence that is hard to understand and difficult to read,
    /// thus leading to the situation that the reader gets bored out due to the sheer amount of words to keep in mind.
    /// </summary>
    private int m_field;
}
");

        protected override string GetDiagnosticId() => MiKo_2215_SentencesShallBeShortAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2215_SentencesShallBeShortAnalyzer();
    }
}
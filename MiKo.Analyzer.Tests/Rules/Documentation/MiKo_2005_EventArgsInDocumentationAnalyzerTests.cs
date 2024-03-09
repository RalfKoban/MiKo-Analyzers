using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2005_EventArgsInDocumentationAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] IncorrectPhrases =
                                                            {
                                                                "This is an event arg comment.",
                                                                "This is an event args comment.",
                                                            };

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
        public void No_issue_is_reported_for_class_commented_with_event_argument() => No_issue_is_reported_for(@"
/// <summary>
/// This is an event argument comment.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_class_([ValueSource(nameof(IncorrectPhrases))] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// " + phrase + @"
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_event_([ValueSource(nameof(IncorrectPhrases))] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_property_([ValueSource(nameof(IncorrectPhrases))] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"
    /// </summary>
    public int SomeProperty { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_method_([ValueSource(nameof(IncorrectPhrases))] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_field_([ValueSource(nameof(IncorrectPhrases))] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"
    /// </summary>
    private int SomeField;
}
");

        protected override string GetDiagnosticId() => MiKo_2005_EventArgsInDocumentationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2005_EventArgsInDocumentationAnalyzer();
    }
}
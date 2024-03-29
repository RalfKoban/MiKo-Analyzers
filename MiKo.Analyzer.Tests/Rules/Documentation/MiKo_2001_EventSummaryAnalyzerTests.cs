﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2001_EventSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_commented_event_on_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_non_commented_event_on_interface() => No_issue_is_reported_for(@"
public interface TestMe
{
    event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_event_on_class() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Occurs always.
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_event_on_class_with_para_tags() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <para>
    /// Occurs always.
    /// </para>
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [TestCase("Occur")]
        [TestCase("The")]
        [TestCase("Whatever that comment means")]
        [TestCase("Invoked if the something changed")]
        [TestCase("Raised at some time")]
        public void An_issue_is_reported_for_wrong_comment_(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_empty_comment() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_inherited_comment() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <inheritdoc />
    public event EventHandler MyEvent;
}
");

        [TestCase("Event fired after", "Occurs after")]
        [TestCase("Event raised after", "Occurs after")]
        [TestCase("Event is fired after", "Occurs after")]
        [TestCase("Event is fired before", "Occurs before")]
        [TestCase("Event is fired when", "Occurs when")]
        [TestCase("Event is raised when", "Occurs when")]
        [TestCase("Event occurs when", "Occurs when")]
        [TestCase("Event shall be fired when", "Occurs when")]
        [TestCase("Event shall be raised when", "Occurs when")]
        [TestCase("Event that is fired when", "Occurs when")]
        [TestCase("Event that is raised when", "Occurs when")]
        [TestCase("Event that shall be fired when", "Occurs when")]
        [TestCase("Event that shall be raised when", "Occurs when")]
        [TestCase("Event to fire when", "Occurs when")]
        [TestCase("Event to raise when", "Occurs when")]
        [TestCase("Event which is fired when", "Occurs when")]
        [TestCase("Event which is raised when", "Occurs when")]
        [TestCase("Fired after", "Occurs after")]
        [TestCase("Fired before", "Occurs before")]
        [TestCase("Fired when", "Occurs when")]
        [TestCase("Indicates that", "Occurs when")]
        [TestCase("Is fired after", "Occurs after")]
        [TestCase("Is fired before", "Occurs before")]
        [TestCase("Is fired when", "Occurs when")]
        [TestCase("Is raised when", "Occurs when")]
        [TestCase("Raised after", "Occurs after")]
        [TestCase("Raised before", "Occurs before")]
        [TestCase("Raised if", "Occurs if")]
        [TestCase("Raised when", "Occurs when")]
        [TestCase("Shall be fired when", "Occurs when")]
        [TestCase("Shall be raised when", "Occurs when")]
        [TestCase("The event to fire when", "Occurs when")]
        [TestCase("The event to raise when", "Occurs when")]
        [TestCase("This event is fired for", "Occurs for")]
        [TestCase("This event is raised for", "Occurs for")]
        [TestCase("This event occurs when", "Occurs when")]
        [TestCase("This event shall be fired when", "Occurs when")]
        [TestCase("This event shall be raised when", "Occurs when")]
        [TestCase("When", "Occurs when")]
        [TestCase("Invoked if", "Occurs when")]
        public void Code_gets_fixed_(string originalComment, string fixedComment)
        {
            const string Template = @"
public class TestMe
{
    /// <summary>
    /// ### something.
    /// </summary>
    public event EventHandler MyEvent;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        protected override string GetDiagnosticId() => MiKo_2001_EventSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2001_EventSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2001_CodeFixProvider();
    }
}
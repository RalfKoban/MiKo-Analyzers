using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2003_EventHandlerSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_event_handling_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_event_handling_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object sender, MyEventArgs e) { }
}
");

        [Test]
        public void No_issue_is_reported_for_inherited_documented_event_handling_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <inheritdoc/>
    public void DoSomething(object sender, MyEventArgs e) { }
}
");

        [TestCase("Handles the <see cref='MyEvent' /> event.")]
        [TestCase("<para>Handles the <see cref='MyEvent' /> event.</para>")]
        public void No_issue_is_reported_for_correctly_documented_event_handling_method_(string comment) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class MyEventArgs : System.EventArgs { }

    public class TestMe
    {
        /// <summary>
        /// " + comment + @" 
        /// </summary>
        public void DoSomething(object sender, MyEventArgs e) { }
    }
}");

        [TestCase("Handle the <see cref='MyEvent' /> event.")]
        [TestCase("<para>Handle the <see cref='MyEvent' /> event.</para>")]
        [TestCase("Called by the <see cref='MyEvent' /> event.")]
        [TestCase("<para>Called by the <see cref='MyEvent' /> event.</para>")]
        [TestCase("Callback that is called by the <see cref='MyEvent' /> event.")]
        [TestCase("Handler for the <see cref='MyEvent' /> event.")]
        [TestCase("EventHandler for the <see cref='MyEvent' /> event.")]
        [TestCase("When the <see cref='MyEvent' /> event.")]
        [TestCase("when the <see cref='MyEvent' /> event.")]
        [TestCase("Raised when the <see cref='MyEvent' /> event.")]
        public void An_issue_is_reported_for_incorrectly_documented_event_handling_method_(string comment) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class MyEventArgs : System.EventArgs { }

    public class TestMe
    {
        /// <summary>
        /// " + comment + @" 
        /// </summary>
        public void DoSomething(object sender, MyEventArgs e) { }
    }
}");

        [TestCase("Handler for event", "Handles the event")]
        [TestCase("Handler for the event", "Handles the event")]
        [TestCase(@"Handler for <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Handler for the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase("EventHandler for event", "Handles the event")]
        [TestCase("EventHandler for the event", "Handles the event")]
        [TestCase(@"EventHandler for <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"EventHandler for the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase("Event handler for event", "Handles the event")]
        [TestCase("Event handler for the event", "Handles the event")]
        [TestCase(@"Event handler for <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Event handler for the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Called by the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Called if the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Called if <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Called when the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Called when <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Callback that is called by the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"When the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"when the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Raised when the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        public void Code_gets_fixed_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;

public class TestMe
{
    /// <summary>
    /// ###
    /// </summary>
    void OnSomething(object sender, EventArgs e);
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        protected override string GetDiagnosticId() => MiKo_2003_EventHandlerSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2003_EventHandlerSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2003_CodeFixProvider();
    }
}
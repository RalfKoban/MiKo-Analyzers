using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2002_EventHandlingMethodParametersAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_correctly_documented_event_handling_method() => No_issue_is_reported_for(@"
public class MyEventArgs : System.EventArgs { }

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name='sender'>The source of the event.</param>
    /// <param name='e'>A <see cref='MyEventArgs' /> that contains the event data.</param>
    public void DoSomething(object sender, MyEventArgs e) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_event_handling_method_with_para_tags() => No_issue_is_reported_for(@"
public class MyEventArgs : System.EventArgs { }

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name='sender'><para>The source of the event.</para></param>
    /// <param name='e'><para>A <see cref='MyEventArgs' /> that contains the event data.</para></param>
    public void DoSomething(object sender, MyEventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_partly_documented_event_handling_method_with_missing_docu_for_sender() => An_issue_is_reported_for(@"
public class MyEventArgs : System.EventArgs { }

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name='e'>A <see cref='MyEventArgs' /> that contains the event data.</param>
    public void DoSomething(object sender, MyEventArgs e) { }
}
");

        protected override string GetDiagnosticId() => MiKo_2002_EventHandlingMethodParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2002_EventHandlingMethodParametersAnalyzer();
    }
}
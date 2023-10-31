using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1009_EventHandlerLocalVariableAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_non_EventHandler_variable() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_EventHandler_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        EventHandler handler = null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_generic_EventHandler_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        EventHandler<EventArgs> handler = null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_var_EventHandler_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var handler = OnHandleEvent;
    }

    private void OnHandleEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_EventHandler_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        EventHandler x = null;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_EventHandler_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = new EventHandler<EventArgs>(OnHandleEvent);
    }

    private void OnHandleEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_PropertyChangingEventHandler_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething()
    {
        var x = new PropertyChangingEventHandler(OnHandleEvent);
    }

    private void OnHandleEvent(object sender, PropertyChangingEventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_PropertyChangedEventHandler_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething()
    {
        var x = new PropertyChangedEventHandler(OnHandleEvent);
    }

    private void OnHandleEvent(object sender, PropertyChangedEventArgs e)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed() => VerifyCSharpFix(
                                                         "using System; class TestMe { void DoSomething() { EventHandler x = null; } }",
                                                         "using System; class TestMe { void DoSomething() { EventHandler handler = null; } }");

        protected override string GetDiagnosticId() => MiKo_1009_EventHandlerLocalVariableAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1009_EventHandlerLocalVariableAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1009_CodeFixProvider();
    }
}
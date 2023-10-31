using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1005_EventArgsLocalVariableAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_with_non_EventArgs_variable() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_method_with_EventArgs_variable_with_correct_name_([Values("e", "args")] string variableName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        EventArgs " + variableName + @" = EventArgs.Empty;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_var_EventArgs_variable_with_correct_name_([Values("e", "args")] string variableName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var " + variableName + @" = EventArgs.Empty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_EventArgs_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        EventArgs eventArgs = EventArgs.Empty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_EventArgs_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var eventArgs = EventArgs.Empty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_inherited_EventArgs_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public void DoSomething()
    {
        MyEventArgs eventArgs = new MyEventArgs();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_inherited_EventArgs_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public void DoSomething()
    {
        var eventArgs = new MyEventArgs();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_incorrect_name() => No_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    private MyEventArgs _eventArgs;

    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_variable_declaration_pattern_for_EventArgs_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case MyEventArgs e: return;
            default: return;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_variable_declaration_pattern_for_EventArgs_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case MyEventArgs eventArgs: return;
            default: return;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_inherited_EventArgs_variable_with_incorrect_name_and_Parameter_named_e() => An_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public void DoSomething(MyEventArgs e)
    {
        var eventArgs = new MyEventArgs();
    }
}
");

        [Test]
        public void Code_gets_fixed() => VerifyCSharpFix(
                                                         "using System; class TestMe { void DoSomething() { var eventArgs = new EventArgs(); } }",
                                                         "using System; class TestMe { void DoSomething() { var e = new EventArgs(); } }");

        [Test]
        public void Code_gets_fixed_with_parameter() => VerifyCSharpFix(
                                                         "using System; class TestMe { void DoSomething(EventArgs e) { var eventArgs = new EventArgs(); } }",
                                                         "using System; class TestMe { void DoSomething(EventArgs e) { var args = new EventArgs(); } }");

        protected override string GetDiagnosticId() => MiKo_1005_EventArgsLocalVariableAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1005_EventArgsLocalVariableAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1005_CodeFixProvider();
    }
}
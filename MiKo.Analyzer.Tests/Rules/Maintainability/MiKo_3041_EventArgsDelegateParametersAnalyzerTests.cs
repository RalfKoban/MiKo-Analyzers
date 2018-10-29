using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3041_EventArgsDelegateParametersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_EventArgs_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe(Action action)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_EventArgs_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public Action DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_non_EventArgs_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Action action)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_EventArgs_ctor() => An_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs
{
    public TestMeEventArgs(Action action)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_EventArgs_property_getter() => An_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs
{
    public Action DoSomething { get; }
}
");
        [Test]
        public void An_issue_is_reported_for_EventArgs_property_setter() => An_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs
{
    public Action DoSomething { set; }
}
");

        [Test]
        public void An_issue_is_reported_for_EventArgs_method() => An_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs
{
    public void DoSomething(Action action)
    { }
}
");

        protected override string GetDiagnosticId() => MiKo_3041_EventArgsDelegateParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3041_EventArgsDelegateParametersAnalyzer();
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1049_ShouldAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_symbols() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool m_something;

    public event EventHandler SomethingEvent;

    public bool Something { get; set;}

    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_type() => An_issue_is_reported_for(@"
using System;

public class ShouldTestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_field() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool m_somethingShould;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_event() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler SomethingShouldEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool ShouldSomething { get; set;}
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void ShouldDoSomething() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1049_ShouldAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1049_ShouldAnalyzer();
    }
}
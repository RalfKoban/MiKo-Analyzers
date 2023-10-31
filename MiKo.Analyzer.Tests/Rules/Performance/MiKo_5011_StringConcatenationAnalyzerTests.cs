using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5011_StringConcatenationAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_addition_of_ints() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething()
    {
        int i = 0;
        i += 5;
        return i;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_addition_of_event() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        MyEvent += Handle;
    }

    private void Handle(object sender, EventArgs e) {}
}
");

        [Test]
        public void An_issue_is_reported_for_addition_of_strings() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string DoSomething()
    {
        string s = string.Empty;
        s += ""bla"";
        return s;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_5011_StringConcatenationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5011_StringConcatenationAnalyzer();
    }
}
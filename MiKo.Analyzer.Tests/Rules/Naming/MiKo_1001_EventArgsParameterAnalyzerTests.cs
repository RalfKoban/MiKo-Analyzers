using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1001_EventArgsParameterAnalyzerTests : CodeFixVerifier
    {
        [TestCase("object s, EventArgs args")]
        public void No_issue_is_reported_for_eventhandling_method(string parameters) => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        [TestCase("")]
        [TestCase("int args")]
        [TestCase("object s")]
        [TestCase("object s, int args, object whatever")]
        [TestCase("object whatever, object s, int args")]
        public void No_issue_is_reported_for_non_matching_method(string parameters) => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        [TestCase("EventArgs args")]
        [TestCase("EventArgs args, object s")]
        [TestCase("EventArgs e, EventArgs a")]
        [TestCase("object s, EventArgs args, object whatever")]
        [TestCase("object whatever, object s, EventArgs args")]
        public void An_issue_is_reported_for_matching_method(string parameters) => An_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        [TestCase("EventArgs e")]
        [TestCase("EventArgs e, string a")]
        [TestCase("EventArgs e1, EventArgs e2")]
        public void No_issue_is_reported_for_correctly_named_method(string parameters) => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        protected override string GetDiagnosticId() => MiKo_1001_EventArgsParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1001_EventArgsParameterAnalyzer();
    }
}
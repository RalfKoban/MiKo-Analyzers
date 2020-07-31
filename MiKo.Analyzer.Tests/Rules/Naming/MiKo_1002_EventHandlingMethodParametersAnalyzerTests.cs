using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1002_EventHandlingMethodParametersAnalyzerTests : CodeFixVerifier
    {
        [TestCase("")]
        [TestCase("EventArgs args")]
        [TestCase("object s")]
        [TestCase("EventArgs args, object s")]
        [TestCase("object s, EventArgs args, object whatever")]
        [TestCase("object whatever, object s, EventArgs args")]
        public void No_issue_is_reported_for_non_event_handling_method(string parameters) => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method() => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void OnWhatever(object sender, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_sender() => An_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void OnWhatever(object s, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_sender_on_overridden_method() => An_issue_is_reported_for(@"

using System;

public class TestMe
{
    public override void OnWhatever(object s, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_EventArgs() => An_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void OnWhatever(object sender, EventArgs args) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_EventArgs_on_overridden_method() => An_issue_is_reported_for(@"

using System;

public class TestMe
{
    public override void OnWhatever(object sender, EventArgs args) { }
}
");

        [Test]
        public void Fix_can_be_made() => VerifyCSharpFix(
            @"class TestMe { void OnWhatever(object s, EventArgs args)  { System.Diagnostics.Trace.Write(args.GetType().ToString() + s.ToString(); } }",
            @"class TestMe { void OnWhatever(object sender, EventArgs e)  { System.Diagnostics.Trace.Write(e.GetType().ToString() + sender.ToString(); } }");

        protected override string GetDiagnosticId() => MiKo_1002_EventHandlingMethodParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1002_EventHandlingMethodParametersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1002_EventHandlingMethodParametersCodeFixProvider();
    }
}
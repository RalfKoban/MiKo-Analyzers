using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1004_EventNameSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_event() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyBlaBla;
}
");

        [TestCase("interface", "MyEvent")]
        [TestCase("class", "MyEvent")]
        public void An_issue_is_reported_for_incorrectly_named_event(string type, string eventName) => An_issue_is_reported_for(@"
using System;

public " + type + @" TestMe
{
    public event EventHandler " + eventName + @";
}
");

        protected override string GetDiagnosticId() => MiKo_1004_EventNameSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1004_EventNameSuffixAnalyzer();
    }
}
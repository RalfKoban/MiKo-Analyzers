using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1013_NotifyMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("DoSomething")]
        [TestCase("Raise")]
        [TestCase("OnSomething")]
        public void No_issue_is_reported_for_correctly_named_method(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("Notify")]
        [TestCase("NotifySomething")]
        [TestCase("OnNotify")]
        [TestCase("OnNotifySomething")]
        public void An_issue_is_reported_for_wrong_named_method(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1013_NotifyMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1013_NotifyMethodsAnalyzer();
    }
}
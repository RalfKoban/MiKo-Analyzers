using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1013_NotifyMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("DoSomething")]
        [TestCase("Raise")]
        [TestCase("OnSomething")]
        [TestCase("Notify")]
        public void No_issue_is_reported_for_correctly_named_method_(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("DoSomething")]
        [TestCase("Raise")]
        [TestCase("OnSomething")]
        [TestCase("Notify")]
        public void No_issue_is_reported_for_correctly_named_local_function_(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [TestCase("NotifySomething")]
        [TestCase("OnNotify")]
        [TestCase("OnNotifySomething")]
        public void An_issue_is_reported_for_wrong_named_method_(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("NotifySomething")]
        [TestCase("OnNotify")]
        [TestCase("OnNotifySomething")]
        public void An_issue_is_reported_for_wrong_named_local_function_(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [TestCase("class TestMe { void NotifySomething() { } }", "class TestMe { void OnSomething() { } }")]
        [TestCase("class TestMe { void OnNotifySomething() { } }", "class TestMe { void OnSomething() { } }")]
        public void Code_gets_fixed_for_method_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [TestCase("class TestMe { void Do() { void NotifySomething() { } } }", "class TestMe { void Do() { void OnSomething() { } } }")]
        [TestCase("class TestMe { void Do() { void OnNotifySomething() { } } }", "class TestMe { void Do() { void OnSomething() { } } }")]
        public void Code_gets_fixed_for_local_function_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1013_NotifyMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1013_NotifyMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1013_CodeFixProvider();
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3214_BeginEndScopeMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_scope_methods_in_([Values("class", "struct", "record")] string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe
{
    public void BeginUpdate() { }

    public void EndUpdate() { }

    public void EnterMethod() { }

    public void ExitMethod() { }

    public void LeaveMethod() { }
}
");

        [Test]
        public void No_issue_is_reported_for_interface_with_no_scope_method() => No_issue_is_reported_for(@"
public interface TestMe
{
    public void Update() { }
}
");

        [TestCase("BeginUpdate")]
        [TestCase("EndUpdate")]
        [TestCase("EnterMethod")]
        [TestCase("ExitMethod")]
        [TestCase("LeaveMethod")]
        public void An_issue_is_reported_for_interface_with_scope_method_(string methodName) => An_issue_is_reported_for(@"
public interface TestMe
{
    public void " + methodName + @"() { }
}
");

        protected override string GetDiagnosticId() => MiKo_3214_BeginEndScopeMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3214_BeginEndScopeMethodsAnalyzer();
    }
}
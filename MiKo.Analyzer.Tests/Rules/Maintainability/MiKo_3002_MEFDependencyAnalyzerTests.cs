using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3002_MEFDependencyAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_class_without_MEF() => No_issue_is_reported_for(@"
public class TestMe
{
    private void DoSomething() { }
}");

        [Test]
        public void No_issue_is_reported_for_interface() => No_issue_is_reported_for(@"
public interface TestMe
{
    void DoSomething() { }
}");

        [Test]
        public void No_issue_is_reported_for_class_with_less_than_allowd_dependencies() => No_issue_is_reported_for(@"
public class TestMe
{
    [Import]
    public string Dependency1 { get; set; }

    void DoSomething() { }
}");

        [Test]
        public void No_issue_is_reported_for_class_with_max_allowed_dependencies() => No_issue_is_reported_for(@"
public class TestMe
{
    [ImportingConstructor]
    public TestMe(string dependency1, string dependency2, string dependency3) { }

    [Import]
    public string Dependency4 { get; set; }

    [Import]
    public string Dependency5 { get; set; }

    void DoSomething() { }
}");

        [Test]
        public void An_issue_is_reported_for_class_with_slightly_more_than_allowed_dependencies() => An_issue_is_reported_for(@"
public class TestMe
{
    [ImportingConstructor]
    public TestMe(string dependency1, string dependency2, string dependency3, string dependency4, string dependency5) { }

    [Import]
    public string Dependency6 { get; set; }

    void DoSomething() { }
}");

        [Test]
        public void An_issue_is_reported_for_class_with_much_more_than_allowed_dependencies() => An_issue_is_reported_for(@"
public class TestMe
{
    [ImportingConstructor]
    public TestMe(string dependency1, string dependency2, string dependency3, string dependency4, string dependency5) { }

    [Import]
    public string Dependency6 { get; set; }

    [ImportMany]
    public string Dependency7 { get; set; }

    [Import]
    public string Dependency8 { get; set; }

    void DoSomething() { }
}");

        protected override string GetDiagnosticId() => MiKo_3002_MEFDependencyAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3002_MEFDependencyAnalyzer();
    }
}
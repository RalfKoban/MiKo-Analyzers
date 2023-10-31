using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1068_WorkflowMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_methods_of_non_WorkFlow_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
    public bool CanDoSomething() { }
    public async Task DoSomethingAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_methods_of_Workflow_class() => No_issue_is_reported_for(@"
public class Workflow
{
    public void Run() { }
    public async Task RunAsync() { }
    public bool CanRun() { }
    public async Task<bool> CanRunAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_methods_of_Workflow_interface() => No_issue_is_reported_for(@"
public interface IWorkflow
{
    void Run();
    Task RunAsync();
    bool CanRun();
    Task<bool> CanRunAsync();
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_Run_method_of_Workflow_class() => An_issue_is_reported_for(@"
public class Workflow
{
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_CanRun_method_of_Workflow_class() => An_issue_is_reported_for(@"
public class Workflow
{
    public bool Whatever() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_RunAsync_method_of_Workflow_class() => An_issue_is_reported_for(@"
public class Workflow
{
    public async Task BlaAsync() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_CanRunAsync_method_of_Workflow_class() => An_issue_is_reported_for(@"
public class Workflow
{
    public async Task<bool> BlaAgain() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1068_WorkflowMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1068_WorkflowMethodsAnalyzer();
    }
}
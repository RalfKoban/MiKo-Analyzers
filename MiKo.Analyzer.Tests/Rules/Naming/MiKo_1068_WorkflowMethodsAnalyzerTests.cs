using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1068_WorkflowMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_Workflow_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
    public bool CanDoSomething() { }
    public async Task DoSomethingAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_Workflow_class_with_Run_and_CanRun_methods() => No_issue_is_reported_for(@"
public class Workflow
{
    public void Run() { }
    public async Task RunAsync() { }
    public bool CanRun() { }
    public async Task<bool> CanRunAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_Workflow_interface_with_Run_and_CanRun_methods() => No_issue_is_reported_for(@"
public interface IWorkflow
{
    void Run();
    Task RunAsync();
    bool CanRun();
    Task<bool> CanRunAsync();
}
");

        [Test]
        public void An_issue_is_reported_for_Workflow_class_with_void_method_not_named_Run() => An_issue_is_reported_for(@"
public class Workflow
{
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_Workflow_class_with_boolean_method_not_named_CanRun() => An_issue_is_reported_for(@"
public class Workflow
{
    public bool Whatever() { }
}
");

        [Test]
        public void An_issue_is_reported_for_Workflow_class_with_Task_method_not_named_RunAsync() => An_issue_is_reported_for(@"
public class Workflow
{
    public async Task BlaAsync() { }
}
");

        [Test]
        public void An_issue_is_reported_for_Workflow_class_with_Task_bool_method_not_named_CanRunAsync() => An_issue_is_reported_for(@"
public class Workflow
{
    public async Task<bool> BlaAgain() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1068_WorkflowMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1068_WorkflowMethodsAnalyzer();
    }
}
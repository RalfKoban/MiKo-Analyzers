using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3071_TaskMethodReturnsNullAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_void_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() {}
}");

        [Test]
        public void No_issue_is_reported_for_non_Task_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething()
    {
        return null;
    }
}");

        [Test]
        public void No_issue_is_reported_for_non_Task_method_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething() => null;
}");

        [Test]
        public void No_issue_is_reported_for_Task_method_returning_a_completed_Task() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        return Task.CompletedTask;
    }
}");

        [Test]
        public void No_issue_is_reported_for_Task_method_body_returning_a_completed_Task() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.CompletedTask;
}");

        [Test]
        public void No_issue_is_reported_for_Task_method_returning_a_Task_from_result() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        return Task.FromResult(null);
    }
}");

        [Test]
        public void No_issue_is_reported_for_Task_method_body_returning_a_Task_from_result() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.FromResult(null);
}");

        [Test]
        public void An_issue_is_reported_for_Task_method_returning_null() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        return null;
    }
}");

        [Test]
        public void An_issue_is_reported_for_Task_method_body_returning_null() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => null;
}");

        protected override string GetDiagnosticId() => MiKo_3071_TaskMethodReturnsNullAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3071_TaskMethodReturnsNullAnalyzer();
    }
}
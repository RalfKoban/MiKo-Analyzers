using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3020_CompletedTaskAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_that_returns_void() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_no_task() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_a_generic_task() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task<int> DoSomething() => Task.FromResult(42);
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_a_completed_task() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_a_completed_task_from_a_result() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.FromResult(42);
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_creates_and_returns_a_completed_task_from_a_result() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        var result = Task.FromResult(42);
        return result;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3020_CompletedTaskAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3020_CompletedTaskAnalyzer();
    }
}
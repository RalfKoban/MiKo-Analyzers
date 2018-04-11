using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3021_TaskRunAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_that_returns_a_completed_task_from_a_result() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.FromResult(42);
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_creates_and_returns_a_completed_task_from_a_result() => No_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_method_that_returns_a_Task_that_is_started_with_Run_method() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        return Task.Run(DoSomething);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_body_method_that_returns_a_Task_that_is_started_with_Run_method() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.Run(DoSomething);
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_a_Task_variable_that_is_started_with_Run_method() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        var result = Task.Run(DoSomething);
        return result;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_awaits_a_Task_that_is_started_with_Run_method() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomething()
    {
        await Task.Run(DoSomething);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_body_method_that_awaits_a_Task_that_is_started_with_Run_method() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomething() => await Task.Run(DoSomething);
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_awaits_a_Task_variable_that_is_started_with_Run_method() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomething()
    {
        var result = Task.Run(DoSomething);
        await result;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3021_TaskRunAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3021_TaskRunAnalyzer();
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        public void No_issue_is_reported_for_incomplete_method() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        return ; // return value is still missing here
    }
}");

        [Test]
        public void No_issue_is_reported_for_incomplete_method_body() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => ; // return value is still missing here
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

        [Test]
        public void An_issue_is_reported_for_Task_method_returning_variable_that_is_null_in_first_assignment_and_non_null_in_last_assignment() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(bool flag)
    {
        Task variable;

        if (flag)
            variable = null;
        else
            variable = Task.CompletedTask;

        return variable;
    }
}");

        [Test]
        public void An_issue_is_reported_for_Task_method_returning_Coalescence_operator_with_null_on_right_side() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(Task value)
    {
        return value ?? null;
    }
}");

        [Test]
        public void An_issue_is_reported_for_Task_method_returning_conditional_with_null_and_variable_that_has_assignments() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(bool flag)
    {
        Task variable = Task.CompletedTask;

        return flag ? null : variable;
    }
}");

        [Test]
        public void An_issue_is_reported_for_Task_method_body_returning_Coalescence_operator_with_null_on_right_side() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(Task value) => value ?? null;
}");

        [Test]
        public void No_issue_is_reported_for_Task_method_returning_a_variable_that_is_null_in_if_without_braces_but_then_reassigned() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(bool flag)
    {
        Task variable;

        if (flag)
            variable = null;

        variable = Task.CompletedTask;

        return variable;
    }
}");

        [Test]
        public void No_issue_is_reported_for_Task_method_returning_a_variable_that_is_null_in_if_else_but_then_reassigned() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(bool flag)
    {
        Task variable;

        if (flag)
        {
            variable = null;
        }
        else
        {
            variable = null;
        }

        variable = Task.CompletedTask;

        return variable;
    }
}");

        [Test]
        public void No_issue_is_reported_for_Task_method_returning_a_variable_that_is_null_in_try_but_then_reassigned() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(bool flag)
    {
        Task variable;

        try
        {
            if (flag)
                variable = null;
        }
        finally
        {
            Console.WriteLine();
        }

        variable = Task.CompletedTask;

        return variable;
    }
}");

        [Test]
        public void No_issue_is_reported_for_Task_method_returning_a_variable_that_is_null_in_loop_but_then_reassigned() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(bool flag)
    {
        Task variable;

        while (flag)
        {
            if (flag)
                variable = null;

            flag = false;
        }

        variable = Task.CompletedTask;

        return variable;
    }
}");

        [Test]
        public void An_issue_is_reported_for_Task_method_returning_a_variable_that_is_null_and_returned_before_reassignment() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(bool flag)
    {
        Task variable = null;

        if (flag)
            return variable;

        variable = Task.CompletedTask;

        return variable;
    }
}");

        protected override string GetDiagnosticId() => MiKo_3071_TaskMethodReturnsNullAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3071_TaskMethodReturnsNullAnalyzer();
    }
}
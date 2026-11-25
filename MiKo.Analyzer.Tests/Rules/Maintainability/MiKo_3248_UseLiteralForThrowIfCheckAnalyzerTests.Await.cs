using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_awaited_task_variable_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomething(Task<string> pending) => " + invocation + @"(await pending);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_awaited_task_variable_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomething(Task<string> pending, string paramName) => " + invocation + @"(await pending, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_awaited_task_variable_and_ConfigureAwait_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomething(Task<string> pending) => " + invocation + @"(await pending.ConfigureAwait(false));
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_awaited_task_invocation_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomething() => " + invocation + @"(await GetValueAsync());

    private Task<string> GetValueAsync() => Task.FromResult(""value"");
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_parenthesized_awaited_task_variable_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomething(Task<string> pending) => " + invocation + @"((await pending));
}
");

        [Test]
        public void No_issue_is_reported_for_String_invocation_with_awaited_value_assigned_to_variable_([ValueSource(nameof(NullChecks))] string invocation) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomething(Task<string> pending)
    {
        var value = await pending;

        " + invocation + @"(value);
    }
}
");
    }
}

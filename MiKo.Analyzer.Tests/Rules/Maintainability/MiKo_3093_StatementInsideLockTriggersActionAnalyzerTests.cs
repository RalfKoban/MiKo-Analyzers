using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3093_StatementInsideLockTriggersActionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_method_without_action_parameter_trigger() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
        lock (this)
        {
            DoSomething();
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_action_parameter_trigger_in_code_block_of_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(Action action)
    {
        action();
    }
}");

        [Test]
        public void No_issue_is_reported_for_action_parameter_trigger_via_elvis_operator_in_code_block_of_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(Action action)
    {
        action?.Invoke();
    }
}");

        [Test]
        public void An_issue_is_reported_for_action_parameter_trigger_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(Action action)
    {
        lock (this)
        {
            action();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_action_parameter_trigger_via_elvis_operator_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(Action action)
    {
        lock (this)
        {
            action?.Invoke();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_action_parameter_trigger_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(Action action, bool condition)
    {
        lock (this)
        {
            if (condition)
                action();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_block_action_parameter_trigger_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(Action action, bool condition)
    {
        lock (this)
        {
            if (condition)
            {
                action();
            }
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_action_variable_trigger_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
        Action action = null;

        lock (this)
        {
            action();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_action_field_trigger_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private Action _action = null;

    public bool DoSomething()
    {
        lock (this)
        {
            _action();
        }
    }
}");

        [Test] // that's why we have rule MiKo_3092
        public void No_issue_is_reported_for_event_raise_in_lock_statement_of_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        lock (this)
        {
            MyEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_just_using_delegate_variables_in_lock_statement_of_method() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    private readonly List<Action> _actions = new List<Action>();

    public bool DoSomething(Action action)
    {
        lock (this)
        {
            if (!_actions.Contains(action))
            {
                _actions.Add(action);
            }
        }
    }
}");

        protected override string GetDiagnosticId() => MiKo_3093_StatementInsideLockTriggersActionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3093_StatementInsideLockTriggersActionAnalyzer();
    }
}
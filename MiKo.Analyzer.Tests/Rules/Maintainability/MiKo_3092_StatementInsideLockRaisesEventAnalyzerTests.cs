using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3092_StatementInsideLockRaisesEventAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_method_without_event_raise() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        lock (this)
        {
            DoSomething();
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_event_raise_in_code_block_of_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        MyEvent?.Invoke(this, EventArgs.Empty);
    }
}");

        [Test]
        public void No_issue_is_reported_for_event_registration_in_lock_statement_of_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        lock (this)
        {
            MyEvent += OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_event_deregistration_in_lock_statement_of_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        lock (this)
        {
            MyEvent -= OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_access_event_raise_in_lock_statement_of_method() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_handler_event_raise_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        lock (this)
        {
            var handlers = MyEvent;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_direct_event_raise_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        lock (this)
        {
            MyEvent(this, EventArgs.Empty);
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_event_raise_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething(bool condition)
    {
        lock (this)
        {
            if (condition)
                MyEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_block_event_raise_in_lock_statement_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething(bool condition)
    {
        lock (this)
        {
            if (condition)
            {
                MyEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}");

        protected override string GetDiagnosticId() => MiKo_3092_StatementInsideLockRaisesEventAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3092_StatementInsideLockRaisesEventAnalyzer();
    }
}
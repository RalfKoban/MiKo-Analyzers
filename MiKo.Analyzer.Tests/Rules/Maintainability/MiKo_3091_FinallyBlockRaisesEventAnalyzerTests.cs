using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3091_FinallyBlockRaisesEventAnalyzerTests : CodeFixVerifier
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
        try
        {
            DoSomething();
        }
        finally
        {
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
        public void No_issue_is_reported_for_event_raise_in_try_block_of_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        try
        {
            MyEvent?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_event_raise_in_catch_block_of_method_without_finally_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        try
        {
        }
        catch
        {
            MyEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_event_raise_in_catch_block_of_method_with_finally_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        try
        {
        }
        catch
        {
            MyEvent?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_event_raise_in_finally_block_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething()
    {
        try
        {
        }
        finally
        {
            MyEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_event_raise_in_finally_block_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething(bool condition)
    {
        try
        {
        }
        finally
        {
            if (condition)
                MyEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_block_event_raise_in_finally_block_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public bool DoSomething(bool condition)
    {
        try
        {
        }
        finally
        {
            if (condition)
            {
                MyEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}");

        protected override string GetDiagnosticId() => MiKo_3091_FinallyBlockRaisesEventAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3091_FinallyBlockRaisesEventAnalyzer();
    }
}
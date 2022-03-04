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
        public void No_issue_is_reported_for_event_add_on_same_type_in_finally_block() => No_issue_is_reported_for(@"
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
            MyEvent += OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_remove_on_same_type_in_finally_block() => No_issue_is_reported_for(@"
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
            MyEvent -= OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_add_on_separate_type_in_finally_block() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public bool DoSomething()
    {
        var provider = new EventProvider();
        try
        {
        }
        finally
        {
            provider.MyEvent += OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_remove_on_separate_type_in_finally_block() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public bool DoSomething()
    {
        var provider = new EventProvider();
        try
        {
        }
        finally
        {
            provider.MyEvent -= OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_add_on_field_of_separate_type_in_finally_block() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    private EventProvider provider = new EventProvider();

    public bool DoSomething()
    {
        try
        {
        }
        finally
        {
            provider.MyEvent += OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_remove_on_field_of_separate_type_in_finally_block() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    private EventProvider provider = new EventProvider();

    public bool DoSomething()
    {
        try
        {
        }
        finally
        {
            provider.MyEvent -= OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_add_on_field_of_separate_type_in_finally_block_if_base_class_contains_similar_event() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMeBase
{
    public event EventHandler MyEvent;
}

public class TestMe : TestMeBase
{
    private EventProvider provider = new EventProvider();

    public bool DoSomething()
    {
        try
        {
        }
        finally
        {
            provider.MyEvent += OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_remove_on_field_of_separate_type_in_finally_block_if_base_class_contains_similar_event() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMeBase
{
    public event EventHandler MyEvent;
}

public class TestMe : TestMeBase
{
    private EventProvider provider = new EventProvider();

    public bool DoSomething()
    {
        try
        {
        }
        finally
        {
            provider.MyEvent -= OnMyEvent;
        }
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

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
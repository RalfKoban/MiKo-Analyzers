using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3242_RaceConditionOnEventRaiseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_if_statement_with_identifier_as_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_if_statement_with_only_null_check_on_event() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        if (MyEvent != null)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_if_statement_with_null_check_on_event_handler_variable() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        var handler = MyEvent;

        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_raised_via_null_conditional_invocation() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        MyEvent?.Invoke(this, EventArgs.Empty);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_event_raised_with_race_condition_without_block() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        if (MyEvent != null)
            MyEvent(this, EventArgs.Empty);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_event_raised_with_race_condition_with_block() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        if (MyEvent != null)
        {
            MyEvent(this, EventArgs.Empty);
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_event_raised_with_race_condition_on_single_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        if (MyEvent != null) MyEvent(this, EventArgs.Empty);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        MyEvent?.Invoke(this, EventArgs.Empty);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_raised_with_race_condition_without_block()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        if (MyEvent != null)
            MyEvent(this, EventArgs.Empty);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        MyEvent?.Invoke(this, EventArgs.Empty);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_raised_with_race_condition_with_block()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        if (MyEvent != null)
        {
            MyEvent(this, EventArgs.Empty);
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        MyEvent?.Invoke(this, EventArgs.Empty);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_raised_with_race_condition_with_code_before_if_statement()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        var x = 42;

        if (MyEvent != null)
        {
            MyEvent(this, EventArgs.Empty);
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        var x = 42;

        MyEvent?.Invoke(this, EventArgs.Empty);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_raised_with_race_condition_with_code_after_if_statement()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        if (MyEvent != null)
            MyEvent(this, EventArgs.Empty);

        var x = 42;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        MyEvent?.Invoke(this, EventArgs.Empty);

        var x = 42;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_raised_with_race_condition_with_code_before_and_after_if_statement()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        var x = 42;

        if (MyEvent != null)
        {
            MyEvent(this, EventArgs.Empty);
        }

        var y = 17;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        var x = 42;

        MyEvent?.Invoke(this, EventArgs.Empty);

        var y = 17;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_raised_with_race_condition_with_comment_before_if_statement()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        // some comment
        if (MyEvent != null)
        {
            MyEvent(this, EventArgs.Empty);
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        // some comment
        MyEvent?.Invoke(this, EventArgs.Empty);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_raised_with_race_condition_with_comment_on_event_raise()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        if (MyEvent != null)
        {
            // some comment
            MyEvent(this, EventArgs.Empty);
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        // some comment
        MyEvent?.Invoke(this, EventArgs.Empty);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_raised_with_race_condition_with_comment_behind_event_raise()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        if (MyEvent != null)
        {
            MyEvent(this, EventArgs.Empty); // some comment
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        MyEvent?.Invoke(this, EventArgs.Empty); // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3242_RaceConditionOnEventRaiseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3242_RaceConditionOnEventRaiseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3242_CodeFixProvider();
    }
}
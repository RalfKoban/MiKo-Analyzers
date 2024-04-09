using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6049_EventRegistrationsSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_adding_a_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int i = 0;

        i += 42;
        i += 43;
        i += 44;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_subtracting_a_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int i = 0;

        i -= 42;
        i -= 43;
        i -= 44;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_registering_to_own_event_in_case_it_is_surrounded_by_empty_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        MyEvent += OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_unregistering_from_own_event_in_case_it_is_surrounded_by_empty_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    {
        MyEvent -= OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_registering_to_an_event_in_case_it_is_surrounded_by_empty_lines() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent += OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_unregistering_from_an_event_in_case_it_is_surrounded_by_empty_lines() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent -= OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_registering_multiple_times_in_a_row_to_an_event_in_case_it_is_surrounded_by_empty_lines() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent += OnMyEvent;
        provider.MyEvent += OnMyEvent;
        provider.MyEvent += OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_unregistering_multiple_times_in_a_row_from_an_event_in_case_it_is_surrounded_by_empty_lines() => No_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent -= OnMyEvent;
        provider.MyEvent -= OnMyEvent;
        provider.MyEvent -= OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_registering_to_an_event_in_case_it_is_surrounded_by_a_different_call() => An_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent += OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_unregistering_from_an_event_in_case_it_is_surrounded_by_a_different_call() => An_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent -= OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_registering_to_an_event_in_case_it_is_preceded_by_a_different_call() => An_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent += OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_unregistering_from_an_event_in_case_it_is_preceded_by_a_different_call() => An_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent -= OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_registering_to_an_event_in_case_it_is_followed_by_a_different_call() => An_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent += OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_unregistering_from_an_event_in_case_it_is_followed_by_a_different_call() => An_issue_is_reported_for(@"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent -= OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_registering_to_an_event_in_case_it_is_surrounded_by_a_different_call()
        {
            const string OriginalCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent += OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);

        provider.MyEvent += OnMyEvent;

        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_unregistering_from_an_event_in_case_it_is_surrounded_by_a_different_call()
        {
            const string OriginalCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent -= OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);

        provider.MyEvent -= OnMyEvent;

        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_registering_multiple_times_in_a_row_to_an_event_in_case_it_is_surrounded_by_a_different_call()
        {
            const string OriginalCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent += OnMyEvent;
        provider.MyEvent += OnMyEvent;
        provider.MyEvent += OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);

        provider.MyEvent += OnMyEvent;
        provider.MyEvent += OnMyEvent;
        provider.MyEvent += OnMyEvent;

        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_unregistering_multiple_times_in_a_row_from_an_event_in_case_it_is_surrounded_by_a_different_call()
        {
            const string OriginalCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent -= OnMyEvent;
        provider.MyEvent -= OnMyEvent;
        provider.MyEvent -= OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);

        provider.MyEvent -= OnMyEvent;
        provider.MyEvent -= OnMyEvent;
        provider.MyEvent -= OnMyEvent;

        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_registering_to_an_event_in_case_it_is_preceded_by_a_different_call()
        {
            const string OriginalCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent += OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);

        provider.MyEvent += OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_unregistering_from_an_event_in_case_it_is_preceded_by_a_different_call()
        {
            const string OriginalCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);
        provider.MyEvent -= OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        DoSomething(provider);

        provider.MyEvent -= OnMyEvent;
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_registering_to_an_event_in_case_it_is_followed_by_a_different_call()
        {
            const string OriginalCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent += OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent += OnMyEvent;

        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_unregistering_from_an_event_in_case_it_is_followed_by_a_different_call()
        {
            const string OriginalCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent -= OnMyEvent;
        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class EventProvider
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public void DoSomething(EventProvider provider)
    {
        provider.MyEvent -= OnMyEvent;

        DoSomething(provider);
    }

    private void OnMyEvent(object sender, EventArgs e)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6049_EventRegistrationsSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6049_EventRegistrationsSurroundedByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6049_CodeFixProvider();
    }
}
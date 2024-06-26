using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1003_EventHandlingMethodNamePrefixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_event_handling_method() => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method() => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void OnWhatever(object sender, EventArgs e) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_local_function() => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething()
    {
        void OnWhatever(object sender, EventArgs e) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_overridden_method() => No_issue_is_reported_for(@"

using System;

public class TestMe : BaseClass
{
    public override void Whatever(object sender, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method() => An_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void Whatever(object sender, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method_with_own_event_assignment() => An_issue_is_reported_for(@"

using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void Initialize() => MyEvent += Whatever;

    public void Whatever(object sender, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method_with_other_event_assignment() => An_issue_is_reported_for(@"

using System;

public class TestMeEvent
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public TestMe(TestMeEvent tme) => TME = tme;

    public TestMeEvent TME { get; set; }

    public void Initialize() => TME.MyEvent += Whatever;

    public void Whatever(object sender, EventArgs e) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method_with_other_event_assignment() => No_issue_is_reported_for(@"

using System;

public class TestMeEvent
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public TestMe(TestMeEvent tme) => TME = tme;

    public TestMeEvent TME { get; set; }

    public void Initialize() => TME.MyEvent += OnTMEMyEvent;

    public void OnTMEMyEvent(object sender, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method_with_underscore_inside_other_event_assignment() => An_issue_is_reported_for(@"

using System;

public class TestMeEvent
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public TestMe(TestMeEvent tme) => TME = tme;

    public TestMeEvent TME { get; set; }

    public void Initialize() => TME.MyEvent += On_TME_MyEvent;

    public void On_TME_MyEvent(object sender, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_local_function_with_other_event_assignment() => An_issue_is_reported_for(@"

using System;

public class TestMeEvent
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public TestMe(TestMeEvent tme) => TME = tme;

    public TestMeEvent TME { get; set; }

    public void Initialize()
    {
        TME.MyEvent += Whatever;
    
        void Whatever(object sender, EventArgs e) { }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_([Values("TME_MyEvent", "myEvent")] string method)
        {
            const string Template = @"
using System;

public class TestMeEvent
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public TestMe(TestMeEvent tme) => TME = tme;

    public TestMeEvent TME { get; set; }

    public void Initialize() => TME.MyEvent += #;

    public void #(object sender, EventArgs e) { }
}";

            VerifyCSharpFix(Template.Replace("#", method), Template.Replace("#", "OnMyEvent"));
        }

        [Test]
        public void Code_gets_fixed_for_local_function()
        {
            const string Template = @"
using System;

public class TestMeEvent
{
    public event EventHandler MyEvent;
}

public class TestMe
{
    public TestMe(TestMeEvent tme) => TME = tme;

    public TestMeEvent TME { get; set; }

    public void Initialize()
    {
        TME.MyEvent += #;

        void #(object sender, EventArgs e) { }
    }
}";

            VerifyCSharpFix(Template.Replace("#", "TME_MyEvent"), Template.Replace("#", "OnMyEvent"));
        }

        [TestCase("CommandBinding_OnCanExecute", "OnCanExecuteCommandBinding")]
        [TestCase("CommandBinding_OnExecuted", "OnExecutedCommandBinding")]
        public void Code_gets_fixed_for_method_(string originalMethod, string fixedMethod)
        {
            const string Template = @"
using System;

public class TestMe
{
    public void #(object sender, EventArgs e) { }
}";

            VerifyCSharpFix(Template.Replace("#", originalMethod), Template.Replace("#", fixedMethod));
        }

        protected override string GetDiagnosticId() => MiKo_1003_EventHandlingMethodNamePrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1003_EventHandlingMethodNamePrefixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1003_CodeFixProvider();
    }
}
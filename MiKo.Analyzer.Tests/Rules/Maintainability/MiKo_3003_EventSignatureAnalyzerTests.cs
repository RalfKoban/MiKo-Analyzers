using System;
using System.Collections.Specialized;
using System.ComponentModel;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3003_EventSignatureAnalyzerTests : CodeFixVerifier
    {
        [TestCase(nameof(EventHandler))]
        [TestCase("EventHandler<EventArgs>")]
        [TestCase(nameof(NotifyCollectionChangedEventHandler))]
        [TestCase(nameof(PropertyChangedEventHandler))]
        [TestCase(nameof(PropertyChangingEventHandler))]
        [TestCase(nameof(CancelEventHandler))]
        public void No_issue_is_reported_for_EventHandler_(string handler) => No_issue_is_reported_for(@"

public class TestMe
{
    public event " + handler + @" MyEvent;
}
");

        [TestCase(nameof(EventHandler))]
        [TestCase("EventHandler<EventArgs>")]
        [TestCase(nameof(NotifyCollectionChangedEventHandler))]
        [TestCase(nameof(PropertyChangedEventHandler))]
        [TestCase(nameof(PropertyChangingEventHandler))]
        [TestCase(nameof(CancelEventHandler))]
        public void No_issue_is_reported_for_EventHandler_with_usings_(string handler) => No_issue_is_reported_for(@"
using System;
using System.Collections.Specialized;
using System.ComponentModel;

public class TestMe
{
    public event " + handler + @" MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_Action() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Action_with_1_Argument() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action<MyEventArgs> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Action_with_correct_2_Arguments() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action<object, MyEventArgs> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Action_with_incorrect_2_Arguments() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action<string, MyEventArgs> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Action_with_3_Arguments() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action<object, MyEventArgs, int> MyEvent;
}
");

        protected override string GetDiagnosticId() => MiKo_3003_EventSignatureAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3003_EventSignatureAnalyzer();
    }
}
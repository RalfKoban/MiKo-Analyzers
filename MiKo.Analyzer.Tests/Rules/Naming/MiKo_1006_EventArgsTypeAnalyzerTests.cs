using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1006_EventArgsTypeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_event_with_generic_EventHandler_using_correct_EventArgs() => No_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public event EventHandler<MyEventArgs> My;
}
");

        [Test]
        public void No_issue_is_reported_for_predefined_CanExecuteChanged_EventHandler() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;
using System.Collections.Specialized;

public class TestMe
{
    public event EventHandler CanExecuteChanged;
}
");

        [Test]
        public void An_issue_is_reported_for_event_with_generic_EventHandler_using_incorrect_EventArgs() => An_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public event EventHandler<MyEventArgs> MyOwn;
}
");

        [Test]
        public void An_issue_is_reported_for_non_generic_EventHandler() => An_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public event EventHandler My;
}
");

        [Test, Ignore("Currently cannot be tested")]
        public void No_issue_is_reported_for_interface_implementation() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

namespace Bla
{
    public abstract class TestMe : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1006_EventArgsTypeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1006_EventArgsTypeAnalyzer();
    }
}
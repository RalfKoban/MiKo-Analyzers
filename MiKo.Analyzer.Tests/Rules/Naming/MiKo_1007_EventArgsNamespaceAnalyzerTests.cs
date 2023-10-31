using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1007_EventArgsNamespaceAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_predefined_CanExecuteChanged_EventHandler() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler CanExecuteChanged;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_generic_EventHandler() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler My;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_in_same_namespace() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class MyEventArgs : EventArgs { }

    public class TestMe
    {
        public event EventHandler<MyEventArgs> My;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_dotNet_conform_event() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler<int> My;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_EventArgs_class_itself() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler<EventArgs> My;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_event_in_different_namespace() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class MyEventArgs : EventArgs { }
}

namespace Blubb
{
    using Bla;

public class TestMe
    {
        public event EventHandler<MyEventArgs> My;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1007_EventArgsNamespaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1007_EventArgsNamespaceAnalyzer();
    }
}
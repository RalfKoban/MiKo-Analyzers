using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1542_SameEventWithDifferentNamesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_without_events() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_single_event() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;
}
");

        [TestCase("Finishing", "Finished")]
        [TestCase("Opening", "Opened")]
        [TestCase("ProjectOpening", "ProjectOpened")]
        [TestCase("OpeningProject", "OpenedProject")]
        public void No_issue_is_reported_for_type_with_different_events_(string name1, string name2) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler " + name1 + @";

    public event EventHandler " + name2 + @";
}
");

        [Test]
        public void No_issue_is_reported_for_events_of_different_interfaces_that_are_explicitly_implemented() => No_issue_is_reported_for(@"
using System;

public interface IFirst
{
    event EventHandler Finishing;
}

public interface ISecond
{
    event EventHandler OnFinishing;
}

public class TestMe : IFirst, ISecond
{
    event EventHandler IFirst.Finishing
    {
        add { }
        remove { }
    }

    event EventHandler ISecond.OnFinishing
    {
        add { }
        remove { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_events_of_different_interfaces_that_are_explicitly_implemented_as_event_fields() => No_issue_is_reported_for(@"
using System;

public interface IFirst
{
    event EventHandler Finishing;
}

public interface ISecond
{
    event EventHandler OnFinishing;
}

public class TestMe : IFirst, ISecond
{
    event EventHandler IFirst.Finishing;

    event EventHandler ISecond.OnFinishing;
}
");

        [Test]
        public void No_issue_is_reported_for_event_inherited_via_diamond_inheritance() => No_issue_is_reported_for(@"
using System;

public interface IBase
{
    event EventHandler Finishing;
}

public interface ILeft : IBase
{
}

public interface IRight : IBase
{
}

public interface IDerived : ILeft, IRight
{
    event EventHandler Started;
}
");

        [Test]
        public void An_issue_is_reported_for_diamond_inheritance_with_colliding_event_only_once() => An_issue_is_reported_for(@"
using System;

public interface IBase
{
    event EventHandler Finishing;
}

public interface ILeft : IBase
{
}

public interface IRight : IBase
{
}

public interface IDerived : ILeft, IRight
{
    event EventHandler OnFinishing;
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [TestCase("Finishing", "finishing")]
        [TestCase("Finishing", "finishingEvent")]
        [TestCase("Finishing", "FinishingEvent")]
        [TestCase("Finishing", "OnFinishing")]
        [TestCase("Finishing", "onFinishingEvent")]
        [TestCase("Finishing", "OnFinishingEvent")]
        [TestCase("AfterDocumentOpen", "OpenedDocument")]
        [TestCase("AfterDocumentOpened", "OpenedDocument")]
        [TestCase("AfterDocumentOpening", "OpenedDocument")]
        [TestCase("AfterFileOpen", "FileOpened")]
        [TestCase("AfterFileOpen", "OpenedFile")]
        [TestCase("AfterFinalize", "Finalized")]
        [TestCase("AfterFinish", "Finished")]
        [TestCase("AfterFinishEvent", "Finished")]
        [TestCase("AfterNameChange", "ChangedName")]
        [TestCase("AfterNameChange", "NameChanged")]
        [TestCase("AfterOpenDocument", "OpenedDocument")]
        [TestCase("AfterOpenedDocument", "OpenedDocument")]
        [TestCase("AfterOpeningDocument", "OpenedDocument")]
        [TestCase("AfterProjectOpen", "OpenedProject")]
        [TestCase("AfterViewOpened", "ViewOpened")]
        [TestCase("BeforeDocumentOpen", "OpeningDocument")]
        [TestCase("BeforeDocumentOpened", "OpeningDocument")]
        [TestCase("BeforeDocumentOpening", "OpeningDocument")]
        [TestCase("BeforeFileOpen", "FileOpening")]
        [TestCase("BeforeFileOpen", "OpeningFile")]
        [TestCase("BeforeFinalize", "Finalizing")]
        [TestCase("BeforeFinish", "Finishing")]
        [TestCase("BeforeFinishEvent", "Finishing")]
        [TestCase("BeforeNameChange", "ChangingName")]
        [TestCase("BeforeNameChange", "NameChanging")]
        [TestCase("BeforeOpenDocument", "OpeningDocument")]
        [TestCase("BeforeOpenedDocument", "OpeningDocument")]
        [TestCase("BeforeOpeningDocument", "OpeningDocument")]
        [TestCase("BeforeProjectOpen", "OpeningProject")]
        [TestCase("BeforeViewOpening", "ViewOpening")]
        public void An_issue_is_reported_for_type_with_different_event_names_but_same_meaning_(string name1, string name2) => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public event EventHandler " + name1 + @";

    public event EventHandler " + name2 + @";
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_type_with_different_event_parameters() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public event EventHandler<EventArgs> Finishing;

    public event EventHandler<ConsoleCancelEventArgs> OnFinishing;
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_type_with_different_event_types() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public event Action Finishing;

    public event Func<int> FinishingEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_interface_event_with_same_name_but_different_event_data_as_inherited_interface_event() => An_issue_is_reported_for(@"
using System;

public class OtherArgs : EventArgs { }

public interface IBase
{
    event EventHandler Finalized;
}

public interface IDerived : IBase
{
    new event EventHandler<OtherArgs> Finalized;
}
");

        [Test]
        public void An_issue_is_reported_for_interface_event_with_different_name_but_same_meaning_as_inherited_interface_event() => An_issue_is_reported_for(@"
using System;

public interface IBase
{
    event EventHandler Finishing;
}

public interface IDerived : IBase
{
    event EventHandler OnFinishing;
}
");

        [Test]
        public void An_issue_is_reported_for_interface_event_with_different_name_as_event_inherited_from_indirect_base_interface() => An_issue_is_reported_for(@"
using System;

public interface IBase
{
    event EventHandler Finishing;
}

public interface IMiddle : IBase
{
}

public interface IDerived : IMiddle
{
    event EventHandler OnFinishing;
}
");

        protected override string GetDiagnosticId() => MiKo_1542_SameEventWithDifferentNamesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1542_SameEventWithDifferentNamesAnalyzer();
    }
}
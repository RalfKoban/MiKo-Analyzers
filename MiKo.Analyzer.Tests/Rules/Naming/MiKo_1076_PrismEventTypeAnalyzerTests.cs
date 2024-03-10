using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1076_PrismEventTypeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_EventArgs() => No_issue_is_reported_for(@"

namespace Microsoft.Practices.Prism.Events
{
    public abstract class EventBase { }
}

namespace MyNamespace
{
    using Microsoft.Practices.Prism.Events;

    public class MyEvent : EventBase
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_EventArgs() => An_issue_is_reported_for(@"

namespace Microsoft.Practices.Prism.Events
{
    public abstract class EventBase { }
}

namespace MyNamespace
{
    using Microsoft.Practices.Prism.Events;

    public class MyEventArgs : EventBase
    {
    }
}
");

        [TestCase("TestMe", "TestMeEvent")]
        [TestCase("TestMeArg", "TestMeEvent")]
        [TestCase("TestMeArgs", "TestMeEvent")]
        [TestCase("TestMeBaseArg", "TestMeEvent")]
        [TestCase("TestMeBaseArgs", "TestMeEvent")]
        [TestCase("TestMeEventArg", "TestMeEvent")]
        [TestCase("TestMeEventArgBase", "TestMeEvent")]
        [TestCase("TestMeEventArgs", "TestMeEvent")]
        [TestCase("TestMeEventArgsBase", "TestMeEvent")]
        [TestCase("TestMeEventBaseArg", "TestMeEvent")]
        [TestCase("TestMeEvents", "TestMeEvent")]
        [TestCase("TestMeEventsArg", "TestMeEvent")]
        [TestCase("TestMeEventsArgBase", "TestMeEvent")]
        [TestCase("TestMeEventsArgs", "TestMeEvent")]
        [TestCase("TestMeEventsArgsBase", "TestMeEvent")]
        [TestCase("TestMeEventsBaseArg", "TestMeEvent")]
        [TestCase("TestMeEventsBaseArgs", "TestMeEvent")]
        public void Code_gets_fixed_(string originalName, string fixedName)
        {
            const string Template = @"

namespace Microsoft.Practices.Prism.Events
{
    public abstract class EventBase { }
}

namespace MyNamespace
{
    using Microsoft.Practices.Prism.Events;

    public class ### : EventBase
    {
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1076_PrismEventTypeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1076_PrismEventTypeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1076_CodeFixProvider();
    }
}
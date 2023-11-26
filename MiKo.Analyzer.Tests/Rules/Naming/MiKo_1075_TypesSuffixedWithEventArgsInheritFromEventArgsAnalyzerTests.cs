using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_EventArgs_type_not_named_EventArgs() => No_issue_is_reported_for(@"
using System;

public class TestMe : EventArgs
{
}
");

        [Test]
        public void No_issue_is_reported_for_EventArgs_type_named_EventArgs() => No_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs
{
}
");

        [TestCase("MyEventArg")]
        [TestCase("MyEventsArg")]
        [TestCase("MyEventArgs")]
        [TestCase("MyEventsArgs")]
        public void An_issue_is_reported_for_non_EventArgs_type_incorrectly_named_(string name) => An_issue_is_reported_for(@"
using System;

public class " + name + @"
{
}
");

        [TestCase("MyEventArg")]
        [TestCase("MyEventsArg")]
        [TestCase("MyEventArgs")]
        [TestCase("MyEventsArgs")]
        public void An_issue_is_reported_for_Prism_event_type_incorrectly_named_(string name) => An_issue_is_reported_for(@"

namespace Microsoft.Practices.Prism.Events
{
    public abstract class EventBase { }
}

namespace MyNamespace
{
    using Microsoft.Practices.Prism.Events;

    public class " + name + @" : EventBase
    {
    }
}
");

        [TestCase("using System; class TestMeEventArg { }", "using System; class TestMe { }")]
        [TestCase("using System; class TestMeEventArgs { }", "using System; class TestMe { }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [TestCase("TestMeEventArg", "TestMeEvent")]
        [TestCase("TestMeEventsArg", "TestMeEvent")]
        [TestCase("TestMeEventArgs", "TestMeEvent")]
        [TestCase("TestMeEventsArgs", "TestMeEvent")]
        public void Code_gets_fixed_for_Prism_event_(string originalName, string fixedName)
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

        protected override string GetDiagnosticId() => MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1075_CodeFixProvider();
    }
}
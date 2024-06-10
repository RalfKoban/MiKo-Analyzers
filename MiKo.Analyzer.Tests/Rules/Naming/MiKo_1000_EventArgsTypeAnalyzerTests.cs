using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1000_EventArgsTypeAnalyzerTests : CodeFixVerifier
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
using System;

public class MyEventArgs : EventArgs
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_EventArgs() => An_issue_is_reported_for(@"
using System;

public class TestMe : EventArgs
{
}
");

        [Test]
        public void No_issue_is_reported_for_record() => No_issue_is_reported_for(@"
public record TestMe
{
}
");

        [TestCase("using System; class TestMe : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeArg : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeArgs : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeBaseArg : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeBaseArgs : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventArg : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventsArg : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventsArgs : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventBaseArg : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventsBaseArg : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventsBaseArgs : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventArgBase : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventArgsBase : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventsArgBase : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        [TestCase("using System; class TestMeEventsArgsBase : EventArgs { }", "using System; class TestMeEventArgs : EventArgs { }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1000_EventArgsTypeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1000_EventArgsTypeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1000_CodeFixProvider();
    }
}
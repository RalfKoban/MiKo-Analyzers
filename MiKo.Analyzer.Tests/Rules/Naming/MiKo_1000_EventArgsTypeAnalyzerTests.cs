using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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
        public void Code_gets_fixed() => VerifyCSharpFix(
                                                         "using System; class TestMe : EventArgs { }",
                                                         "using System; class TestMeEventArgs : EventArgs { }");

        protected override string GetDiagnosticId() => MiKo_1000_EventArgsTypeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1000_EventArgsTypeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1000_CodeFixProvider();
    }
}
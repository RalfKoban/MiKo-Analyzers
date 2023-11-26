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

        [Test]
        public void An_issue_is_reported_for_non_EventArgs_type_incorrectly_named_EventArgs() => An_issue_is_reported_for(@"
using System;

public class MyEventArgs
{
}
");

        [TestCase("using System; class TestMeEventArgs { }", "using System; class TestMe { }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1075_CodeFixProvider();
    }
}
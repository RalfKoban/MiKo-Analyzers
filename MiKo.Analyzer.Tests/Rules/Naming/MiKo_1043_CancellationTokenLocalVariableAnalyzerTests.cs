using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1043_CancellationTokenLocalVariableAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_non_CancellationToken_variable() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_CancellationToken_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        CancellationToken token = CancellationToken.None;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_var_CancellationToken_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        var token = CancellationToken.None;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_CancellationToken_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        CancellationToken cancellationToken = CancellationToken.None;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_CancellationToken_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        var cancellationToken = CancellationToken.None;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_incorrect_name() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    private CancellationToken _token;

    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_variable_declaration_pattern_for_CancellationToken_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case CancellationToken token: return;
            default: return;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_variable_declaration_pattern_for_CancellationToken_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case CancellationToken cancellationToken: return;
            default: return;
        }
    }
}
");

        [TestCase(
             "using System.Threading; class TestMe { void DoSomething() { var cancellationToken = CancellationToken.None; } }",
             "using System.Threading; class TestMe { void DoSomething() { var token = CancellationToken.None; } }")]
        [TestCase(
            "using System.Threading; class TestMe { void DoSomething(object o) { switch (o) { case CancellationToken cancellationToken: return; default: return; } } }",
            "using System.Threading; class TestMe { void DoSomething(object o) { switch (o) { case CancellationToken token: return; default: return; } } }")]
        [TestCase(
            "using System.Threading; class TestMe { void DoSomething() { foreach (CancellationToken c in new CancellationToken[0]) { } } } }",
            "using System.Threading; class TestMe { void DoSomething() { foreach (CancellationToken token in new CancellationToken[0]) { } } } }")]
        [TestCase(
             "using System.Threading; class TestMe { void DoSomething(object o) { if (o is CancellationToken c) return; } } }",
             "using System.Threading; class TestMe { void DoSomething(object o) { if (o is CancellationToken token) return; } } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1043_CancellationTokenLocalVariableAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1043_CancellationTokenLocalVariableAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1043_CodeFixProvider();
    }
}
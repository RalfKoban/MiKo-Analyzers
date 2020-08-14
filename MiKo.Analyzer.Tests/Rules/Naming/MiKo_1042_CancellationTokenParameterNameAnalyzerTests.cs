using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1042_CancellationTokenParameterNameAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_without_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_but_no_CancellationToken_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_overridden_method_with_CancellationToken_parameter_having_wrong_name() => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public override void DoSomething(CancellationToken token, int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_CancellationToken_parameter_with_correct_name() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(int i, CancellationToken cancellationToken) { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_CancellationToken_parameter_having_wrong_name() => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(int i, CancellationToken token) { }
}
");

        [Test]
        public void Code_gets_fixed() => VerifyCSharpFix(
                                      "using System.Threading; class TestMe { void DoSomething(CancellationToken token) { } }",
                                      "using System.Threading; class TestMe { void DoSomething(CancellationToken cancellationToken) { } }");

        protected override string GetDiagnosticId() => MiKo_1042_CancellationTokenParameterNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1042_CancellationTokenParameterNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1042_CodeFixProvider();
    }
}
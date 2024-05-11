using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3006_CancellationTokenParameterPositionAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_overridden_method_with_CancellationToken_parameter_at_wrong_place() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public override void DoSomething(CancellationToken token, int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_CancellationToken_parameter_at_correct_place() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(int i, CancellationToken token) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_CancellationToken_parameter_when_followed_by_a_params_array() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(CancellationToken token, params int[] i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_CancellationToken_parameter_at_wrong_place() => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(CancellationToken token, int i) { }
}
");

        protected override string GetDiagnosticId() => MiKo_3006_CancellationTokenParameterPositionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3006_CancellationTokenParameterPositionAnalyzer();
    }
}
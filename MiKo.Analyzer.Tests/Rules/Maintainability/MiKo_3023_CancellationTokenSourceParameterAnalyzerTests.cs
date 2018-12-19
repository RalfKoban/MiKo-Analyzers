using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3023_CancellationTokenSourceParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_that_returns_void() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_takes_no_CancellationTokenSource() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public int DoSomething(CancellationToken token) => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_takes_a_CancellationTokenSource() => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(CancellationTokenSource source)
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3023_CancellationTokenSourceParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3023_CancellationTokenSourceParameterAnalyzer();
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_invocation_with_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(42);

    private void DoSomethingCore(int i) => { }
}
");

        [Test]
        public void No_issue_is_reported_for_invocation_with_null_as_non_collection() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(null);

    private void DoSomethingCore(object o) => { }
}
");

        [TestCase("int[] array")]
        [TestCase("params int[] array")]
        [TestCase("IEnumerable i")]
        [TestCase("IEnumerable<int> i")]
        [TestCase("List<int> i")]
        [TestCase("Dictionary<int, string> i")]
        public void An_issue_is_reported_for_invocation_with_null_as_(string parameter) => An_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(null);

    private void DoSomethingCore(" + parameter + @") => { }
}
");

        protected override string GetDiagnosticId() => MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzer();
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1047_NonAsyncMethodsButAsyncSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_non_async_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_non_async_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomethingAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_async_void_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async void DoSomethingAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_Task_method() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomethingAsync() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1047_NonAsyncMethodsButAsyncSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1047_NonAsyncMethodsButAsyncSuffixAnalyzer();
    }
}
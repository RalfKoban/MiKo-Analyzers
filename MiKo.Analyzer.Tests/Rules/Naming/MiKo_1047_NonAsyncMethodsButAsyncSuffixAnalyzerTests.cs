using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        public void No_issue_is_reported_for_correctly_named_non_async_local_function() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async void DoSomethingAsync()
    {
        void DoSomethingCore() { }
    }
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
        public void No_issue_is_reported_for_correctly_named_async_void_local_function() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async void DoSomethingAsync()
    {
        async void DoSomethingCoreAsync() { }
    }
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

        [Test]
        public void No_issue_is_reported_for_correctly_named_Task_local_function() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomethingAsync()
    {
        Task DoSomethingCoreAsync() { }
    }
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
        public void An_issue_is_reported_for_incorrectly_named_non_async_local_function() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async void DoSomethingAsync()
    {
        void DoSomethingCoreAsync() { }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method() => VerifyCSharpFix(
                                                                "class TestMe { void DoSomethingAsync() { } }",
                                                                "class TestMe { void DoSomething() { } }");

        [Test]
        public void Code_gets_fixed_for_local_function() => VerifyCSharpFix(
                                                                        "class TestMe { void DoSomething() { void SomethingAsync() { } } }",
                                                                        "class TestMe { void DoSomething() { void Something() { } } }");

        protected override string GetDiagnosticId() => MiKo_1047_NonAsyncMethodsButAsyncSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1047_NonAsyncMethodsButAsyncSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1047_CodeFixProvider();
    }
}
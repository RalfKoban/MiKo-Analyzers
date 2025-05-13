using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1118_TestMethodsButAsyncSuffixAnalyzerTests : CodeFixVerifier
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
    public async Task DoSomethingAsync()
    {
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
        public void No_issue_is_reported_for_incorrectly_named_non_async_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomethingAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_correct_name_(
                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                        [ValueSource(nameof(Tests))] string test)
        => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public Task DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_incorrect_name_(
                                                                          [ValueSource(nameof(TestFixtures))] string fixture,
                                                                          [ValueSource(nameof(Tests))] string test)
        => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public Task DoSomethingAsync() { }
}
");

        [Test]
        public void Code_gets_fixed_for_method_(
                                            [ValueSource(nameof(TestFixtures))] string fixture,
                                            [ValueSource(nameof(Tests))] string test)
        {
            var template = @"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public Task ###() { }
}
";

            VerifyCSharpFix(template.Replace("###", "DoSomethingAsync"), template.Replace("###", "DoSomething"));
        }

        protected override string GetDiagnosticId() => MiKo_1118_TestMethodsButAsyncSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1118_TestMethodsButAsyncSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1118_CodeFixProvider();
    }
}
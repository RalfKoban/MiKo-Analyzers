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
        public void No_issue_is_reported_for_non_async_method_without_Async_suffix() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_async_local_function_without_Async_suffix() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_async_void_method_with_Async_suffix() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async void DoSomethingAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_async_void_local_function_with_Async_suffix() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_Task_returning_method_with_Async_suffix() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomethingAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_Task_returning_local_function_with_Async_suffix() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_non_async_method_with_Async_suffix_outside_test_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomethingAsync() { }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_without_Async_suffix_(
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
        public void An_issue_is_reported_for_test_method_with_Async_suffix_(
                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                        [ValueSource(nameof(Tests))] string test,
                                                                        [Values("DoSomethingAsync", "Do_something_async", "DoSomethingAsync_", "Do_something_async_")] string methodName)
        => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public Task " + methodName + @"() { }
}
");

        [TestCase("DoSomethingAsync", "DoSomething")]
        [TestCase("DoSomethingAsync_", "DoSomething_")]
        [TestCase("Do_something_async", "Do_something")]
        [TestCase("Do_something_async_", "Do_something_")]
        public void Code_gets_fixed_by_removing_Async_suffix_from_method_name_(string originalName, string fixedName)
        {
            const string Template = @"

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public Task ###() { }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [Test]
        public void Code_gets_fixed_by_removing_async_suffix_from_snake_case_method_name_(
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

            VerifyCSharpFix(template.Replace("###", "Do_something_async"), template.Replace("###", "Do_something"));
        }

        protected override string GetDiagnosticId() => MiKo_1118_TestMethodsButAsyncSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1118_TestMethodsButAsyncSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1118_CodeFixProvider();
    }
}
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1046_AsyncMethodsSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] TaskFactoryMethods = typeof(TaskFactory).GetMethods().Concat(typeof(TaskFactory<int>).GetMethods()).ToHashSet(_ => _.Name).ToArray();

        [Test]
        public void No_issue_is_reported_for_non_async_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_static_async_Main_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static async Task Main() { }
}
");

        [Test]
        public void An_issue_is_reported_for_non_static_async_Main_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async Task Main() { }
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
        public void No_issue_is_reported_for_correctly_named_async_void_core_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    protected async void DoSomethingAsyncCore() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_async_void_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async void DoSomething() { }
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
        public void No_issue_is_reported_for_correctly_named_Task_core_method() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    protected Task DoSomethingAsyncCore() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_Task_method() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method()
            => Assert.Multiple(() =>
                                    {
                                        foreach (var testFixture in TestFixtures)
                                        {
                                            foreach (var test in Tests)
                                            {
                                                No_issue_is_reported_for(@"
using NUnit;
using System.Threading.Tasks;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public Task DoSomething() => Task.CompletedTask;
}
");
                                            }
                                        }
                                    });

        [Test]
        public void No_issue_is_reported_for_TaskFactory_method_([ValueSource(nameof(TaskFactoryMethods))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_async_local_function() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        void DoSomethingCore() { }
    }
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
        public void An_issue_is_reported_for_incorrectly_named_async_void_local_function() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async void DoSomethingAsync()
    {
        async void DoSomethingCore() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_Task_local_function() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomethingAsync()
    {
        async Task DoSomethingCoreAsync() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_Task_local_function() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomethingAsync()
    {
        async Task DoSomethingCore() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_local_function_inside_test_method()
            => Assert.Multiple(() =>
                                    {
                                        foreach (var testFixture in TestFixtures)
                                        {
                                            foreach (var test in Tests)
                                            {
                                                No_issue_is_reported_for(@"
using NUnit;
using System.Threading.Tasks;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public Task DoSomething()
    {
        Task DoSomethingCoreAsync() => Task.CompletedTask;
    }
}
");
                                            }
                                        }
                                    });

        [Test]
        public void An_issue_is_reported_for_correctly_named_local_function_inside_test_method()
            => Assert.Multiple(() =>
                                    {
                                        foreach (var testFixture in TestFixtures)
                                        {
                                            foreach (var test in Tests)
                                            {
                                                An_issue_is_reported_for(@"
using NUnit;
using System.Threading.Tasks;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public Task DoSomething()
    {
        Task DoSomethingCore() => Task.CompletedTask;
    }
}
");
                                            }
                                        }
                                    });

        [Test]
        public void Code_gets_fixed_for_method() => VerifyCSharpFix(
                                                                "using System.Threading.Tasks; class TestMe { Task DoSomething() { } }",
                                                                "using System.Threading.Tasks; class TestMe { Task DoSomethingAsync() { } }");

        [Test]
        public void Code_gets_fixed_for_local_function() => VerifyCSharpFix(
                                                                        "using System.Threading.Tasks; class TestMe { Task DoSomethingAsync() { Task Core() { } } }",
                                                                        "using System.Threading.Tasks; class TestMe { Task DoSomethingAsync() { Task CoreAsync() { } } }");

        protected override string GetDiagnosticId() => MiKo_1046_AsyncMethodsSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1046_AsyncMethodsSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1046_CodeFixProvider();
    }
}
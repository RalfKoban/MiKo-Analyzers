using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6063_InvocationIsOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_invocation_is_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject.DoSomething(1, 2, 3)
                        .ConfigureAwait(false);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_invocation_on_property_is_on_other_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject.SomeProperty
            .DoSomething(1, 2, 3)
            .ConfigureAwait(false);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_invocation_is_on_different() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await MyCallback(async my =>
                                     {
                                        await my.DoSomething(1, 2, 3).ConfigureAwait(false);
                                        await my.DoSomething(4, 5, 6).ConfigureAwait(false);
                                        await my.DoSomething(7, 8, 9).ConfigureAwait(false);
                                     }).ConfigureAwait(false);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;

    private async Task MyCallback(Func<TestMe, Task> callback) => await callback(null);
}
");

        [Test]
        public void An_issue_is_reported_if_invocation_is_on_different_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject
                   .DoSomething(1, 2, 3)
                   .ConfigureAwait(false);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_if_invocation_on_method_is_on_different_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public async void DoSomething(int x, int y, int z)
    {
        await DoSomething(1, 2, 3)
                         .ConfigureAwait(false);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void Code_gets_fixed_if_invocation_is_on_different_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject
                    .DoSomething(1, 2, 3)
                    .ConfigureAwait(false);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject.DoSomething(1, 2, 3)
                    .ConfigureAwait(false);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_invocation_is_on_different_line_inside_lambda()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(TestMe someObject)
    {
        DoSomethingCore(_ =>
                             {
                                 TestMe
                                    .DoSomething(1, 2, 3)
                                    .ConfigureAwait(false);
                             });
    }

    private static Task DoSomething(int i, int j, int k) => Task.CompletedTask;

    private void DoSomethingCore(Action<object> callback) { }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(TestMe someObject)
    {
        DoSomethingCore(_ =>
                             {
                                 TestMe.DoSomething(1, 2, 3)
                                    .ConfigureAwait(false);
                             });
    }

    private static Task DoSomething(int i, int j, int k) => Task.CompletedTask;

    private void DoSomethingCore(Action<object> callback) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_invocation_on_method_is_on_different_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public async void DoSomething(int x, int y, int z)
    {
        await DoSomething(1, 2, 3)
                         .ConfigureAwait(false);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            const string FixedCode = @"
public class TestMe
{
    public async void DoSomething(int x, int y, int z)
    {
        await DoSomething(1, 2, 3).ConfigureAwait(false);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6063_InvocationIsOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6063_InvocationIsOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6063_CodeFixProvider();
    }
}
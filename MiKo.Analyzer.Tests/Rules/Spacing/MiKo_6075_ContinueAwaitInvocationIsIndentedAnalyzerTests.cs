using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6075_ContinueAwaitInvocationIsIndentedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_ConfigureAwait_is_on_same_line_as_invocation() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync(TestMe someObject)
    {
        await someObject.DoSomethingAsync(1, 2, 3).ConfigureAwait(false);
    }

    private Task DoSomethingAsync(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_ConfigureAwait_is_on_another_line_as_invocation() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync(TestMe someObject)
    {
        await someObject.DoSomethingAsync(1, 2, 3)
                        .ConfigureAwait(false);
    }

    private Task DoSomethingAsync(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_configured_invocation_spans_multiple_lines_and_ConfigureAwait_is_on_other_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync(TestMe someObject)
    {
        await someObject.DoSomethingAsync([1,
                                           2,
                                           3])
                        .ConfigureAwait(false);
    }

    private Task DoSomethingAsync(params int[] indices) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_awaited_Task_Run_spans_multiple_lines_and_ConfigureAwait_is_on_other_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
        await Task.Run(
                       () =>
                           {
                               // do stuff
                           })
                  .ConfigureAwait(false);
    }
}
");

        [Test]
        public void No_issue_is_reported_if_awaited_call_with_object_initializer_spans_multiple_lines_and_ConfigureAwait_is_on_other_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public int Value { get; set; }

    public async Task DoSomethingAsync()
    {
        await DoStuff(new TestMe
                          {
                              Value = 42,
                          })
                  .ConfigureAwait(false);
    }

    private Task DoStuff(TestMe value) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_for_user_defined_ConfigureAwait_on_multi_line_parenthesized_this_expression() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething()
    {
        var configured = (
                          this).ConfigureAwait(false);
    }

    private TestMe ConfigureAwait(bool continueOnCapturedContext) => this;
}
");

        [Test]
        public void An_issue_is_reported_if_configured_invocation_spans_multiple_lines_and_ConfigureAwait_is_on_same_line() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync(TestMe someObject)
    {
        await someObject.DoSomethingAsync([1,
                                           2,
                                           3]).ConfigureAwait(false);
    }

    private Task DoSomethingAsync(params int[] indices) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_if_awaited_Task_Run_spans_multiple_lines_and_ConfigureAwait_is_on_same_line() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
        await Task.Run(
                       () =>
                           {
                               // do stuff
                           }).ConfigureAwait(false);
    }
}
");

        [Test]
        public void An_issue_is_reported_if_awaited_call_with_object_initializer_spans_multiple_lines_and_ConfigureAwait_is_on_same_line() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public int Value { get; set; }

    public async Task DoSomethingAsync()
    {
        await DoStuff(new TestMe
                          {
                              Value = 42,
                          }).ConfigureAwait(false);
    }

    private Task DoStuff(TestMe value) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_for_unqualified_generic_invocation_spanning_multiple_lines_and_ConfigureAwait_on_same_line_as_end() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
        await DoStuffAsync<int>(
                                1).ConfigureAwait(false);
    }

    private Task DoStuffAsync<T>(T value) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_for_generic_invocation_with_identifier_argument_and_ConfigureAwait_on_same_line_as_end() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync(int value)
    {
        await DoStuffAsync<int>(
                                value).ConfigureAwait(false);
    }

    private Task DoStuffAsync<T>(T value) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_for_non_awaited_ConfigureAwait_on_same_line_as_end_of_multi_line_invocation() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(int value)
    {
        var configured = DoStuffAsync(
                                      value).ConfigureAwait(false);
    }

    private Task DoStuffAsync(int value) => Task.CompletedTask;
}
");

        [Test]
        public void Code_gets_fixed_if_configured_invocation_spans_multiple_lines_and_ConfigureAwait_is_on_same_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync(TestMe someObject)
    {
        await someObject.DoSomethingAsync([1,
                                           2,
                                           3]).ConfigureAwait(false);
    }

    private Task DoSomethingAsync(params int[] indices) => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync(TestMe someObject)
    {
        await someObject.DoSomethingAsync([1,
                                           2,
                                           3])
                        .ConfigureAwait(false);
    }

    private Task DoSomethingAsync(params int[] indices) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_awaited_Task_Run_spans_multiple_lines_and_ConfigureAwait_is_on_same_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
        await Task.Run(
                       () =>
                           {
                               // do stuff
                           }).ConfigureAwait(false);
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
        await Task.Run(
                       () =>
                           {
                               // do stuff
                           })
                  .ConfigureAwait(false);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_awaited_call_with_object_initializer_spans_multiple_lines_and_ConfigureAwait_is_on_same_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public int Value { get; set; }

    public async Task DoSomethingAsync()
    {
        await DoStuff(new TestMe
                          {
                              Value = 42,
                          }).ConfigureAwait(false);
    }

    private Task DoStuff(TestMe value) => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public int Value { get; set; }

    public async Task DoSomethingAsync()
    {
        await DoStuff(new TestMe
                          {
                              Value = 42,
                          })
                 .ConfigureAwait(false);
    }

    private Task DoStuff(TestMe value) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_unqualified_generic_invocation_spanning_multiple_lines_and_ConfigureAwait_on_same_line_as_end()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
        await DoStuffAsync<int>(
                                1).ConfigureAwait(false);
    }

    private Task DoStuffAsync<T>(T value) => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
        await DoStuffAsync<int>(
                                1)
                           .ConfigureAwait(false);
    }

    private Task DoStuffAsync<T>(T value) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_generic_invocation_with_identifier_argument_and_ConfigureAwait_on_same_line_as_end()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync(int value)
    {
        await DoStuffAsync<int>(
                                value).ConfigureAwait(false);
    }

    private Task DoStuffAsync<T>(T value) => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync(int value)
    {
        await DoStuffAsync<int>(
                                value)
                           .ConfigureAwait(false);
    }

    private Task DoStuffAsync<T>(T value) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_awaited_ConfigureAwait_on_same_line_as_end_of_multi_line_invocation()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(int value)
    {
        var configured = DoStuffAsync(
                                      value).ConfigureAwait(false);
    }

    private Task DoStuffAsync(int value) => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(int value)
    {
        var configured = DoStuffAsync(
                                      value)
                                 .ConfigureAwait(false);
    }

    private Task DoStuffAsync(int value) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_ConfigureAwait_is_on_same_line_and_preceded_by_whitespace()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
        await Task.Run(
                       () =>
                           {
                               // do stuff
                           })   .ConfigureAwait(false);
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task DoSomethingAsync()
    {
        await Task.Run(
                       () =>
                           {
                               // do stuff
                           })   
                  .ConfigureAwait(false);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6075_ContinueAwaitInvocationIsIndentedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6075_ContinueAwaitInvocationIsIndentedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6075_CodeFixProvider();
    }
}
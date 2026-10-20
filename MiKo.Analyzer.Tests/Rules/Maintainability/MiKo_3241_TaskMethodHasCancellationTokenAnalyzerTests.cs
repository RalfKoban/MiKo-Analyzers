using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3241_TaskMethodHasCancellationTokenAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] TestRelatedAttributes = Enumerable.Empty<string>()
                                                                           .Concat(Tests)
                                                                           .Concat(TestSetUps)
                                                                           .Concat(TestTearDowns)
                                                                           .Concat(TestOneTimeSetUps)
                                                                           .Concat(TestOneTimeTearDowns)
                                                                           .Concat(TestAssemblySetUps)
                                                                           .Concat(TestAssemblyTearDowns)
                                                                           .ToArray();

        [Test]
        public void No_issue_is_reported_for_generic_Task_method_with_cancellation_token() => No_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public Task<T> DoSomething<T>(T value, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_generic_ValueTask_method_with_cancellation_token() => No_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public ValueTask<T> DoSomething<T>(T value, CancellationToken cancellationToken)
    {
        return default;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_without_cancellation_token_parameter_if_an_overload_has_it() => No_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(int value)
    {
        return Task.CompletedTask;
    }

    public Task DoSomething(int value, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_generic_Task_method_with_cancellation_token() => No_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(int value, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_generic_ValueTask_method_with_cancellation_token() => No_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public ValueTask DoSomething(int value, CancellationToken cancellationToken)
    {
        return default;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_Task_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething()
    {
        return null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Task_test_method_without_cancellation_token_parameter_([ValueSource(nameof(TestRelatedAttributes))] string test) => No_issue_is_reported_for(@"
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public Task DoSomething(int value)
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Task_test_method_without_parameters_([ValueSource(nameof(TestRelatedAttributes))] string test) => No_issue_is_reported_for(@"
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public Task DoSomething()
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ValueTask_method_without_cancellation_token_parameter_if_an_overload_has_it() => No_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public ValueTask DoSomething(int value)
    {
        return default;
    }

    public ValueTask DoSomething(int value, CancellationToken cancellationToken)
    {
        return default;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ValueTask_test_method_without_cancellation_token_parameter_([ValueSource(nameof(TestRelatedAttributes))] string test) => No_issue_is_reported_for(@"
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public ValueTask DoSomething(int value)
    {
        return default;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ValueTask_test_method_without_parameters_([ValueSource(nameof(TestRelatedAttributes))] string test) => No_issue_is_reported_for(@"
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public ValueTask DoSomething()
    {
        return default;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property() => No_issue_is_reported_for(@"
public class TestMe
{
    public Task Completion
    {
        get
        {
            return Task.CompletedTask;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() {}
}
");

        [Test]
        public void No_issue_is_reported_for_Main_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public static Task Main() {}
}
");

        [Test]
        public void No_issue_is_reported_for_interface_implementation() => No_issue_is_reported_for(@"
#pragma warning disable MiKo_3241

public interface ITestMe
{
    Task DoSomethingAsync();
}

#pragma warning restore MiKo_3241

public class TestMe : ITestMe
{
    public Task DoSomethingAsync() {}
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Task_method_without_any_parameter() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task<int> DoSomething()
    {
        return Task.FromResult(42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Task_method_without_cancellation_token() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task<T> DoSomething<T>(T value)
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_generic_ValueTask_method_without_any_parameter() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public ValueTask<int> DoSomething()
    {
        return default;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_generic_ValueTask_method_without_cancellation_token() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public ValueTask<T> DoSomething<T>(T value)
    {
        return default;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_without_cancellation_token_parameter_if_no_overload_has_it() => An_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(int value)
    {
        return Task.CompletedTask;
    }

    public Task DoSomethingElse(int value, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_without_cancellation_token_parameter_if_no_overload_with_Task_has_it() => An_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(int value)
    {
        return Task.CompletedTask;
    }

    public void DoSomething(int value, CancellationToken cancellationToken)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_generic_Task_method_without_any_parameter() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_generic_Task_method_without_cancellation_token() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething(int value)
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_generic_ValueTask_method_without_any_parameter() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public ValueTask DoSomething()
    {
        return default;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_generic_ValueTask_method_without_cancellation_token() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public ValueTask DoSomething(int value)
    {
        return default;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ValueTask_method_without_cancellation_token_parameter_if_no_overload_has_it() => An_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public ValueTask DoSomething(int value)
    {
        return default;
    }

    public ValueTask DoSomethingElse(int value, CancellationToken cancellationToken)
    {
        return default;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ValueTask_method_without_cancellation_token_parameter_if_no_overload_with_ValueTask_has_it() => An_issue_is_reported_for(@"
using System.Threading;
using System.Threading.Tasks;

public class TestMe
{
    public ValueTask DoSomething(int value)
    {
        return default;
    }

    public void DoSomething(int value, CancellationToken cancellationToken)
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3241_TaskMethodHasCancellationTokenAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3241_TaskMethodHasCancellationTokenAnalyzer();
    }
}
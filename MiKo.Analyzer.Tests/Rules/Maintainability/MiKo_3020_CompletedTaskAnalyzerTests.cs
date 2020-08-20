using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3020_CompletedTaskAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_that_returns_no_task() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_a_generic_task() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task<int> DoSomething() => Task.FromResult(42);
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_a_completed_task() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_for_a_method_that_returns_a_completed_task_from_a_result() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.FromResult(42);
}
");

        [Test]
        public void An_issue_is_reported_for_a_strangely_formatted_method_that_returns_a_completed_task_from_a_result() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task
                                    .FromResult(42);
}
");

        [Test]
        public void An_issue_is_reported_for_a_method_that_creates_and_returns_a_completed_task_from_a_result() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        var result = Task.FromResult(42);
        return result;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_strangely_formatted_method_that_creates_and_returns_a_completed_task_from_a_result() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        var result = Task
                        .FromResult(42);
        return result;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_setup_method_for_a_mock() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        Task<int> DoSomeStuffAsync();
    }

    public class TestMe
    {
        public Task DoSomething()
        {
            var mock = new Mock<ITestMe>();
            mock.Setup(_ => _.DoSomeStuffAsync()).Returns(Task.FromResult(42));
            
            return Task.CompletedTask;
        }
    }
}");

        [Test]
        public void Code_gets_fixed_when_on_same_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.FromResult(42);
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_oassigned_to_variable_and_strangely_formatted()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        var result = Task
                        .FromResult(42);
        return result;
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething()
    {
        var result = Task.CompletedTask;
        return result;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3020_CompletedTaskAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3020_CompletedTaskAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3020_CodeFixProvider();
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3022_TaskReturnsIEnumerableAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_that_returns_a_non_generic_task() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.FromResult(42);
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_a_generic_non_IEnumerable_task() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public Task<int> DoSomething() => Task.FromResult(42);
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_a_generic_IList_task() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    public Task<IList<int>> DoSomething() => Task.FromResult(new List<int>());
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_an_IEnumerable_task() => An_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Threading.Tasks;

public class TestMe
{
    public Task<IEnumerable> DoSomething() => Task.FromResult(new ArrayList());
}
");

        [Test]
        public void No_issue_is_reported_for_overridden_method_that_returns_an_IEnumerable_task()
        {
            const string Code = @"
using System;
using System.Collections;
using System.Threading.Tasks;

public class TestMeBase
{
    public virtual Task<IEnumerable> DoSomething() => Task.FromResult(new ArrayList());
}

public class TestMe : TestMeBase
{
    public override Task<IEnumerable> DoSomething() => Task.FromResult(new ArrayList());
}
";

            An_issue_is_reported_for(Code); // it's only a single issue on the base class
        }

        [Test]
        public void No_issue_is_reported_for_interface_implementation_method_that_returns_an_IEnumerable_task()
        {
            const string Code = @"
using System;
using System.Collections;
using System.Threading.Tasks;

public interface ITestMe
{
    Task<IEnumerable> DoSomething();
}

public class TestMe : ITestMe
{
    public Task<IEnumerable> DoSomething() => Task.FromResult(new ArrayList());
}
";

            An_issue_is_reported_for(Code); // it's only a single issue on the interface
        }

        [Test]
        public void An_issue_is_reported_for_method_that_returns_a_generic_IEnumerable_task() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    public Task<IEnumerable<int>> DoSomething() => Task.FromResult(new List<int>());
}
");

        protected override string GetDiagnosticId() => MiKo_3022_TaskReturnsIEnumerableAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3022_TaskReturnsIEnumerableAnalyzer();
    }
}
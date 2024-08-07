using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3119_TestMethodsDoNotReturnCompletedTaskAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_method_that_returns_completed_Task() => No_issue_is_reported_for(@"
using System.Collections.Generic;
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
        public void No_issue_is_reported_for_expression_body_non_test_method_that_returns_completed_Task() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_method() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_that_returns_generic_Task() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public Task<int> DoSomething()
    {
        return Task.FromResult(42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_expression_body_test_method_that_returns_generic_Task() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public Task<int> DoSomething() => Task.FromResult(42);
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_that_returns_Task() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public Task DoSomething()
    {
        return Task.Run(() => { });
    }
}
");

        [Test]
        public void No_issue_is_reported_for_expression_body_test_method_that_returns_Task() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public Task DoSomething() => Task.Run(() => { });
}
");

        [Test]
        public void No_issue_is_reported_for_async_test_method_that_returns_generic_Task() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public async Task<int> DoSomething()
    {
        return await Task.FromResult(42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_expression_body_async_test_method_that_returns_generic_Task() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public async Task<int> DoSomething() => await Task.FromResult(42);
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_that_returns_completed_Task() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public Task DoSomething()
    {
        return Task.CompletedTask;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_expression_body_test_method_that_returns_completed_Task() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public Task DoSomething() => Task.CompletedTask;
}
");

        [Test]
        public void Code_gets_fixed_for_test_method_that_returns_completed_Task()
        {
            const string OriginalCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    private Task _dummyTask;

    [Test]
    public Task DoSomething()
    {
        Assert.Fail(""Fix me"");

        return Task.CompletedTask;
    }
}
";

            const string FixedCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    private Task _dummyTask;

    [Test]
    public void DoSomething()
    {
        Assert.Fail(""Fix me"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_expression_body_test_method_that_returns_completed_Task()
        {
            const string OriginalCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public Task DoSomething() => Task.CompletedTask;
}

public class TestMe2
{
    private Task _dummyTask;
}
";

            const string FixedCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
}

public class TestMe2
{
    private Task _dummyTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_expression_body_test_method_that_returns_completed_Task_positioned_at_begin()
        {
            const string OriginalCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public Task DoSomething() => Task.CompletedTask;

    [Test]
    public void Test1() => Assert.Fail(""Fix me"");

    [Test]
    public void Test2() => Assert.Fail(""Fix me"");

    private Task _dummyTask;
}
";

            const string FixedCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public void Test1() => Assert.Fail(""Fix me"");

    [Test]
    public void Test2() => Assert.Fail(""Fix me"");

    private Task _dummyTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_expression_body_test_method_that_returns_completed_Task_positioned_in_the_middle()
        {
            const string OriginalCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    private Task _dummyTask;

    [Test]
    public void Test1() => Assert.Fail(""Fix me"");

    [Test]
    public Task DoSomething() => Task.CompletedTask;

    [Test]
    public void Test2() => Assert.Fail(""Fix me"");
}
";

            const string FixedCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    private Task _dummyTask;

    [Test]
    public void Test1() => Assert.Fail(""Fix me"");

    [Test]
    public void Test2() => Assert.Fail(""Fix me"");
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_expression_body_test_method_that_returns_completed_Task_positioned_at_the_end()
        {
            const string OriginalCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    private Task _dummyTask;

    [Test]
    public void Test1() => Assert.Fail(""Fix me"");

    [Test]
    public void Test2() => Assert.Fail(""Fix me"");

    [Test]
    public Task DoSomething() => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    private Task _dummyTask;

    [Test]
    public void Test1() => Assert.Fail(""Fix me"");

    [Test]
    public void Test2() => Assert.Fail(""Fix me"");
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3119_TestMethodsDoNotReturnCompletedTaskAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3119_TestMethodsDoNotReturnCompletedTaskAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3119_CodeFixProvider();
    }
}
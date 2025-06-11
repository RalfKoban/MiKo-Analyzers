using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6065_InvocationIsIndentedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_invocation_on_object_is_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject.DoSomething(1, 2, 3);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_invocation_on_object_is_indented_on_other_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject
                    .DoSomething(1, 2, 3);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_if_invocation_on_object_is_outdented_on_other_line() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject
            .DoSomething(1, 2, 3);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_invocation_on_object_is_on_same_line_in_expression_body() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject) => await someObject.DoSomething(1, 2, 3);

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_invocation_on_object_is_indented_on_other_line_in_expression_body() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject) => await someObject
                                                                    .DoSomething(1, 2, 3);

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_if_invocation_on_object_is_outdented_on_other_line_in_expression_body() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject) => await someObject
                                                            .DoSomething(1, 2, 3);

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_invocation_on_return_value_is_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42).DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_invocation_on_return_value_is_indented_on_other_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42)
                    .DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_if_invocation_on_return_value_is_outdented_on_other_line() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42)
            .DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_conditional_invocation_on_object_is_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject?.DoSomething(1, 2, 3);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_conditional_invocation_on_object_is_indented_on_other_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject
                    ?.DoSomething(1, 2, 3);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_if_conditional_invocation_on_object_is_outdented_on_other_line() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject
            ?.DoSomething(1, 2, 3);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_conditional_invocation_on_return_value_is_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42)?.DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void No_issue_is_reported_if_conditional_invocation_on_return_value_is_indented_on_other_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42)
                    ?.DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void An_issue_is_reported_if_conditional_invocation_on_return_value_is_outdented_on_other_line() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42)
            ?.DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
");

        [Test]
        public void Code_gets_fixed_if_invocation_on_object_is_outdented_on_other_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject
            .DoSomething(1, 2, 3);
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
        await someObject
                    .DoSomething(1, 2, 3);
    }

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_invocation_on_object_is_outdented_on_other_line_in_expression_body()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject) => await someObject
                                                            .DoSomething(1, 2, 3);

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject) => await someObject
                                                                    .DoSomething(1, 2, 3);

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_invocation_on_return_value_is_outdented_on_other_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42)
            .DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42)
                  .DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_conditional_invocation_on_object_is_outdented_on_other_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething(TestMe someObject)
    {
        await someObject
            ?.DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

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
        await someObject
                    ?.DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_conditional_invocation_on_return_value_is_outdented_on_other_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42)
            ?.DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public async void DoSomething()
    {
        await CreateMe(42)
                  ?.DoSomething(1, 2, 3);
    }

    private TestMe CreateMe(int unused) => new TestMe();

    private Task DoSomething(int i, int j, int k) => Task.CompletedTask;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6065_InvocationIsIndentedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6065_InvocationIsIndentedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6065_CodeFixProvider();
    }
}
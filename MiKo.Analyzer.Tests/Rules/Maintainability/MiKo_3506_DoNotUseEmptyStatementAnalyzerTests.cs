using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3506_DoNotUseEmptyStatementAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_if_statement_with_block_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            DoSomething(true);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_if_statement_with_statement_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
            DoSomething(true);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_if_statement_with_empty_statement_as_body() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag);
            DoSomething(true);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_else_statement_with_block_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            DoSomething(true);
        }
        else
        {
            DoSomething(false);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_else_statement_with_statement_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
            DoSomething(true);
        else
            DoSomething(false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_else_statement_with_empty_statement_as_body() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
            DoSomething(true);
        else;
            DoSomething(false);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_statement_with_block_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int[] values)
    {
        foreach (var x in values)
        {
            DoSomething(values);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_statement_with_statement_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int[] values)
    {
        foreach (var x in values)
            DoSomething(values);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_statement_with_empty_statement_as_body() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int[] values)
    {
        foreach (var x in values);
            DoSomething(values);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_for_statement_with_block_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        for (var i = 0; i < 10; i++)
        {
            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_for_statement_with_statement_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        for (var i = 0; i < 10; i++)
            DoSomething();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_for_statement_with_empty_statement_as_body() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        for (var i = 0; i < 10; i++);
            DoSomething();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_while_statement_with_block_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while (flag)
        {
            DoSomething(false);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_while_statement_with_statement_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while (flag)
            DoSomething(false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_while_statement_with_empty_statement_as_body() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while (flag);
            DoSomething(false);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_do_statement_with_block_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        do
        {
            DoSomething(false);
        }
        while (flag);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_do_statement_with_statement_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        do
            DoSomething(false);
        while (flag);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_do_statement_with_empty_statement_as_body() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        do; while (flag);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_declaration_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        using var resource = GetResource();
            DoSomething();
    }

    private IDisposable GetResource() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_using_statement_with_block_as_body() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        using (var resource = GetResource())
        {
            DoSomething();
        }
    }

    private IDisposable GetResource() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_using_statement_with_statement_as_body() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        using (var resource = GetResource())
            DoSomething();
    }

    private IDisposable GetResource() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_using_statement_with_empty_statement_as_body() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        using (var resource = GetResource());
            DoSomething();
    }

    private IDisposable GetResource() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_lock_statement_with_block_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    private readonly object _lock = new object();

    public void DoSomething()
    {
        lock (_lock)
        {
            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_lock_statement_with_statement_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    private readonly object _lock = new object();

    public void DoSomething()
    {
        lock (_lock)
            DoSomething();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_lock_statement_with_empty_statement_as_body() => An_issue_is_reported_for(@"
public class TestMe
{
    private readonly object _lock = new object();

    public void DoSomething()
    {
        lock (_lock);
            DoSomething();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_fixed_statement_with_block_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public unsafe void DoSomething(int[] values)
    {
        fixed (int* pointer = values)
        {
            DoSomething(values);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_fixed_statement_with_statement_as_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public unsafe void DoSomething(int[] values)
    {
        fixed (int* pointer = values)
            DoSomething(values);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_fixed_statement_with_empty_statement_as_body() => An_issue_is_reported_for(@"
public class TestMe
{
    public unsafe void DoSomething(int[] values)
    {
        fixed (int* pointer = values);
            DoSomething(values);
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3506_DoNotUseEmptyStatementAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3506_DoNotUseEmptyStatementAnalyzer();
    }
}
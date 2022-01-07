using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3017_DoNotSwallowExceptionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_normal_created_object() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = new object();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_created_exception_with_inner_exception_containing_the_caught_exception() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        try
        {
            return o.ToString();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(""something went wrong here"", ex);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_created_exception_with_inner_exception() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(Task task)
    {
        if (task.Exception != null)
            throw new InvalidOperationException(""something went wrong here"", task.Exception);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_created_exception_without_inner_exception() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(Task task)
    {
        if (task.Exception != null)
            throw new InvalidOperationException(""something went wrong here"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_created_exception_without_inner_exception_in_catch_block_without_exception() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public string DoSomething(object o)
    {
        try
        {
            return o.ToString();
        }
        catch
        {
            throw new InvalidOperationException(""something went wrong here"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_created_exception_without_inner_exception_in_catch_block_with_ExceptionType_only() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public string DoSomething(object o)
    {
        try
        {
            return o.ToString();
        }
        catch (NotSupportedException)
        {
            throw new InvalidOperationException(""something went wrong here"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_created_exception_without_inner_exception_in_catch_block_with_ignored_exception_instance() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public string DoSomething(object o)
    {
        try
        {
            return o.ToString();
        }
        catch (NotSupportedException ex)
        {
            throw new InvalidOperationException(""something went wrong here"");
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3017_DoNotSwallowExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3017_DoNotSwallowExceptionAnalyzer();
    }
}
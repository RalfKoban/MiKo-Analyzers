using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3213_PublicDisposeMethodInvokesNonPublicDisposeMethodAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_without_Dispose_methods() => No_issue_is_reported_for(@"
public class TestMe1
{
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_parameterless_Dispose_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_parameterless_Dispose_method_that_has_not_Disposing_dispose_method_but_invokes_other_code() => No_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose()
    {
        DoSomething();
    }

    private void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_Dispose_disposing_method_where_Dispose_method_invokes_Dispose_disposing() => No_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose()
    {
        Dispose(true);
    }

    protected void Dispose(bool disposing)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_Dispose_disposing_method_where_Dispose_method_invokes_Dispose_disposing_and_GC_SuppressFinalize() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    ~TestMe()
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_Dispose_disposing_method_where_Dispose_method_invokes_Dispose_disposing_as_expression_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose() => Dispose(true);

    protected void Dispose(bool disposing)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_Dispose_disposing_method_where_Dispose_method_does_not_invoke_anything() => An_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose()
    {
    }

    protected void Dispose(bool disposing)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_Dispose_disposing_method_where_Dispose_method_invokes_Dispose_disposing_with_false() => An_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose()
    {
        Dispose(false);
    }

    protected void Dispose(bool disposing)
    {
    }

    private void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_Dispose_disposing_method_where_Dispose_method_invokes_something_other_than_Dispose_disposing() => An_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose()
    {
        DoSomething();
    }

    protected void Dispose(bool disposing)
    {
    }

    private void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_Dispose_disposing_method_where_Dispose_method_invokes_something_more_than_Dispose_disposing() => An_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose()
    {
        DoSomething();
        Dispose(true);
    }

    protected void Dispose(bool disposing)
    {
    }

    private void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_Dispose_disposing_method_where_Dispose_method_invokes_Dispose_disposing_with_false_as_expression_body() => An_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose() => Dispose(false);

    protected void Dispose(bool disposing)
    {
    }

    private void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_Dispose_disposing_method_where_Dispose_method_invokes_other_than_Dispose_disposing_as_expression_body() => An_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose() => DoSomething();

    protected void Dispose(bool disposing)
    {
    }

    private void DoSomething()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3213_PublicDisposeMethodInvokesNonPublicDisposeMethodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3213_PublicDisposeMethodInvokesNonPublicDisposeMethodAnalyzer();
    }
}
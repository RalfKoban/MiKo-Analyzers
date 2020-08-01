using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1200_ExceptionCatchBlockAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_catch_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        try
        {
        }
        catch
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_unnamed_exception_in_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        try
        {
        }
        catch (Exception)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_exception_in_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        try
        {
        }
        catch (Exception ex)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_exception_in_catch_block() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        try
        {
        }
        catch (Exception exception)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incomplete_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        try
        {
        }
        catch (
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incomplete_exception_in_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        try
        {
        }
        catch (Exception 
        {
        }
    }
}
");

        [Test]
        public void Fix_can_be_made() => VerifyCSharpFix(
                         @"class TestMe { void DoSomething() { try { } catch (System.Exception e) { System.Diagnostics.Trace.Write(e.Message); } } }",
                         @"class TestMe { void DoSomething() { try { } catch (System.Exception ex) { System.Diagnostics.Trace.Write(ex.Message); } } }");

        protected override string GetDiagnosticId() => MiKo_1200_ExceptionCatchBlockAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1200_ExceptionCatchBlockAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1200_CodeFixProvider();
    }
}
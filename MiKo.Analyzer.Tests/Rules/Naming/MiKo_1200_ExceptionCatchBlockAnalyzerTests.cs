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
        public void No_issue_is_reported_empty_catch_block() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_unnamed_exception_in_catch_block() => No_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_1200_ExceptionCatchBlockAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1200_ExceptionCatchBlockAnalyzer();
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3090_FinallyBlockThrowsExceptionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_method_without_throw_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
        try
        {
            DoSomething();
        }
        finally
        {
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_throw_statement_in_code_block_of_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
        throw new Exception();
    }
}");

        [Test]
        public void No_issue_is_reported_for_throw_statement_in_try_block_of_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
        try
        {
            throw new Exception();
        }
        finally
        {
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_throw_statement_in_catch_block_of_method_without_finally_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
        try
        {
        }
        catch
        {
            throw new Exception();
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_throw_statement_in_catch_block_of_method_with_finally_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
        try
        {
        }
        catch
        {
            throw new Exception();
        }
        finally
        {
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_throw_statement_in_finally_block_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
        try
        {
        }
        finally
        {
            throw new Exception();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_throw_statement_in_finally_block_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool condition)
    {
        try
        {
        }
        finally
        {
            if (condition)
                throw new Exception();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_block_throw_statement_in_finally_block_of_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool condition)
    {
        try
        {
        }
        finally
        {
            if (condition)
            {
                throw new Exception();
            }
        }
    }
}");

        protected override string GetDiagnosticId() => MiKo_3090_FinallyBlockThrowsExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3090_FinallyBlockThrowsExceptionAnalyzer();
    }
}
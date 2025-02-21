using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3503_DoNotReturnVariableImmediatelyAfterTryCatchBlockAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_when_returning_a_variable_without_any_try_catch_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        var s = o.ToString();

        return s;
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_variable_inside_try_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public string DoSomething(object o)
    {
        try
        {
            var result = DoSomething(null);

            return result;
        }
        catch
        {
           throw;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_when_returning_a_variable_directly_after_try_catch_block_that_was_not_assigned_inside_the_try_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        string result = string.Empty;
        try
        {
            o = DoSomething(null);
        }
        catch
        {
           throw;
        }

        return result;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_variable_directly_after_try_catch_block_that_was_assigned_inside_the_block() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        string result;
        try
        {
            result = DoSomething(null);
        }
        catch
        {
           throw;
        }

        return result;
    }
}
");

        [Test]
        public void An_issue_is_reported_when_returning_a_variable_directly_after_try_catch_block_that_was_assigned_inside_the_block_but_catch_block_swallows() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        string result;
        try
        {
            result = DoSomething(null);
        }
        catch
        {
        }

        return result;
    }
}
");

        [Test]
        public void Code_gets_fixed_when_returning_a_variable_directly_after_try_catch_block_that_was_assigned_inside_the_block()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        string result;
        try
        {
            result = DoSomething(null);
        }
        catch
        {
           throw;
        }

        return result;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        try
        {
            string result;
            result = DoSomething(null);

            return result;
        }
        catch
        {
           throw;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_variable_directly_after_try_catch_block_that_was_assigned_inside_the_block_and_catch_block_returns()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        string result;
        try
        {
            result = DoSomething(null);
        }
        catch
        {
           return null;
        }

        return result;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        try
        {
            string result;
            result = DoSomething(null);

            return result;
        }
        catch
        {
           return null;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_returning_a_variable_directly_after_try_catch_block_that_was_assigned_inside_the_block_but_catch_block_swallows()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        string result;
        try
        {
            result = DoSomething(null);
        }
        catch
        {
        }

        return result;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        try
        {
            string result;
            result = DoSomething(null);

            return result;
        }
        catch
        {
            string result;

            return result;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3503_DoNotReturnVariableImmediatelyAfterTryCatchBlockAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3503_DoNotReturnVariableImmediatelyAfterTryCatchBlockAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3503_CodeFixProvider();
    }
}
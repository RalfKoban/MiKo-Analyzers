using System;

using Microsoft.CodeAnalysis.CodeFixes;
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
        public void No_issue_is_reported_for_created_exception_without_inner_exception_if_there_is_no_exception() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        if (i != 42)
            throw new InvalidOperationException(""something went wrong here"");
    }
}
");

        [Test]
        public void No_issue_is_reported_for_created_exception_without_inner_exception_if_there_is_no_exception_2() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (o is null)
        {
            throw new ArgumentNullException(""something went wrong here"");
        }

        if (o.GetHashCode() != 42)
        {
            throw new ArgumentException(""something went wrong here"");
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_created_ArgumentException_without_inner_exception_even_if_there_is_an_exception_([Values(nameof(ArgumentException), nameof(ArgumentNullException), nameof(ArgumentOutOfRangeException))] string exception)
        {
            var code = @"
using System;

public class TestMe
{
    public void DoSomething(Exception ex)
    {
        if (ex is null)
        {
            throw new " + exception + @"(""something went wrong here"");
        }
    }
}
";
            No_issue_is_reported_for(code);
        }

        [Test]
        public void An_issue_is_reported_for_created_exception_without_inner_exception_from_parameter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Exception ex)
    {
        throw new InvalidOperationException(""something went wrong here"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_created_exception_without_inner_exception_from_task() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_created_exception_without_inner_exception_in_catch_block_with_exception_type_only() => An_issue_is_reported_for(@"
using System;

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
        public void An_issue_is_reported_for_created_exception_without_inner_exception_in_catch_block_with_available_but_ignored_exception_instance() => An_issue_is_reported_for(@"
using System;

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

        [Test]
        public void An_issue_is_reported_for_created_exception_with_inner_exception_containing_null_caught_exception() => An_issue_is_reported_for(@"
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
            throw new InvalidOperationException(""something went wrong here"", null);
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_created_exception_without_inner_exception_from_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(Exception ex)
    {
        throw new InvalidOperationException(""something went wrong here"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(Exception ex)
    {
        throw new InvalidOperationException(""something went wrong here"", ex);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_created_exception_without_inner_exception_from_Task()
        {
            const string OriginalCode = @"
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
";

            const string FixedCode = @"
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
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_created_exception_message_only_in_catch_block_without_exception()
        {
            const string OriginalCode = @"
using System;

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
";

            const string FixedCode = @"
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
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_created_exception_message_only_in_catch_block_with_exception_type_only()
        {
            const string OriginalCode = @"
using System;

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
";

            const string FixedCode = @"
using System;

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
            throw new InvalidOperationException(""something went wrong here"", ex);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_created_exception_message_only_in_catch_block_with_available_but_ignored_exception_instance()
        {
            const string OriginalCode = @"
using System;

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
";

            const string FixedCode = @"
using System;

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
            throw new InvalidOperationException(""something went wrong here"", ex);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_created_exception_with_null_for_inner_exception_in_catch_block_with_available_but_ignored_exception_instance()
        {
            const string OriginalCode = @"
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
            throw new InvalidOperationException(""something went wrong here"", null);
        }
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
            return o.ToString();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(""something went wrong here"", ex);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3017_DoNotSwallowExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3017_DoNotSwallowExceptionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3017_CodeFixProvider();
    }
}
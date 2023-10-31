using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2308_CommentPlacedAfterCodeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_comment_before_code() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
        DoSomething();
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_at_return_level() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        return DoSomething(); // some comment
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_at_return_level_for_arrow_expression() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething() => DoSomething(); // some comment
}");

        [Test]
        public void No_issue_is_reported_for_comment_as_only_statement() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_as_only_statement_in_catch_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        try
        {
            DoSomething();
        }
        catch (Exception ex)
        {
            // some comment
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_as_last_statement_in_array_initializer_with_colon() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        var array = new[]
                        {
                            ""something"",
                            ""something else"", // some comment
                        };
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_as_last_statement_in_array_initializer_without_colon() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        var array = new[]
                        {
                            ""something"",
                            ""something else"" // some comment
                        };
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_in_object_initializer() => No_issue_is_reported_for(@"
public class TestMe
{
    public string Name { get; set; }

    public TestMe DoSomething()
    {
        return new TestMe
                   {
                       Name = ""something"" // some comment
                   };
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_after_simple_local_declaration() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        var value = 42; // some comment
    }
}");

        [Test]
        public void No_issue_is_reported_for_comment_as_only_statement_in_catch_block_if_Resharper_comment() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        try
        {
            DoSomething();
        }
        catch (Exception ex)
        {
//// ReSharper disable once PossibleIntendedRethrow
            throw ex;
//// ReSharper restore once PossibleIntendedRethrow
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_comment_after_code() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        DoSomething();

        // some comment
    }
}");

        [Test]
        public void An_issue_is_reported_for_comment_after_code_followed_by_white_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        DoSomething();

        // some comment

    }
}");

        [Test]
        public void An_issue_is_reported_for_comment_after_code_in_catch_block() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        try
        {
            DoSomething();
        }
        catch (Exception ex)
        {
            DoSomething();

            // some comment
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_comment_after_array_initializer() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        var array = new[]
                        {
                            ""something"",
                            ""something else"",
                        }; // some comment
    }
}");

        [Test]
        public void Code_gets_fixed_for_comment_at_end_of_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        DoSomething();

        // some comment
    }
}";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
        DoSomething();
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_comment_in_catch_block()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        try
        {
            DoSomething();
        }
        catch (Exception ex)
        {
            DoSomething();

            // some comment
        }
    }
}";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        try
        {
            DoSomething();
        }
        catch (Exception ex)
        {
            // some comment
            DoSomething();
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_comment_after_array_initializer()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        var array = new[]
                        {
                            ""something"",
                            ""something else"",
                        }; // some comment
    }
}";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
        var array = new[]
                        {
                            ""something"",
                            ""something else"",
                        };
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2308_CommentPlacedAfterCodeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2308_CommentPlacedAfterCodeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2308_CodeFixProvider();
    }
}
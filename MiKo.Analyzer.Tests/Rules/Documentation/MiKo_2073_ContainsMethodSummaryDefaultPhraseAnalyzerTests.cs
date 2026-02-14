using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MethodNames = ["Contains", "ContainsKey", "ContainsValue"];

        [Test]
        public void No_issue_is_reported_for_undocumented_Contains_method_([ValueSource(nameof(MethodNames))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Contains_method_with_summary_not_starting_with_Determines_whether_([ValueSource(nameof(MethodNames))] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Contains_method_with_see_cref_not_followed_by_Determines_whether_([ValueSource(nameof(MethodNames))] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <see cref=""TestMe""/> something.
    /// </summary>
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_async_Contains_method_with_summary_not_containing_Determines_whether_([ValueSource(nameof(MethodNames))] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Asynchronously does something.
    /// </summary>
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_Contains_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Contains_method_with_summary_starting_with_Determines_whether_([ValueSource(nameof(MethodNames))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Determines whether something is there.
    /// </summary>
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_async_Contains_method_with_summary_containing_Determines_whether_([ValueSource(nameof(MethodNames))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Asynchronously determines whether something is there.
    /// </summary>
    public bool " + methodName + @"()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_to_start_summary_with_Determines_whether()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether something.
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_replace_Asynchronously_does_with_Asynchronously_determines_whether()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously does something.
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously determines whether something.
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_replace_Determines_if_with_Determines_whether()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Determines if it is there.
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether it is there.
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_replace_Checks_if_with_Determines_whether_when_dot_directly_after_XML_tag()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>.
    /// Checks if it is there.
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>.
    /// Determines whether it is there.
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_summary_with_spaces_on_same_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary> </summary>
    public bool Contains() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_summary_without_contents_on_same_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary></summary>
    public bool Contains() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_summary_without_contents_on_different_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_summary_without_leading_space()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    ///
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_summary_with_leading_space()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// 
    /// </summary>
    public bool Contains() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool Contains() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2073_CodeFixProvider();
    }
}
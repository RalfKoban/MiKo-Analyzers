using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2232_EmptySummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_empty_summary_on_single_line() => An_issue_is_reported_for(@"
/// <summary></summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_empty_summary_on_single_line() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary></summary>
    private int m_something;
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_empty_summary_on_single_line() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary></summary>
    public int Something { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_event_with_empty_summary_on_single_line() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary></summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_empty_summary_on_single_line() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary></summary>
    public void DoSomething()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_whitespace_only_summary_on_single_line() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>   </summary>
    public void DoSomething()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_empty_summary_on_separate_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// </summary>
    public void DoSomething()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_whitespace_only_summary_on_separate_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// 
    /// </summary>
    public void DoSomething()
    { }
}
");

        [Test]
        public void Code_gets_fixed_for_type_with_empty_summary_on_single_line()
        {
            const string OriginalCode = @"
/// <summary></summary>
public class TestMe
{
}
";

            const string FixedCode = @"
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_with_empty_summary_on_single_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary></summary>
    private int m_something;
}
";

            const string FixedCode = @"
public class TestMe
{
    private int m_something;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_empty_summary_on_single_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary></summary>
    public int Something { get; set; }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int Something { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_with_empty_summary_on_single_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary></summary>
    public event EventHandler MyEvent;
}
";

            const string FixedCode = @"
public class TestMe
{
    public event EventHandler MyEvent;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_empty_summary_on_single_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <summary></summary>
    public void DoSomethingElse()
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    public void DoSomethingElse()
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_whitespace_only_summary_on_single_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <summary>   </summary>
    public void DoSomethingElse()
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    public void DoSomethingElse()
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_empty_summary_on_separate_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <summary>
    /// </summary>
    public void DoSomethingElse()
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    public void DoSomethingElse()
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_whitespace_only_summary_on_separate_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <summary>
    /// 
    /// </summary>
    public void DoSomethingElse()
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    public void DoSomethingElse()
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_empty_summary_and_non_empty_returns_on_separate_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <summary>
    /// </summary>
    /// <returns>
    /// Some value
    /// </returns>
    public int DoSomethingElse() => 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <returns>
    /// Some value
    /// </returns>
    public int DoSomethingElse() => 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_empty_summary_and_non_empty_returns_on_single_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <summary></summary>
    /// <returns>Some value</returns>
    public int DoSomethingElse() => 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <returns>Some value</returns>
    public int DoSomethingElse() => 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_empty_summary_and_non_empty_returns_on_single_line_with_empty_line_between_xml_nodes()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <summary></summary>
    
    /// <returns>Some value</returns>
    public int DoSomethingElse() => 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>Some text</summary>
    public void DoSomething()
    { }

    /// <returns>Some value</returns>
    public int DoSomethingElse() => 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2232_EmptySummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2232_EmptySummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2232_CodeFixProvider();
    }
}
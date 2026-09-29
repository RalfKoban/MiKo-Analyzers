using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2246_XmlTextStartsWithMultipleSpacesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_text_with_single_gap() => No_issue_is_reported_for(@"
/// <summary>
/// Some text
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_text_with_no_gap() => No_issue_is_reported_for(@"
/// <summary>Some text</summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_code() => No_issue_is_reported_for(@"
/// <summary>
/// Some text
/// </summary>
/// <example>
/// <code>
///    This is some code.
/// </code>
/// </example>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_CDataSection() => No_issue_is_reported_for(@"
/// <summary>
/// Some text
/// </summary>
/// <example>
/// <code>
/// <![CDATA[
///    <Button>
///    </Button>
/// ]]>
/// </code>
/// </example>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_text_with_multiple_lines() => An_issue_is_reported_for(@"
/// <summary>
///    Some text
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_mixed_text_with_multiple_lines() => An_issue_is_reported_for(@"
/// <summary>
/// Some text
///     with some more text.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void Code_gets_fixed_for_text_with_multiple_lines()
        {
            const string OriginalCode = @"
/// <summary>
///     Some text
/// </summary>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Some text
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_mixed_text_with_multiple_lines()
        {
            const string OriginalCode = @"
/// <summary>
/// Some text
///     with some more text.
/// </summary>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Some text
/// with some more text.
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_trailing_whitespaces_after_summary_start_element()
        {
            const string OriginalCode = @"
/// <summary>    
/// Some text.
/// </summary>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Some text.
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_trailing_whitespaces_after_summary_end_element()
        {
            const string OriginalCode = @"
/// <summary>
/// Some text.
/// </summary>    
public class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Some text.
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2246_XmlTextStartsWithMultipleSpacesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2246_XmlTextStartsWithMultipleSpacesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2246_CodeFixProvider();
    }
}
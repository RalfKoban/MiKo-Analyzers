using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2214_DocumentationContainsEmptyLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_empty_comment() => No_issue_is_reported_for(@"
/// <summary>
/// </summary>
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_comment() => No_issue_is_reported_for(@"
/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_comment_with_empty_line() => An_issue_is_reported_for(@"
/// <summary>
/// 
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_empty_line_at_begin_of_comment() => An_issue_is_reported_for(@"
/// <summary>
/// 
/// Some summary.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_empty_line_at_middle_of_comment() => An_issue_is_reported_for(@"
/// <summary>
/// Some summary.
/// 
/// Some more summary.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_empty_line_at_end_of_comment() => An_issue_is_reported_for(@"
/// <summary>
/// Some summary.
/// 
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_non_XML_comment() => An_issue_is_reported_for(@"
/// Some text.
/// 
/// Some more text.
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_multiple_consecutive_empty_lines_at_begin_of_comment() => An_issue_is_reported_for(@"
/// <summary>
/// 
/// 
/// 
/// Some summary.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_multiple_consecutive_empty_lines_at_middle_of_comment() => An_issue_is_reported_for(@"
/// <summary>
/// Some summary.
/// 
/// 
/// 
/// Some more text.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_multiple_consecutive_empty_lines_at_end_of_comment() => An_issue_is_reported_for(@"
/// <summary>
/// Some summary.
/// 
/// 
/// 
/// </summary>
public class TestMe
{
}");

        [Test]
        public void Code_gets_fixed_for_empty_line_comment()
        {
            const string OriginalCode = @"
/// <summary>
/// 
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_line_at_begin_of_comment()
        {
            const string OriginalCode = @"
/// <summary>
/// 
/// Some summary.
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_line_at_middle_of_comment()
        {
            const string OriginalCode = @"
/// <summary>
/// Some summary.
/// 
/// Some more summary.
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Some summary.
/// <para/>
/// Some more summary.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_line_at_end_of_comment()
        {
            const string OriginalCode = @"
/// <summary>
/// Some summary.
/// 
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_empty_lines_in_comment()
        {
            const string OriginalCode = @"
/// <summary>
/// 
/// Some summary.
/// 
/// Some more summary.
/// 
/// Some even more summary.
/// 
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Some summary.
/// <para/>
/// Some more summary.
/// <para/>
/// Some even more summary.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_for_multiple_empty_lines_in_non_XML_comment()
        {
            const string OriginalCode = @"
/// 
/// Some summary.
/// 
/// Some more summary.
/// 
/// Some even more summary.
/// 
public class TestMe
{
}";

            const string FixedCode = @"
/// Some summary.
/// <para/>
/// Some more summary.
/// <para/>
/// Some even more summary.
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_for_some_more_multiple_empty_lines_in_non_XML_comment()
        {
            const string OriginalCode = @"
/// 
/// Some summary.
/// 
/// Some more summary.
/// 
/// Some even more summary.
/// 
/// Still some more summary.
/// 
/// 
/// Final summary.
public class TestMe
{
}";

            const string FixedCode = @"
/// Some summary.
/// <para/>
/// Some more summary.
/// <para/>
/// Some even more summary.
/// <para/>
/// Still some more summary.
/// <para/>
/// Final summary.
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_for_multiple_consecutive_empty_lines_at_begin_of_comment()
        {
            const string OriginalCode = @"
/// <summary>
/// 
/// 
/// 
/// Some summary.
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_for_multiple_consecutive_empty_lines_at_middle_of_comment()
        {
            const string OriginalCode = @"
/// <summary>
/// Some summary.
/// 
/// 
/// 
/// Some more text.
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Some summary.
/// <para/>
/// Some more text.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_for_multiple_consecutive_empty_lines_at_end_of_comment()
        {
            const string OriginalCode = @"
/// <summary>
/// Some summary.
/// 
/// 
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2214_DocumentationContainsEmptyLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2214_DocumentationContainsEmptyLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2214_CodeFixProvider();
    }
}
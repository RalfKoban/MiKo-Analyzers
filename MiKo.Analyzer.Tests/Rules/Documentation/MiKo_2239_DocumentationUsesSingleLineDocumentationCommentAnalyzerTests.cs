using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2239_DocumentationUsesSingleLineDocumentationCommentAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_SingleLineDocumentationComment_on_single_line() => No_issue_is_reported_for(@"
/// <summary></summary>
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_SingleLineDocumentationComment_on_separate_lines() => No_issue_is_reported_for(@"
/// <summary>
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_MultiLineDocumentationComment_on_single_line() => An_issue_is_reported_for(@"
/** */
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_MultiLineDocumentationComment_on_separate_lines() => An_issue_is_reported_for(@"
/**
*/
public class TestMe
{
}");

        [Test]
        public void Code_gets_fixed_for_MultiLineDocumentationComment_on_single_line()
        {
            const string OriginalCode = @"
/** Some comment */
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Some comment
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MultiLineDocumentationComment_on_separate_lines()
        {
            const string OriginalCode = @"
/**
* Some code
* with some text
*/
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Some code
/// with some text
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2239_DocumentationUsesSingleLineDocumentationCommentAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2239_DocumentationUsesSingleLineDocumentationCommentAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2239_CodeFixProvider();
    }
}
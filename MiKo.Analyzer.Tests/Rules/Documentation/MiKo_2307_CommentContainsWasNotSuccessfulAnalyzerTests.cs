using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2307_CommentContainsWasNotSuccessfulAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_comment() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
    }
}");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_([Values("", " ", ".", ",", ";", ":", "!", "?")] string delimiter) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // This was not successful" + delimiter + @"
    }
}");

        [TestCase("This was not successful", "This failed")]
        [TestCase("This was not successful,", "This failed,")]
        [TestCase("This was not successful;", "This failed;")]
        [TestCase("This was not successful.", "This failed.")]
        [TestCase("This was not successful!", "This failed!")]
        [TestCase("This was not successful?", "This failed?")]
        [TestCase("This result was not successful.", "This result failed.")]
        public void Code_gets_fixed_for_single_line_(string originalComment, string fixedComment)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // " + originalComment + @"
        DoSomething();
    }
}
";

            var fixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // " + fixedComment + @"
        DoSomething();
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2307_CommentContainsWasNotSuccessfulAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2307_CommentContainsWasNotSuccessfulAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2307_CodeFixProvider();
    }
}
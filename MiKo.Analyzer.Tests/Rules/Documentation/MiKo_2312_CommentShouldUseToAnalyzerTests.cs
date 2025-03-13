using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2312_CommentShouldUseToAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongTerms = Constants.Comments.WhichIsToTerms;

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
        public void An_issue_is_reported_for_wrong_documentation_([ValueSource(nameof(WrongTerms))] string term) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // Something" + term + @"do
    }
}");

        [Test]
        public void Code_gets_fixed_for_single_line_([ValueSource(nameof(WrongTerms))] string term)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // Something" + term + @"do
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // Something to do
        DoSomething();
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2312_CommentShouldUseToAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2312_CommentShouldUseToAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2312_CodeFixProvider();
    }
}
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2234_DocumentationShouldUseToAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongTerms = Constants.Comments.WhichIsToTerms;

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [TestCase("Some summary.")]
        [TestCase("Some parent.")]
        public void No_issue_is_reported_for_correct_comment_(string text) => No_issue_is_reported_for(@"
/// <summary>
/// " + text + @"
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_contraction_in_documentation_([ValueSource(nameof(WrongTerms))] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// Something" + phrase + @"do.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(WrongTerms))] string phrase)
        {
            var originalCode = @"
/// <summary>
/// Something" + phrase + @"do.
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// Something to do.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2234_DocumentationShouldUseToAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2234_DocumentationShouldUseToAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2234_CodeFixProvider();
    }
}
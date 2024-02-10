using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2213_DocumentationContainsNtContradictionAnalyzerTests : CodeFixVerifier
    {
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
        public void An_issue_is_reported_for_contradiction_in_documentation_([ValueSource(nameof(WrongContradictionPhrases))] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// This " + phrase + @" intended.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(WrongContradictionPhrases))] string phrase)
        {
            const string Template = @"
/// <summary>
/// This ### intended.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(Template.Replace("###", phrase), Template.Replace("###", ContradictionMap[phrase]));
        }

        protected override string GetDiagnosticId() => MiKo_2213_DocumentationContainsNtContradictionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2213_DocumentationContainsNtContradictionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2213_CodeFixProvider();
    }
}
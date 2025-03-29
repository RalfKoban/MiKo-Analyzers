using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2238_MakeSureToCallThisAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongTerms = ["Make sure to call this"];

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_empty_comment_on_single_line() => No_issue_is_reported_for(@"
/// <summary></summary>
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_empty_comment_on_separate_lines() => No_issue_is_reported_for(@"
/// <summary>
/// </summary>
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_whitespace_only_comment_on_single_line() => No_issue_is_reported_for(@"
/// <summary>   </summary>
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_whitespace_only_comment_on_separate_lines() => No_issue_is_reported_for(@"
/// <summary>
/// 
/// </summary>
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

        [TestCase("Some summary.")]
        [TestCase("Some parent.")]
        public void No_issue_is_reported_for_comment_starting_with_see_cref_(string text) => No_issue_is_reported_for(@"
/// <summary>
/// <see cref=""TestMe""/> " + text + @"
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_contraction_in_documentation_([ValueSource(nameof(WrongTerms))] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// " + phrase + @" to do something.
/// </summary>
public class TestMe
{
}");

        protected override string GetDiagnosticId() => MiKo_2238_MakeSureToCallThisAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2238_MakeSureToCallThisAnalyzer();
    }
}
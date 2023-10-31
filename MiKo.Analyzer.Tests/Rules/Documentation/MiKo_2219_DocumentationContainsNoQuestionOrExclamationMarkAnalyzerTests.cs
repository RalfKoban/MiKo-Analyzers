using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2219_DocumentationContainsNoQuestionOrExclamationMarkAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = { "summary", "remarks", "returns", "example", "value", "exception" };
        private static readonly string[] Markers = { "?", "!" };

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_documentation_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
/// <" + tag + ">Some text.</" + tag + @">
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_wrong_text_in_documentation_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Markers))] string marker) => An_issue_is_reported_for(@"
/// <" + tag + ">Some text" + marker + "</" + tag + @">
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_text_with_exclamation_mark_inside_important_note() => No_issue_is_reported_for(@"
/// <summary>
/// Some text.
/// </summary>
/// <remarks>
/// <note type=""important"">
/// Some warning!
/// </note>
/// </remarks>
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_text_inside_code_tag() => No_issue_is_reported_for(@"
/// <summary>
/// Some text.
/// </summary>
/// <remarks>
/// <code>
/// var o = new object();
/// var hash = o?.GetHashCode();
/// </code>
/// </remarks>
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_hyperlink() => No_issue_is_reported_for(@"
/// <summary>
/// See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-6.0.
/// </summary>
public class TestMe
{
}");

        protected override string GetDiagnosticId() => MiKo_2219_DocumentationContainsNoQuestionOrExclamationMarkAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2219_DocumentationContainsNoQuestionOrExclamationMarkAnalyzer();
    }
}
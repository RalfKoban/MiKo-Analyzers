using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_enum() => No_issue_is_reported_for(@"
public enum TestMe
{
    None = 0,
}");

        [Test]
        public void No_issue_is_reported_for_remarks_on_enum_itself() => No_issue_is_reported_for(@"
/// <remarks>
/// Some remarks.
/// </remarks>
public enum TestMe
{
    None = 0,
}");

        [Test]
        public void No_issue_is_reported_for_documented_enum_member_without_remarks() => No_issue_is_reported_for(@"
public enum TestMe
{
    /// <summary>
    /// Some summary.
    /// </summary>
    None = 0,
}");

        [Test]
        public void An_issue_is_reported_for_documented_enum_member_with_remarks() => An_issue_is_reported_for(@"
public enum TestMe
{
    /// <remarks>
    /// Some remarks.
    /// </remarks>
    None = 0,
}");

        [Test]
        public void No_issue_is_reported_for_documented_field_with_remarks() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <remarks>
    /// Some remarks.
    /// </remarks>
    private int _field;
}");

        protected override string GetDiagnosticId() => MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer();
    }
}
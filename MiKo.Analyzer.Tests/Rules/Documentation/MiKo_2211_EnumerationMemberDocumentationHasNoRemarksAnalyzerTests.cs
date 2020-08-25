using Microsoft.CodeAnalysis.CodeFixes;
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

        [Test]
        public void Code_gets_fixed_for_remarks_only()
        {
            const string OriginalCode = @"
public enum TestMe
{
    /// <remarks>
    /// Some remarks.
    /// </remarks>
    None = 0,
}";

            const string FixedCode = @"
public enum TestMe
{
    /// <summary>
    /// Some remarks.
    /// </summary>
    None = 0,
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_summary_and_remarks()
        {
            const string OriginalCode = @"
public enum TestMe
{
    /// <summary>
    /// Some summary.
    /// </summary>
    /// <remarks>
    /// Some remarks.
    /// </remarks>
    None = 0,
}";

            const string FixedCode = @"
public enum TestMe
{
    /// <summary>
    /// Some summary.
    /// <para/>
    /// Some remarks.
    /// </summary>
    None = 0,
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiline_summary_and_remarks()
        {
            const string OriginalCode = @"
public enum TestMe
{
    /// <summary>
    /// Some summary.
    /// Some more summary.
    /// </summary>
    /// <remarks>
    /// Some remarks.
    /// Some more remarks.
    /// <list type=""bullet"">
    /// </list>
    /// </remarks>
    None = 0,
}";

            const string FixedCode = @"
public enum TestMe
{
    /// <summary>
    /// Some summary.
    /// Some more summary.
    /// <para/>
    /// Some remarks.
    /// Some more remarks.
    /// <list type=""bullet"">
    /// </list>
    /// </summary>
    None = 0,
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2211_CodeFixProvider();
    }
}
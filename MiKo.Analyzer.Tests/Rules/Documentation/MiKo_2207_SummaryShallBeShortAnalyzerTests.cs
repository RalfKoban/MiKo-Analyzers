using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2207_SummaryShallBeShortAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_being_not_too() => No_issue_is_reported_for(@"
/// <summary>
/// Specifies that the test fixture(s) marked with this attribute are considered to be <i>atomic</i> by NCrunch, meaning that their child tests cannot
/// be run separately from each other.
/// </summary>
/// <remarks>
/// A test being queued for execution under an atomic fixture will result in the entire fixture being queued with its child tests all executed in the
/// same task/batch.
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_too_long_class_summary() => An_issue_is_reported_for(@"
/// <summary>
/// Specifies that the test fixture(s) marked with this attribute are considered to be <i>atomic</i> by NCrunch, meaning that their child tests cannot
/// be run separately from each other.
/// A test being queued for execution under an atomic fixture will result in the entire fixture being queued with its child tests all executed in the
/// same task/batch.
/// </summary>
public class TestMe
{
}
");

        protected override string GetDiagnosticId() => MiKo_2207_SummaryShallBeShortAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2207_SummaryShallBeShortAnalyzer();
    }
}
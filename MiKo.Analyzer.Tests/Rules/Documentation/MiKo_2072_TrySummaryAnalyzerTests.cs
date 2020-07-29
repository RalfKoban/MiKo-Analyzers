using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2072_TrySummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool TryDoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_Try_method_([Values("Try", "Tries", "try", "tries")] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public bool TryDoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_async_Try_method_([Values("Try", "Tries", "try", "tries")] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Asynchronously " + phrase + @" something.
    /// </summary>
    public bool TryDoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_documented_non_Try_method_([Values("Try", "Tries", "try", "tries")] string phrase) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2072_TrySummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2072_TrySummaryAnalyzer();
    }
}
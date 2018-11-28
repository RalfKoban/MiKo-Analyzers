using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2070_ReturnsSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_items() => No_issue_is_reported_for(@"

public class TestMe
{
    public int SomethingProperty { get; set; }

    public int DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_items() => No_issue_is_reported_for(@"
using System.Windows.Input;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public int SomethingProperty { get; set; }

    /// <summary>
    /// Does something.
    /// </summary>
    public int DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method([Values("Return", "Returns", "return", "returns")] string phrase)
            => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public int SomethingProperty { get; set; }

    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public int DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property([Values("Return", "Returns", "return", "returns")] string phrase)
            => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @" something.
    /// </summary>
    public int SomethingProperty { get; set; }

    /// <summary>
    /// Does something.
    /// </summary>
    public int DoSomething()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2070_ReturnsSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2070_ReturnsSummaryAnalyzer();
    }
}
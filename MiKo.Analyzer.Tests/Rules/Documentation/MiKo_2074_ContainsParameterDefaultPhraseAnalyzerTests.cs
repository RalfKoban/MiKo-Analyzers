using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2074_ContainsParameterDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool Contains()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_([Values("Contains", "ContainsKey")] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// Something to find.
    /// </param>
    public bool " + methodName + @"(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_parameter_on_method_(
                                                                [Values("Contains", "ContainsKey")] string methodName,
                                                                [Values(@"<param name=""i"" />", @"<param name=""i""></param>", @"<param name=""i"">     </param>")] string parameter)
            => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// " + parameter + @"
    public bool " + methodName + @"(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_documented_non_Contains_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// Something to find.
    /// </param>
    public void DoSomething(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_([Values("Contains", "ContainsKey")] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// Something to seek.
    /// </param>
    public bool " + methodName + @"(int i)
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2074_ContainsParameterDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2074_ContainsParameterDefaultPhraseAnalyzer();
    }
}
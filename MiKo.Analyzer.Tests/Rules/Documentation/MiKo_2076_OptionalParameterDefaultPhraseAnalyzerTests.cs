using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2076_OptionalParameterDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method_with_no_optional_value() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_with_no_optional_value() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// </param>
    public bool DoSomething(bool value)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_method_with_optional_value() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(bool value = false)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_parameter_on_method_([Values("", "     ")] string parameter) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">" + parameter + @"</param>
    public bool DoSomething(bool value = false)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_documented_method_with_optional_value_with_missing_default_documentation() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""value"">
    /// Some value.
    /// </param>
    public bool DoSomething(bool value = false)
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2076_OptionalParameterDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2076_OptionalParameterDefaultPhraseAnalyzer();
    }
}
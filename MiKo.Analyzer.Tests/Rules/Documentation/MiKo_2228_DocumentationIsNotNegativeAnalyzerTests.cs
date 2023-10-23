using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2228_DocumentationIsNotNegativeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [TestCase("Cannot be used.")]
        [TestCase("The annotation is here to have something annotated.")]
        public void No_issue_is_reported_for_type_with_documentation_(string text) => No_issue_is_reported_for(@"
/// <summary>
/// " + text + @"
/// This class cannot be inherited.
/// </summary>
public class TestMe
{
}
");

        [TestCase("Can be invoked.")]
        [TestCase("Cannot be invoked.")]
        [TestCase("Can not be invoked.")]
        [TestCase("The annotation is here to have something annotated.")]
        public void No_issue_is_reported_for_method_with_documentation_(string text) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + text + @"
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [TestCase("Shall not be invoked in case it is not available.")]
        [TestCase("Should not be invoked in case it is not available.")]
        [TestCase("Shouldn't be invoked in case it is not available.")]
        [TestCase("Will not be invoked in case it is not available.")]
        [TestCase("Won't be invoked in case it is not available.")]
        [TestCase("Cannot be invoked in case it is not available.")]
        [TestCase("Cannot be invoked if it isn't available.")]
        [TestCase("Cannot be invoked if they aren't available.")]
        [TestCase("Isn't available and cannot be invoked.")]
        public void An_issue_is_reported_for_method_with_negative_documentation_(string text) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + text + @"
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [TestCase("Isn't available and cannot be invoked.")]
        public void An_issue_is_reported_for_method_with_negative_remarks_documentation_(string text) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <remarks>
    /// " + text + @"
    /// </remarks>
    public void DoSomething()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2228_DocumentationIsNotNegativeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2228_DocumentationIsNotNegativeAnalyzer();
    }
}
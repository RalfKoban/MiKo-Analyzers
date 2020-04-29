using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2029_InheritdocUsesWrongCrefAnalyzerTests : CodeFixVerifier
    {
        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("enum")]
        public void No_issue_is_reported_for_undocumented_named_type(string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe
{
}
");

        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("enum")]
        public void An_issue_is_reported_for_wrong_inherited_XML_of_named_type(string type) => An_issue_is_reported_for(@"
namespace Bla
{
    /// <inheritdoc cref='TestMe' />
    public " + type + @" TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        /// <inheritdoc cref='DoSomething' />
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_property() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        /// <inheritdoc cref='DoSomething' />
        public int DoSomething { get; set; }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2029_InheritdocUsesWrongCrefAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2029_InheritdocUsesWrongCrefAnalyzer();
    }
}
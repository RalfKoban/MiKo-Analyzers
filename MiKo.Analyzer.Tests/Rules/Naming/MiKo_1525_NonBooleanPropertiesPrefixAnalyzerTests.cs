using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1525_NonBooleanPropertiesPrefixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("AreSelected")]
        [TestCase("CanGoOnline")]
        [TestCase("ContainsStuff")]
        [TestCase("HasIcon")]
        [TestCase("IsSelected")]
        [TestCase("Is2D")]
        public void No_issue_is_reported_for_boolean_property_named_(string name) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public bool " + name + @" { get; set; }
    }
}
");

        [TestCase("AreSelected")]
        [TestCase("CanGoOnline")]
        [TestCase("ContainsStuff")]
        [TestCase("HasIcon")]
        [TestCase("IsSelected")]
        [TestCase("Is2D")]
        public void An_issue_is_reported_for_non_boolean_property_named_(string name) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public object " + name + @" { get; set; }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1525_NonBooleanPropertiesPrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1525_NonBooleanPropertiesPrefixAnalyzer();
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1035_PropertyModelSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("CreateViewModel")]
        [TestCase("CreateViewModels")]
        [TestCase("DoSomething")]
        [TestCase("EnableModelessStuff")]
        public void No_issue_is_reported_for_valid_property_(string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public string " + name + @" { get; set; }
}
");

        [TestCase("CreateModel")]
        [TestCase("CreateModels")]
        [TestCase("CreateitemModel")]
        [TestCase("CreateModelItem")]
        [TestCase("ModelCollection", Description = "Special situation as plural name becomes null and we don't want to have a NRE.")]
        public void An_issue_is_reported_for_invalid_property_(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public string " + name + @" { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_1035_PropertyModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1035_PropertyModelSuffixAnalyzer();
    }
}
using Microsoft.CodeAnalysis.CodeFixes;
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

        [TestCase("class TestMe { public string Model { get; set; } }", "class TestMe { public string Entity { get; set; } }")]
        [TestCase("class TestMe { public string ModelItem { get; set; } }", "class TestMe { public string Item { get; set; } }")]
        [TestCase("class TestMe { public string ModelCollection { get; set; } }", "class TestMe { public string Entities { get; set; } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1035_PropertyModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1035_PropertyModelSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1035_CodeFixProvider();
    }
}
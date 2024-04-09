using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1036_EventModelSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("CreateViewModel")]
        [TestCase("CreateViewModels")]
        [TestCase("DoSomething")]
        [TestCase("EnableModelessStuff")]
        public void No_issue_is_reported_for_valid_event_(string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public event EventHandler " + name + @";
}
");

        [TestCase("CreateModel")]
        [TestCase("CreateModels")]
        [TestCase("CreateItemModel")]
        [TestCase("CreateModelItem")]
        public void An_issue_is_reported_for_invalid_event_(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public event EventHandler " + name + @";
}
");

        [TestCase("class TestMe { public event EventHandler CreatedModel; }", "class TestMe { public event EventHandler Created; }")]
        [TestCase("class TestMe { public event EventHandler ModelCreated; }", "class TestMe { public event EventHandler Created; }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1036_EventModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1036_EventModelSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1036_CodeFixProvider();
    }
}
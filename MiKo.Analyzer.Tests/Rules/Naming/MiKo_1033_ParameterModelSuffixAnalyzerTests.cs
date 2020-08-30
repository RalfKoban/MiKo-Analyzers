using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1033_ParameterModelSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("viewModel")]
        [TestCase("viewModels")]
        [TestCase("item")]
        [TestCase("modeless")]
        [TestCase("semanticModel")]
        public void No_issue_is_reported_for_valid_parameter_(string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int " + name + @") { }
}
");

        [TestCase("model")]
        [TestCase("models")]
        [TestCase("itemModel")]
        [TestCase("modelItem")]
        public void An_issue_is_reported_for_invalid_parameter_(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int " + name + @") { }
}
");

        [TestCase("model")]
        [TestCase("models")]
        [TestCase("itemModel")]
        [TestCase("modelItem")]
        public void An_issue_is_reported_for_invalid_index_parameter_(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public int this[int " + name + @"] => " + name + @"
}
");

        [TestCase("class TestMe { public void DoSomething(object model) { } }", "class TestMe { public void DoSomething(object entity) { } }")]
        [TestCase("class TestMe { public void DoSomething(object modelItem) { } }", "class TestMe { public void DoSomething(object item) { } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1033_ParameterModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1033_ParameterModelSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1033_CodeFixProvider();
    }
}
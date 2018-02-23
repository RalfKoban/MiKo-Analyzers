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
        public void No_issue_is_reported_for_valid_parameter(string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int " + name + @") { }
}
");

        [TestCase("model")]
        [TestCase("models")]
        [TestCase("itemModel")]
        [TestCase("modelItem")]
        public void An_issue_is_reported_for_invalid_parameter(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int " + name + @") { }
}
");

        protected override string GetDiagnosticId() => MiKo_1033_ParameterModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1033_ParameterModelSuffixAnalyzer();
    }
}
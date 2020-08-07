using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1034_FieldModelSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("viewModel")]
        [TestCase("viewModels")]
        [TestCase("item")]
        [TestCase("modeless")]
        [TestCase("_viewModel")]
        [TestCase("_viewModels")]
        [TestCase("_item")]
        [TestCase("_modeless")]
        [TestCase("m_viewModel")]
        [TestCase("m_viewModels")]
        [TestCase("m_item")]
        [TestCase("m_modeless")]
        [TestCase("s_viewModel")]
        [TestCase("s_viewModels")]
        [TestCase("s_item")]
        [TestCase("s_modeless")]
        public void No_issue_is_reported_for_valid_field_(string name) => No_issue_is_reported_for(@"
public class TestMe
{
    private int " + name + @";
}
");

        [TestCase("model")]
        [TestCase("models")]
        [TestCase("itemModel")]
        [TestCase("modelItem")]
        [TestCase("m_model")]
        [TestCase("m_models")]
        [TestCase("m_itemModel")]
        [TestCase("m_modelItem")]
        [TestCase("s_model")]
        [TestCase("s_models")]
        [TestCase("s_itemModel")]
        [TestCase("s_modelItem")]
        public void An_issue_is_reported_for_invalid_field_(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    private int " + name + @";
}
");

        protected override string GetDiagnosticId() => MiKo_1034_FieldModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1034_FieldModelSuffixAnalyzer();
    }
}
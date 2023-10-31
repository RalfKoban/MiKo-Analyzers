using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

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
        [TestCase("t_viewModel")]
        [TestCase("t_viewModels")]
        [TestCase("t_item")]
        [TestCase("t_modeless")]
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
        [TestCase("t_model")]
        [TestCase("t_models")]
        [TestCase("t_itemModel")]
        [TestCase("t_modelItem")]
        public void An_issue_is_reported_for_invalid_field_(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    private int " + name + @";
}
");

        [TestCase("class TestMe { private int model; }", "class TestMe { private int entity; }")]
        [TestCase("class TestMe { private int _model; }", "class TestMe { private int _entity; }")]
        [TestCase("class TestMe { private int m_model; }", "class TestMe { private int m_entity; }")]
        [TestCase("class TestMe { private static int s_model; }", "class TestMe { private static int s_entity; }")]
        [TestCase("class TestMe { private static int t_model; }", "class TestMe { private static int t_entity; }")]
        [TestCase("class TestMe { private int modelItem; }", "class TestMe { private int item; }")]
        [TestCase("class TestMe { private int _modelItem; }", "class TestMe { private int _item; }")]
        [TestCase("class TestMe { private int m_modelItem; }", "class TestMe { private int m_item; }")]
        [TestCase("class TestMe { private static int s_modelItem; }", "class TestMe { private static int s_item; }")]
        [TestCase("class TestMe { private static int t_modelItem; }", "class TestMe { private static int t_item; }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1034_FieldModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1034_FieldModelSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1034_CodeFixProvider();
    }
}
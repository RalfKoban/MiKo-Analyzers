using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1065_OperatorParameterNameAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_unary_parameter() => No_issue_is_reported_for(@"
public class TestMe
{
    public static implicit operator string(TestMe value) => ""bla"";
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_unary_name() => An_issue_is_reported_for(@"
public class TestMe
{
    public static implicit operator string(TestMe v) => ""bla"";
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_binary_parameters() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool operator !=(TestMe left, TestMe right)  => false;
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_binary_left_parameter_name() => An_issue_is_reported_for(@"
public class TestMe
{
    public static bool operator !=(TestMe l, TestMe right) => false;
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_binary_right_parameter_name() => An_issue_is_reported_for(@"
public class TestMe
{
    public static bool operator !=(TestMe left, TestMe r) => false;
}
");

        [TestCase(
                @"class TestMe { public static implicit operator string(TestMe v) => ""bla""; }",
                @"class TestMe { public static implicit operator string(TestMe value) => ""bla""; }")]
        [TestCase(
                "class TestMe { public static bool operator !=(TestMe l, TestMe r) => false; }",
                "class TestMe { public static bool operator !=(TestMe left, TestMe right) => false; }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1065_OperatorParameterNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1065_OperatorParameterNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1065_CodeFixProvider();
    }
}
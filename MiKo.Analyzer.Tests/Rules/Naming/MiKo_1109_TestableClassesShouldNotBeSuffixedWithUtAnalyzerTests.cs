using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_class_without_Ut_suffix() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Ut_suffix() => An_issue_is_reported_for(@"
public class TestMeUt
{
}
");

        [TestCase("class TestMeUt { }", "class TestableTestMe { }")]
        [TestCase("class TestableMeUt { }", "class TestableMe { }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1109_CodeFixProvider();
    }
}
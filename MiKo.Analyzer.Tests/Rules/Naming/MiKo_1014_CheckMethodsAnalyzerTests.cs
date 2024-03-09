using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1014_CheckMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("DoSomething")]
        [TestCase("CheckIn")]
        [TestCase("CheckOut")]
        [TestCase("CheckAccess")]
        public void No_issue_is_reported_for_correctly_named_method_(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("DoSomething")]
        [TestCase("CheckIn")]
        [TestCase("CheckOut")]
        [TestCase("CheckAccess")]
        public void No_issue_is_reported_for_correctly_named_local_function_(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [TestCase("CheckArguments")]
        [TestCase("CheckParameter")]
        [TestCase("CheckConnection")]
        [TestCase("CheckOnline")]
        [TestCase("Check")]
        public void An_issue_is_reported_for_wrong_named_method_(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("CheckArguments")]
        [TestCase("CheckParameter")]
        [TestCase("CheckConnection")]
        [TestCase("CheckOnline")]
        [TestCase("Check")]
        public void An_issue_is_reported_for_wrong_named_local_function_(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
public class TestMe
{
    [" + test + @"]
    public void Check() { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_inside_test_method_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
public class TestMe
{
    [" + test + @"]
    public void Something()
    {
        Check() { }
    }
}
");

        [TestCase("class TestMe { object CheckSomething() => 42; }", "class TestMe { object FindSomething() => 42; }")]
        [TestCase("class TestMe { bool CheckSomething() => true; }", "class TestMe { bool CanSomething() => true; }")]
        [TestCase("class TestMe { bool CheckForSomething() => true; }", "class TestMe { bool HasSomething() => true; }")]
        [TestCase("class TestMe { bool CheckFormat() => true; }", "class TestMe { bool HasFormat() => true; }")]
        [TestCase("class TestMe { void CheckSomething() { } }", "class TestMe { void VerifySomething() { } }")]
        [TestCase("class TestMe { void CheckSomething(object o) { } }", "class TestMe { void ValidateSomething(object o) { } }")]
        public void Code_gets_fixed_for_method_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [TestCase("class TestMe { void Something() { object CheckSomething() => 42; } }", "class TestMe { void Something() { object FindSomething() => 42; } }")]
        [TestCase("class TestMe { void Something() { bool CheckSomething() => true; } }", "class TestMe { void Something() { bool CanSomething() => true; } }")]
        [TestCase("class TestMe { void Something() { bool CheckForSomething() => true; } }", "class TestMe { void Something() { bool HasSomething() => true; } }")]
        [TestCase("class TestMe { void Something() { bool CheckFormat() => true; } }", "class TestMe { void Something() { bool HasFormat() => true; } }")]
        [TestCase("class TestMe { void Something() { void CheckSomething() { } } }", "class TestMe { void Something() { void VerifySomething() { } } }")]
        [TestCase("class TestMe { void Something() { void CheckSomething(object o) { } } }", "class TestMe { void Something() { void ValidateSomething(object o) { } } }")]
        public void Code_gets_fixed_for_local_function_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1014_CheckMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1014_CheckMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1014_CodeFixProvider();
    }
}
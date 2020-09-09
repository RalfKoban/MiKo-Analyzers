using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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

        [Test]
        public void No_issue_is_reported_for_test_method_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
public class TestMe
{
    [" + test + @"]
    public void Check() { }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_non_boolean_return_value()
        {
            const string OriginalCode = @"
public class TestMe
{
    public object CheckSomething() => 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public object FindSomething() => 42;
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_boolean_return_value()
        {
            const string OriginalCode = @"
public class TestMe
{
    public bool CheckSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    public bool CanSomething() => true;
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_void_method_without_parameters()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void CheckSomething() { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void VerifySomething() { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_void_method_with_parameters()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void CheckSomething(object o) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void ValidateSomething(object o) { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1014_CheckMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1014_CheckMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1014_CodeFixProvider();
    }
}
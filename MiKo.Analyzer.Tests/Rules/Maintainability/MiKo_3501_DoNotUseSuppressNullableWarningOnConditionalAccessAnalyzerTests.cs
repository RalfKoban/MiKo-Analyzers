using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3501_DoNotUseSuppressNullableWarningOnConditionalAccessAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_Nullable_suppression() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = o!.ToString();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_conditional_access_expression_without_Nullable_suppression() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = o?.ToString();
    }
}
");

        [TestCase("?.ToString()")]
        [TestCase("ToString()?.ToString()")]
        [TestCase("ToString()?.ToString().ToString()")]
        [TestCase("ToString()?.ToString()?.ToString()")]
        [TestCase("ToString().ToString()?.ToString()")]
        public void An_issue_is_reported_for_conditional_access_expression_with_Nullable_suppression_(string expression) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = o" + expression + @"!;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_conditional_access_expression_with_Nullable_suppression_on_array_access() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object[] o)
    {
        var s = o?[42]!;
    }
}
");

        [TestCase("?.ToString()")]
        [TestCase("ToString()?.ToString()")]
        [TestCase("ToString()?.ToString().ToString()")]
        [TestCase("ToString()?.ToString()?.ToString()")]
        [TestCase("ToString().ToString()?.ToString()")]
        public void Code_gets_fixed_for_conditional_access_expression_with_Nullable_suppression_(string expression)
        {
            var originalCode = @"
public class TestMe
{
    public string DoSomething(object o) => o" + expression + @"!;
}
";

            var fixedCode = @"
public class TestMe
{
    public string DoSomething(object o) => o" + expression + @";
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_access_expression_with_Nullable_suppression_on_array_access()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(object[] o)
    {
        var s = o?[42]!;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(object[] o)
    {
        var s = o?[42];
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3501_DoNotUseSuppressNullableWarningOnConditionalAccessAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3501_DoNotUseSuppressNullableWarningOnConditionalAccessAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3501_CodeFixProvider();
    }
}
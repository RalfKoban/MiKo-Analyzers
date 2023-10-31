using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1090_ParametersWrongSuffixedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_no_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_properly_named_parameter_([Values("comparer", "view", "item", "entity", "oldView", "newView")] string name)
            => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(object " + name + @") { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_incorrectly_named_parameter_([Values("myComparer", "myView", "myItem", "myEntity", "myElement")] string name)
            => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(object " + name + @") { }
}
");

        [TestCase("class TestMe { void DoSomething(object myComparer) { } }", "class TestMe { void DoSomething(object comparer) { } }")]
        [TestCase("class TestMe { void DoSomething(object myView) { } }", "class TestMe { void DoSomething(object view) { } }")]
        [TestCase("class TestMe { void DoSomething(object myItem) { } }", "class TestMe { void DoSomething(object item) { } }")]
        [TestCase("class TestMe { void DoSomething(object myEditor) { } }", "class TestMe { void DoSomething(object editor) { } }")]
        [TestCase("class TestMe { void DoSomething(object userEntity) { } }", "class TestMe { void DoSomething(object user) { } }")]
        [TestCase("class TestMe { void DoSomething(object userElement) { } }", "class TestMe { void DoSomething(object user) { } }")]
        [TestCase("class TestMe { void DoSomething(object frameworkElement) { } }", "class TestMe { void DoSomething(object element) { } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1090_ParametersWrongSuffixedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1090_ParametersWrongSuffixedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1090_CodeFixProvider();
    }
}
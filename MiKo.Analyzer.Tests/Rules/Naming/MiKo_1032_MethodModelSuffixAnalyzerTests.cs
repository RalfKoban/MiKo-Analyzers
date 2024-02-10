using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1032_MethodModelSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("CreateViewModel")]
        [TestCase("CreateViewModels")]
        [TestCase("DoSomething")]
        [TestCase("EnableModelessStuff")]
        public void No_issue_is_reported_for_valid_method_(string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + name + @"() { }
}
");

        [TestCase("CreateViewModel")]
        [TestCase("CreateViewModels")]
        [TestCase("DoSomething")]
        [TestCase("EnableModelessStuff")]
        public void No_issue_is_reported_for_valid_local_function_(string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void " + name + @"() { }
    }
}
");

        [TestCase("CreateModel")]
        [TestCase("CreateModels")]
        [TestCase("CreateItemModel")]
        [TestCase("CreateModelItem")]
        public void No_issue_is_reported_for_test_method_(string name) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void " + name + @"() { }
}
");

        [TestCase("CreateModel")]
        [TestCase("CreateModels")]
        [TestCase("CreateItemModel")]
        [TestCase("CreateModelItem")]
        public void An_issue_is_reported_for_invalid_method_(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + name + @"() { }
}
");

        [TestCase("CreateModel")]
        [TestCase("CreateModels")]
        [TestCase("CreateitemModel")]
        [TestCase("CreateModelItem")]
        public void An_issue_is_reported_for_invalid_local_function_(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void " + name + @"() { }
    }
}
");

        [TestCase("class TestMe { public void CreateModel() { } }", "class TestMe { public void Create() { } }")]
        [TestCase("class TestMe { public void CreateModelItem() { } }", "class TestMe { public void CreateItem() { } }")]
        public void Code_gets_fixed_for_method_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [TestCase("class TestMe { public void DoSomething() { void CreateModel() { } } }", "class TestMe { public void DoSomething() { void Create() { } } }")]
        [TestCase("class TestMe { public void DoSomething() { void CreateModelItem() { } } }", "class TestMe { public void DoSomething() { void CreateItem() { } } }")]
        public void Code_gets_fixed_for_local_function_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1032_MethodModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1032_MethodModelSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1032_CodeFixProvider();
    }
}
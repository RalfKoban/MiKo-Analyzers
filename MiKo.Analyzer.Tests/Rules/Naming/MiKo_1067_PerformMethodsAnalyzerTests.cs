using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1067_PerformMethodsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongNames =
                                                      {
                                                          "CanPerform",
                                                          "DoPerform",
                                                          "HasToPerform",
                                                          "Perform",
                                                      };

        private static readonly string[] AllowedNames =
                                                        {
                                                            "CheckPerformance",
                                                            "RunPerformanceTests",
                                                            "HasPerformed",
                                                        };

        [Test]
        public void No_issue_is_reported_for_correctly_named_method_([ValueSource(nameof(AllowedNames))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_local_function_([ValueSource(nameof(AllowedNames))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void " + methodName + @"() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_named_method_([ValueSource(nameof(WrongNames))] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_named_local_function_([ValueSource(nameof(WrongNames))] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void " + methodName + @"() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_interface_named_method_([ValueSource(nameof(WrongNames))] string methodName) => An_issue_is_reported_for(@"
public interface TestMe
{
    public bool " + methodName + @"();
}
");

        [TestCase("class TestMe { public void PerformAnalysis() { } }", "class TestMe { public void Analyze() { } }")]
        [TestCase("class TestMe { public void DoPerform() { } }", "class TestMe { public void Do() { } }")]
        public void Code_gets_fixed_for_method_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [TestCase("class TestMe { public void DoSomething() { void PerformAnalysis() { } } }", "class TestMe { public void DoSomething() { void Analyze() { } } }")]
        [TestCase("class TestMe { public void DoSomething() { void DoPerform() { } } }", "class TestMe { public void DoSomething() { void Do() { } } }")]
        public void Code_gets_fixed_for_local_function_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1067_PerformMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1067_PerformMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1067_CodeFixProvider();
    }
}
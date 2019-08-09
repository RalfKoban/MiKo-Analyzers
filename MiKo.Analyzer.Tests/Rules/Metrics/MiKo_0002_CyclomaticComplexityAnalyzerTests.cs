using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public sealed class MiKo_0002_CyclomaticComplexityAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Empty_method_is_not_reported() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [TestCase("default(T);")]
        [TestCase("default")]
        public void Valid_term_is_not_reported(string term) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
         " + term + @"
    }
}
");

        [TestCase("if (false) return; ")]
        [TestCase("var x = true ? 42 : 1; ")]
        [TestCase("var x = null ?? new object(); ")]
        [TestCase("var x = ((object)(null))?.ToString(); ")]
        [TestCase("foreach (var x in y); ")]
        [TestCase("for (var i = 0; i < 10; i++) ; ")]
        [TestCase("while(true); ")]
        [TestCase("do { i++; } while (true); ")]
        [TestCase("int x = 10; switch(x) { case 1: break; }")]
        [TestCase("object x = null; switch(x) { case null: break; }")]
        [TestCase("var x = new object(); switch (x) { case string _: break; }")]
        [TestCase("int x = 1 && 2;")]
        [TestCase("bool result = true && false;")]
        [TestCase("bool result = true || false;")]
        [TestCase("try { throw new Exception(); } catch { }")]
        [TestCase("try { throw new Exception(); } catch (Exception ex) when (ex is InvalidOperationException) { }")]
        public void Method_with_too_complex_term_is_reported(string term) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
         " + term + @"
    }
}
");

        protected override string GetDiagnosticId() => MiKo_0002_CyclomaticComplexityAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0002_CyclomaticComplexityAnalyzer { MaxCyclomaticComplexity = 1 };
    }
}
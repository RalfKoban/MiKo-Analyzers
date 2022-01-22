using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public sealed class MiKo_0006_LocalFunctionCyclomaticComplexityAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Empty_local_function_is_not_reported() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void LocalDoSomething()
        {
        }
    }
}
");

        [TestCase("default(T);")]
        [TestCase("default")]
        public void Valid_term_is_not_reported_(string term) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void LocalDoSomething()
        {
             " + term + @"
        }
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
        [TestCase("bool result = true and false;", Ignore = "Currently not testable")]
        [TestCase("bool result = true or false;", Ignore = "Currently not testable")]
        [TestCase("try { throw new Exception(); } catch { }")]
        [TestCase("try { throw new Exception(); } catch (Exception ex) when (ex is InvalidOperationException) { }")]
        public void Local_function_with_too_complex_term_is_reported_(string term) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void LocalDoSomething()
        {
             " + term + @"
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_0006_LocalFunctionCyclomaticComplexityAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0006_LocalFunctionCyclomaticComplexityAnalyzer { MaxCyclomaticComplexity = 1 };
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1013_CheckMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("DoSomething")]
        [TestCase("CheckIn")]
        [TestCase("CheckOut")]
        public void No_issue_is_reported_for_correctly_named_method(string methodName) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_wrong_named_method(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1013_CheckMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1013_CheckMethodsAnalyzer();
    }
}
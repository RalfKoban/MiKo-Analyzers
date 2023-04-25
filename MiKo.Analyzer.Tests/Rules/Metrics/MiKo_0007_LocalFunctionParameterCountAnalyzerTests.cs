using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public sealed class MiKo_0007_LocalFunctionParameterCountAnalyzerTests : CodeFixVerifier
    {
        [TestCase("")]
        [TestCase("int a")]
        [TestCase("int a, int b")]
        [TestCase("int a, int b, int c")]
        public void No_issue_is_reported_for_less_than_max_parameters_(string parameters) => No_issue_is_reported_for(@"
public class TestMe
{
    private bool DoSomething()
    {
        return true;

        bool LocalDoSomething(" + parameters + @") => true;
    }
}");

        [TestCase("int a, int b, int c, int d")]
        [TestCase("int a, int b, int c, int d, int e")]
        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void An_issue_is_reported_for_more_than_max_parameters_(string parameters) => An_issue_is_reported_for(@"
public class TestMe
{
    private bool DoSomething()
    {
        return true;

        bool LocalDoSomething(" + parameters + @") => true;
    }
}");

        protected override string GetDiagnosticId() => MiKo_0007_LocalFunctionParameterCountAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0007_LocalFunctionParameterCountAnalyzer();
    }
}
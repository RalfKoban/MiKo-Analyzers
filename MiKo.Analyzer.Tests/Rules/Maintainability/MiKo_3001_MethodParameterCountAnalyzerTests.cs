using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3001_MethodParameterCountAnalyzerTests : CodeFixVerifier
    {
        [TestCase("")]
        [TestCase("int a")]
        [TestCase("int a, int b")]
        [TestCase("int a, int b, int c")]
        [TestCase("int a, int b, int c, int d")]
        [TestCase("int a, int b, int c, int d, int e")]
        public void Less_than_max_parameters_are_allowed(string parameters) => No_issue_is_reported(@"
public class TestMe
{
    private bool DoSomething(" + parameters + @") => true;
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void More_than_max_parameters_are_reported(string parameters) => Issue_is_reported(@"
public class TestMe
{
    private bool DoSomething(" + parameters + @") => true;
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void More_than_max_parameters_are_reported_for_ctor(string parameters) => Issue_is_reported(@"
public class TestMe
{
    public TestMe(" + parameters + @") { }
}");

        protected override string GetDiagnosticId() => MiKo_3001_MethodParameterCountAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3001_MethodParameterCountAnalyzer();
    }
}
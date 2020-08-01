using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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

        [TestCase("CreateModel")]
        [TestCase("CreateModels")]
        [TestCase("CreateitemModel")]
        [TestCase("CreateModelItem")]
        public void An_issue_is_reported_for_invalid_method_(string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + name + @"() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1032_MethodModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1032_MethodModelSuffixAnalyzer();
    }
}
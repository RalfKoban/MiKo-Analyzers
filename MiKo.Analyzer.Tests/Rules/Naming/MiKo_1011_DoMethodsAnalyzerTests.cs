using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1011_DoMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("Dock")]
        [TestCase("Download")]
        [TestCase("CanDock")]
        [TestCase("CanDownload")]
        [TestCase("Whatever")]
        public void No_issue_is_reported_for_correctly_named_method(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("Do")]
        [TestCase("CanDo")]
        [TestCase("DoWhatever")]
        [TestCase("CanDoWhatever")]
        [TestCase("DoDock")]
        [TestCase("DoDownload")]
        [TestCase("CanDoDock")]
        [TestCase("CanDoDownload")]
        public void An_issue_is_reported_for_wrong_named_method(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1011_DoMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1011_DoMethodsAnalyzer();
    }
}
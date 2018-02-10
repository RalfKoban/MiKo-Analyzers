using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1011_DoMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("CanDock")]
        [TestCase("CanDouble")]
        [TestCase("CanDownload")]
        [TestCase("Dock")]
        [TestCase("Dot")]
        [TestCase("Double")]
        [TestCase("Download")]
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
        [TestCase("DoDot")]
        [TestCase("DoDouble")]
        [TestCase("DoDownload")]
        [TestCase("CanDoDock")]
        [TestCase("CanDoDouble")]
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
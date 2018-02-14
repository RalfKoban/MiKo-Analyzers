using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1031_ModelTypeSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("interface", "ISomething")]
        [TestCase("class", "Something")]
        [TestCase("interface", "ISomethingViewModel")]
        [TestCase("class", "SomethingViewModel")]
        public void No_issue_is_reported_for(string type, string name) => No_issue_is_reported_for(@"
public " + type + " " + name + @"
{
}
");

        [TestCase("interface", "ISomethingModel")]
        [TestCase("class", "SomethingModel")]
        public void An_issue_is_reported_for(string type, string name) => An_issue_is_reported_for(@"
public " + type + " " + name + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_1031_ModelTypeSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1031_ModelTypeSuffixAnalyzer();
    }
}
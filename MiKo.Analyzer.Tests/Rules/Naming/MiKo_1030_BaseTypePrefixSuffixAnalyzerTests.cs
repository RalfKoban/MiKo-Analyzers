using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1030_BaseTypePrefixSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("interface", "ISomething")]
        [TestCase("class", "Something")]
        public void No_issue_is_reported_for(string type, string name) => No_issue_is_reported_for(@"
public " + type + " " + name + @"
{
}
");

        [TestCase("interface", "ISomethingBase")]
        [TestCase("class", "SomethingBase")]
        [TestCase("interface", "IBaseSomething")]
        [TestCase("class", "BaseSomething")]
        [TestCase("interface", "ISomethingAbstract")]
        [TestCase("class", "SomethingAbstract")]
        [TestCase("interface", "IAbstractSomething")]
        [TestCase("class", "AbstractSomething")]
        [TestCase("interface", "IAbstractSomethingBase")]
        [TestCase("class", "AbstractSomethingBase")]
        public void An_issue_is_reported_for(string type, string name) => An_issue_is_reported_for(@"
public " + type + " " + name + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_1030_BaseTypePrefixSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1030_BaseTypePrefixSuffixAnalyzer();
    }
}
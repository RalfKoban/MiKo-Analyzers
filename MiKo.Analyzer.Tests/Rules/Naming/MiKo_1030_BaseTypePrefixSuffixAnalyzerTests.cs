using Microsoft.CodeAnalysis.CodeFixes;
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
        [TestCase("class", "IAbstraction")]
        [TestCase("class", "Abstraction")]
        public void No_issue_is_reported_for_(string type, string name) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_(string type, string name) => An_issue_is_reported_for(@"
public " + type + " " + name + @"
{
}
");

        [TestCase("interface", "ISomethingBase", "ISomething")]
        [TestCase("class", "SomethingBase", "Something")]
        [TestCase("interface", "IBaseSomething", "ISomething")]
        [TestCase("class", "BaseSomething", "Something")]
        [TestCase("interface", "ISomethingAbstract", "ISomething")]
        [TestCase("class", "SomethingAbstract", "Something")]
        [TestCase("interface", "IAbstractSomething", "ISomething")]
        [TestCase("class", "AbstractSomething", "Something")]
        [TestCase("interface", "IAbstractSomethingBase", "ISomething")]
        [TestCase("class", "AbstractSomethingBase", "Something")]
        public void Code_gets_fixed_(string type, string name, string expectedName) => VerifyCSharpFix(
                                                                                                       "public " + type + " " + name + " {  }",
                                                                                                       "public " + type + " " + expectedName + " {  }");

        protected override string GetDiagnosticId() => MiKo_1030_BaseTypePrefixSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1030_BaseTypePrefixSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1030_CodeFixProvider();
    }
}
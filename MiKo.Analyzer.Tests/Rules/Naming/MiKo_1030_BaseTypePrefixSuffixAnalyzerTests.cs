using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1030_BaseTypePrefixSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("interface", "ISomething")]
        [TestCase("class", "Something")]
        [TestCase("class", "IAbstraction")]
        [TestCase("class", "Abstraction")]
        [TestCase("class", "ClassBasedOnSomething")]
        public void No_issue_is_reported_for_(string type, string name) => No_issue_is_reported_for(@"
public " + type + " " + name + @"
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [TestCase("interface", "ISomethingBase", 1)]
        [TestCase("class", "SomethingBase", 1)]
        [TestCase("interface", "IBaseSomething", 1)]
        [TestCase("class", "BaseSomething", 1)]
        [TestCase("interface", "ISomethingAbstract", 1)]
        [TestCase("class", "SomethingAbstract", 1)]
        [TestCase("interface", "IAbstractSomething", 1)]
        [TestCase("class", "AbstractSomething", 1)]
        [TestCase("interface", "IAbstractSomethingBase", 2)]
        [TestCase("class", "AbstractSomethingBase", 2)]
        public void An_issue_is_reported_for_(string type, string name, int violations) => An_issue_is_reported_for(violations, @"
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
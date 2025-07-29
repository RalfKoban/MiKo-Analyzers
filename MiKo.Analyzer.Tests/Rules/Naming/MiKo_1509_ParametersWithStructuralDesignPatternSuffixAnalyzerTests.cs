using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1509_ParametersWithStructuralDesignPatternSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] PatternNames = ["Adapter", "Wrapper", "Decorator"];

        [Test]
        public void No_issue_is_reported_for_parameter_without_pattern_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(int something)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_with_suffix_([ValueSource(nameof(PatternNames))] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        private void DoSomething(int something" + name + @")
        {
        }
    }
}
");

        [TestCase("adapter", "adapted")]
        [TestCase("wrapper", "wrapped")]
        [TestCase("decorator", "decorated")]
        [TestCase("dataAdapter", "adaptedData")]
        [TestCase("dataWrapper", "wrappedData")]
        [TestCase("dataDecorator", "decoratedData")]
        public void Code_gets_fixed_for_parameter_with_(string originalName, string fixedName)
        {
            const string Template = @"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(int ###)
        {
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1509_ParametersWithStructuralDesignPatternSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1509_ParametersWithStructuralDesignPatternSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1509_CodeFixProvider();
    }
}
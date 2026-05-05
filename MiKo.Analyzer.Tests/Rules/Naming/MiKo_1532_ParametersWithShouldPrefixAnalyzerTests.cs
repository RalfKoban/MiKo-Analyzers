using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1532_ParametersWithShouldPrefixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_parameter_without_should_in_its_name() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_parameter_with_prefix_([Values("shall", "should")] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        private void DoSomething(int " + name + @"Something)
        {
        }
    }
}
");

        [TestCase("shallBeOnline", "isOnline")]
        [TestCase("shallConnect", "connect")]
        [TestCase("shallNotConnect", "notConnect")]
        [TestCase("shouldBeNotOnline", "isNotOnline")]
        [TestCase("shouldBeOnline", "isOnline")]
        [TestCase("shouldConnect", "connect")]
        [TestCase("shouldNotBeOnline", "isNotOnline")]
        public void Code_gets_fixed_for_parameter_(string originalName, string fixedName)
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

        protected override string GetDiagnosticId() => MiKo_1532_ParametersWithShouldPrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1532_ParametersWithShouldPrefixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1532_CodeFixProvider();
    }
}
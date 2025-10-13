using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1520_ToCopyVariablesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_variable_without_toCopy_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething()
        {
            int something = 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_named_([Values("toCopySomething", "somethingToCopy", "toCopy")] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        private void DoSomething()
        {
            int " + name + @" = 42;
        }
    }
}
");

        [TestCase("toCopy", "original")]
        [TestCase("toCopySomething", "originalSomething")]
        [TestCase("somethingToCopy", "originalSomething")]
        public void Code_gets_fixed_for_variable_(string originalName, string fixedName)
        {
            const string Template = @"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething()
        {
            int ### = 42;
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1520_ToCopyVariablesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1520_ToCopyVariablesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1520_CodeFixProvider();
    }
}
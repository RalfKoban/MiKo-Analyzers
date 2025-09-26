using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1519_ReferenceParametersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_parameter_without_reference_in_its_name() => No_issue_is_reported_for(@"
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

        [Test] // currently, we do not know how to rename such parameter 'reference'
        public void No_issue_is_reported_for_parameter_named_reference() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(int reference)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_named_([Values("referenceSomething", "someReference")] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        private void DoSomething(int " + name + @")
        {
        }
    }
}
");

        [TestCase("referenceSomething", "something")]
        [TestCase("someReference", "some")]
        [TestCase("someReferenceStuff", "someStuff")]
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

        protected override string GetDiagnosticId() => MiKo_1519_ReferenceParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1519_ReferenceParametersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1519_CodeFixProvider();
    }
}
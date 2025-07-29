using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1511_ProxyVariablesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_variable_without_proxy_in_its_name() => No_issue_is_reported_for(@"
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

        [Test] // currently, we do not know how to rename such variable 'proxy'
        public void No_issue_is_reported_for_variable_named_proxy() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething()
        {
            int proxy = 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_named_([Values("proxySomething", "someProxy")] string name) => An_issue_is_reported_for(@"
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

        [TestCase("proxySomething", "something")]
        [TestCase("someProxy", "some")]
        [TestCase("someProxyStuff", "someStuff")]
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

        protected override string GetDiagnosticId() => MiKo_1511_ProxyVariablesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1511_ProxyVariablesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1511_CodeFixProvider();
    }
}
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1528_ArgParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_parameter_with_correct_name() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_special_method_parameter_(
                                                                   [Values("Main", "Format", "FormatWith")] string method,
                                                                   [Values("arg", "args", "argument", "arguments")] string term)
            => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void " + method + "(int " + term + @")
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_starting_with_([Values("arg", "args", "argument", "arguments")] string term) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(int " + term + @"Something)
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_parameter_starting_with_([Values("arg", "args", "argument", "arguments")] string term)
        {
            const string Template = """

                                    namespace Bla
                                    {
                                        public class TestMe
                                        {
                                            private void DoSomething(int ###)
                                            {
                                            }
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", term + "Something"), Template.Replace("###", "something"));
        }

        protected override string GetDiagnosticId() => MiKo_1528_ArgParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1528_ArgParameterAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1528_CodeFixProvider();
    }
}
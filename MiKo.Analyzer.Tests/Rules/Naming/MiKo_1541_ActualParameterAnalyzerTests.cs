using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1541_ActualParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_parameter_without_actual_in_its_name() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_parameter_name_when_there_is_no_better_name_([Values("actual")] string term) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(int " + term + @")
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_starting_with_([Values("actual")] string term) => An_issue_is_reported_for(@"
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
        public void Code_gets_fixed_for_parameter_starting_with_([Values("actual")] string term)
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

        protected override string GetDiagnosticId() => MiKo_1541_ActualParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1541_ActualParameterAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1541_CodeFixProvider();
    }
}
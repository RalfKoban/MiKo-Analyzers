using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1527_TypeSuffixedWithSubAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_with_correct_name() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_type_ending_with_([Values("Sub")] string term) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe" + term + @"
    {
        private void DoSomething(int something)
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_type_ending_with_([Values("Sub")] string term)
        {
            const string Template = """

                                    namespace Bla
                                    {
                                        public class TestMe###
                                        {
                                            private void DoSomething(int something)
                                            {
                                            }
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", term), Template.Replace("###", string.Empty));
        }

        protected override string GetDiagnosticId() => MiKo_1527_TypeSuffixedWithSubAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1527_TypeSuffixedWithSubAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1527_CodeFixProvider();
    }
}
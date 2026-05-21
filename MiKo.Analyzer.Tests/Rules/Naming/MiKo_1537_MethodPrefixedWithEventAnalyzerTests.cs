using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1537_MethodPrefixedWithEventAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_with_correct_name() => No_issue_is_reported_for(@"
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

        [TestCase("Event")]
        public void An_issue_is_reported_for_method_starting_with_(string term) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void " + term + @"Something(int something)
        {
        }
    }
}
");

        [TestCase("Event", "On")]
        public void Code_gets_fixed_for_method_starting_with_(string originalTerm, string fixedTerm)
        {
            const string Template = """

                                    namespace Bla
                                    {
                                        public class TestMe
                                        {
                                            private void ###Something(int something)
                                            {
                                            }
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalTerm), Template.Replace("###", fixedTerm));
        }

        protected override string GetDiagnosticId() => MiKo_1537_MethodPrefixedWithEventAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1537_MethodPrefixedWithEventAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1537_CodeFixProvider();
    }
}
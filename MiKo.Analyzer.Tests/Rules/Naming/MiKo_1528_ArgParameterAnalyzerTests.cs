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
        private static readonly string[] AllowedMethodNames = ["Main", "Concat", "ConcatWidth", "ConcatenatedWith", "Format", "FormatWith", "FormattedWith"];

        [Test]
        public void No_issue_is_reported_for_parameter_without_arg_in_its_name() => No_issue_is_reported_for(@"
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
                                                                   [ValueSource(nameof(AllowedMethodNames))] string method,
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
        public void No_issue_is_reported_for_parameter_name_when_there_is_no_better_name_([Values("argument", "arguments")] string term) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_parameter_name_when_there_is_only_a_number_left_as_better_name_([Values("argument", "arguments")] string term) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(int " + term + @"42)
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
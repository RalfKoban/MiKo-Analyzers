using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1507_ParametersWithCounterSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_value_type_parameter_without_Counter_in_its_name() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_reference_type_parameter_with_Counter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(object somethingCounter)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_parameter_with_Counter_in_its_name() => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        private void DoSomething(int somethingCounter)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_with_Counter_in_its_name() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(int somethingCounter)
        {
        }
    }
}

");

        [TestCase("dataCounter", "countedData")]
        [TestCase("transactionCounter", "countedTransactions")]
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

        protected override string GetDiagnosticId() => MiKo_1507_ParametersWithCounterSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1507_ParametersWithCounterSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1507_CodeFixProvider();
    }
}
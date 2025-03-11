using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1504_PropertiesWithCounterSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_value_type_property_without_Counter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private int Something { get; set; }
  }
}
");

        [Test]
        public void No_issue_is_reported_for_reference_type_property_with_Counter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private object SomeCounter { get; set; }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_test_property_with_Counter_suffix() => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
  [TestFixture]
  public class TestMe
  {
      private int SomeCounter { get; set; }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_Counter_suffix() => An_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private int SomeCounter { get; set; }
  }
}
");

        [TestCase("DataCounter", "CountedData")]
        [TestCase("TransactionCounter", "CountedTransactions")]
        public void Code_gets_fixed_for_property_with_(string originalName, string fixedName)
        {
            const string Template = @"
namespace Bla
{
    public class TestMe
    {
        private int ### { get; set; }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1504_PropertiesWithCounterSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1504_PropertiesWithCounterSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1504_CodeFixProvider();
    }
}
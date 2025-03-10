using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1505_FieldsWithCounterSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] FieldPrefixes = Constants.Markers.FieldPrefixes;

        [Test]
        public void No_issue_is_reported_for_value_type_field_without_Counter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private int Something;
  }
}
");

        [Test]
        public void No_issue_is_reported_for_reference_type_field_with_Counter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private object SomeCounter;
  }
}
");

        [Test]
        public void No_issue_is_reported_for_test_field_with_([ValueSource(nameof(FieldPrefixes))] string fieldPrefix) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
  [TestFixture]
  public class TestMe
  {
      private int " + fieldPrefix + @"SomeCounter;
  }
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_([ValueSource(nameof(FieldPrefixes))] string fieldPrefix) => An_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private int " + fieldPrefix + @"SomeCounter;
  }
}
");

        [Test]
        public void Code_gets_fixed_for_field_with_([ValueSource(nameof(FieldPrefixes))] string fieldPrefix)
        {
            var originalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int " + fieldPrefix + @"SomeCounter;
    }
}
";

            var fixedCode = @"
namespace Bla
{
    public class TestMe
    {
        private int " + fieldPrefix + @"countedSome;
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1505_FieldsWithCounterSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1505_FieldsWithCounterSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1505_CodeFixProvider();
    }
}
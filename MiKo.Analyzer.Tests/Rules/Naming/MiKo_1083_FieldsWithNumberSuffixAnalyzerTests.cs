using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1083_FieldsWithNumberSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_no_number_suffix() => No_issue_is_reported_for(@"

public class TestMe
{
    private int Field;
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_number_suffix_if_type_of_field_has_no_number_suffix_([Range(0, 10)] int number) => No_issue_is_reported_for(@"

public class TestMe
{
    public object Field" + number + @";
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_number_suffix_if_type_of_field_has_number_suffix_([Range(0, 10)] int number) => An_issue_is_reported_for(@"

public class TestMe
{
    public Int32 Field" + number + @";
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_OS_bit_number_suffix_if_type_of_field_has_number_suffix_([Values(32, 64)] int number) => No_issue_is_reported_for(@"

public class TestMe
{
    public Int32 Field" + number + @";
}
");

        [Test]
        public void No_issue_is_reported_for_struct_field_in_test_class() => Assert.Multiple(() =>
                                                                                                  {
                                                                                                      foreach (var testFixture in TestFixtures)
                                                                                                      {
                                                                                                          foreach (var number in Enumerable.Range(0, 10))
                                                                                                          {
                                                                                                              No_issue_is_reported_for(@"
using System;

[" + testFixture + @"]
public class TestMe
{
    public Int32 Field" + number + @";
}
");
                                                                                                          }
                                                                                                      }
                                                                                                  });

        [Test]
        public void An_issue_is_reported_for_non_struct_field_in_test_class() => Assert.Multiple(() =>
                                                                                                      {
                                                                                                          foreach (var testFixture in TestFixtures)
                                                                                                          {
                                                                                                              foreach (var number in Enumerable.Range(0, 10))
                                                                                                              {
                                                                                                                  An_issue_is_reported_for(@"
public class T123
{
}

[" + testFixture + @"]
public class TestMe
{
    public T123 Field" + number + @";
}
");
                                                                                                              }
                                                                                                          }
                                                                                                      });

        [Test]
        public void Code_gets_fixed() => VerifyCSharpFix(
                                                     "class TestMe { int i1 = 42; }",
                                                     "class TestMe { int i = 42; }");

        protected override string GetDiagnosticId() => MiKo_1083_FieldsWithNumberSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1083_FieldsWithNumberSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1083_CodeFixProvider();
    }
}
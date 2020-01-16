using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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
        public void No_issue_is_reported_for_field_with_number_suffix_if_type_of_field_has_no_number_suffix([Range(0, 10)] int number) => No_issue_is_reported_for(@"

public class TestMe
{
    public int Field" + number + @";
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_number_suffix_if_type_of_field_has_number_suffix([Range(0, 10)] int number) => An_issue_is_reported_for(@"

public class TestMe
{
    public Int32 Field" + number + @";
}
");

        protected override string GetDiagnosticId() => MiKo_1083_FieldsWithNumberSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1083_FieldsWithNumberSuffixAnalyzer();
    }
}
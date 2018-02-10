using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1023_FieldNameLengthAnalyzerTests : CodeFixVerifier
    {
        [TestCase("A")]
        [TestCase("Ab")]
        [TestCase("Abc")]
        [TestCase("Abcd")]
        [TestCase("Abcde")]
        [TestCase("Abcdef")]
        [TestCase("Abcdefg")]
        [TestCase("Abcdefgh")]
        [TestCase("Abcdefghi")]
        [TestCase("Abcdefghij")]
        [TestCase("Abcdefghijk")]
        [TestCase("Abcdefghijkl")]
        [TestCase("Abcdefghijklm")]
        [TestCase("Abcdefghijklmn")]
        [TestCase("Abcdefghijklmno")]
        public void No_issue_is_reported_for_field_with_fitting_length(string name) => No_issue_is_reported_for("private bool " + name + " = 42;");

        [TestCase("Abcdefghijklmnop")]
        [TestCase("Abcdefghijklmnopq")]
        [TestCase("Abcdefghijklmnopqrstuvwxyz")]
        public void An_issue_is_reported_for_field_with_exceeding_length(string name) => An_issue_is_reported_for("private bool " + name + " = 42;");

        [TestCase("Abcdefghijklmnopqrstuvwxyz")]
        public void No_issue_is_reported_for_enum_with_exceeding_length(string name) => No_issue_is_reported_for(@"
public enum MyEnum
{
    " + name + @" = 0,
}
");

        [TestCase("Abcdefghijklmnopqrstuvwxyz")]
        public void No_issue_is_reported_for_const_with_exceeding_length(string name) => No_issue_is_reported_for("public const string " + name  + " = string.Empty;");

        protected override string GetDiagnosticId() => MiKo_1023_FieldNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1023_FieldNameLengthAnalyzer();
    }
}
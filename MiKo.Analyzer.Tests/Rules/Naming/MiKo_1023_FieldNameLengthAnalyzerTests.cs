using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1023_FieldNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Fields);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.Fields);

        [Test]
        public void No_issue_is_reported_for_field_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for("private bool " + name + " = 42;");

        [Test]
        public void An_issue_is_reported_for_field_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for("private bool " + name + " = 42;");

        [Test]
        public void No_issue_is_reported_for_enum_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for(@"
public enum MyEnum
{
    " + name + @" = 0,
}
");

        [Test]
        public void No_issue_is_reported_for_const_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for("public const string " + name + " = string.Empty;");

        protected override string GetDiagnosticId() => MiKo_1023_FieldNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1023_FieldNameLengthAnalyzer();
    }
}
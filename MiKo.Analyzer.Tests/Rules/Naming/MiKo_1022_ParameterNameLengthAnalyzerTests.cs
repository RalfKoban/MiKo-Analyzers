using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1022_ParameterNameLengthAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_parameter_with_fitting_length(string name) => No_issue_is_reported_for("private bool DoSomething(int " + name + ") => true;");

        [TestCase("Abcdefghijklmnop")]
        [TestCase("Abcdefghijklmnopq")]
        [TestCase("Abcdefghijklmnopqrstuvwxyz")]
        public void An_issue_is_reported_for_parameter_with_exceeding_length(string name) => An_issue_is_reported_for("private bool DoSomething(int " + name + ") => true;");

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
        public void No_issue_is_reported_for_ctor_parameter_with_fitting_length(string name) => No_issue_is_reported_for("private DoSomething(int " + name + ") { }");

        [TestCase("Abcdefghijklmnop")]
        [TestCase("Abcdefghijklmnopq")]
        [TestCase("Abcdefghijklmnopqrstuvwxyz")]
        public void An_issue_is_reported_for_ctor_parameter_with_exceeding_length(string name) => An_issue_is_reported_for("private DoSomething(int " + name + ") { }");

        protected override string GetDiagnosticId() => MiKo_1022_ParameterNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1022_ParameterNameLengthAnalyzer();
    }
}
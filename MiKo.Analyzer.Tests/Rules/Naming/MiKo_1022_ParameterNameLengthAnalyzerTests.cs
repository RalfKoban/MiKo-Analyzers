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
        [TestCase("Abcdefghijklmnop")]
        [TestCase("Abcdefghijklmnopq")]
        [TestCase("Abcdefghijklmnopqr")]
        [TestCase("Abcdefghijklmnopqrs")]
        [TestCase("Abcdefghijklmnopqrst")]
        public void No_issue_is_reported_for_parameter_with_fitting_length(string name) => No_issue_is_reported_for("private bool DoSomething(int " + name + ") => true;");

        [TestCase("Abcdefghijklmnopqrstu")]
        [TestCase("Abcdefghijklmnopqrstuv")]
        [TestCase("Abcdefghijklmnopqrstuvw")]
        [TestCase("Abcdefghijklmnopqrstuvwx")]
        [TestCase("Abcdefghijklmnopqrstuvwxy")]
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
        [TestCase("Abcdefghijklmnopq")]
        [TestCase("Abcdefghijklmnopqr")]
        [TestCase("Abcdefghijklmnopqrs")]
        [TestCase("Abcdefghijklmnopqrst")]
        public void No_issue_is_reported_for_ctor_parameter_with_fitting_length(string name) => No_issue_is_reported_for("private DoSomething(int " + name + ") { }");

        [TestCase("Abcdefghijklmnopqrstu")]
        [TestCase("Abcdefghijklmnopqrstuv")]
        [TestCase("Abcdefghijklmnopqrstuvw")]
        [TestCase("Abcdefghijklmnopqrstuvwx")]
        [TestCase("Abcdefghijklmnopqrstuvwxy")]
        [TestCase("Abcdefghijklmnopqrstuvwxyz")]
        public void An_issue_is_reported_for_ctor_parameter_with_exceeding_length(string name) => An_issue_is_reported_for("private DoSomething(int " + name + ") { }");

        protected override string GetDiagnosticId() => MiKo_1022_ParameterNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1022_ParameterNameLengthAnalyzer();
    }
}
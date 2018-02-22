using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1024_PropertyNameLengthAnalyzerTests : CodeFixVerifier
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
        [TestCase("Abcdefghijklmnopqrstu")]
        [TestCase("Abcdefghijklmnopqrstuv")]
        [TestCase("Abcdefghijklmnopqrstuvw")]
        [TestCase("Abcdefghijklmnopqrstuvwx")]
        [TestCase("Abcdefghijklmnopqrstuvwxy")]
        public void No_issue_is_reported_for_property_with_fitting_length(string name) => No_issue_is_reported_for("public bool " + name + " { get; set; }");

        [TestCase("Abcdefghijklmnopqrstuvwxyz")]
        [TestCase("Abcdefghijklmnopqrstuvwxyzß")]
        public void An_issue_is_reported_for_property_with_exceeding_length(string name) => An_issue_is_reported_for("public bool " + name + " { get; set; }");

        protected override string GetDiagnosticId() => MiKo_1024_PropertyNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1024_PropertyNameLengthAnalyzer();
    }
}
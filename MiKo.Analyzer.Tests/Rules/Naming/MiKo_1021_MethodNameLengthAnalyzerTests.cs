using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1021_MethodNameLengthAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_with_fitting_length(string methodName) => No_issue_is_reported_for(@"

public void " + methodName + @"()
{
}
");

        [TestCase("Abcdefghijklmnopqrstuvwxyz")]
        [TestCase("Abcdefghijklmnopqrstuvwxyzß")]
        public void An_issue_is_reported_for_method_with_exceeding_length(string methodName) => An_issue_is_reported_for(@"

public void " + methodName + @"()
{
}
");

        [TestCase("Fact")]
        [TestCase("Fact()")]
        [TestCase("Test")]
        [TestCase("Test()")]
        [TestCase("TestCase")]
        [TestCase("TestCase()")]
        [TestCase("Theory")]
        [TestCase("Theory()")]
        [TestCase("TestMethod")]
        [TestCase("TestMethod()")]
        public void No_issue_is_reported_for_test_method_with_exceeding_length(string attributeName) => No_issue_is_reported_for(@"

[" + attributeName + @"]
public void Abcdefghijklmnopqrstuvwxyz()
{
}
");

        protected override string GetDiagnosticId() => MiKo_1021_MethodNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1021_MethodNameLengthAnalyzer();
    }
}
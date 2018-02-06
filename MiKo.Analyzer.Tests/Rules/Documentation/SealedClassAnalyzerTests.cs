using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public class SealedClassAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Correct_documentation_gets_not_reported()
        {
            No_issue_gets_reported(@"
/// <summary>
/// This class cannot be inherited.
/// </summary>
public sealed class TestMe
{
}
");
        }

        protected override string GetDiagnosticId() => SealedClassAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new SealedClassAnalyzer();
    }
}
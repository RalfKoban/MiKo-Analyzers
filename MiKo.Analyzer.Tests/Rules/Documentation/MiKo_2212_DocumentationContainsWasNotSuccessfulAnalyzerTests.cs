using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_comment() => No_issue_is_reported_for(@"
/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation() => An_issue_is_reported_for(@"
/// <summary>
/// This was not successful.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
/// <summary>
/// This was not successful.
/// </summary>
public class TestMe
{
}";

            const string FixedCode = @"
/// <summary>
/// This failed.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2212_CodeFixProvider();
    }
}
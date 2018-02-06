using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public class SealedClassAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Unsealed_class_is_not_reported()
        {
            No_issue_gets_reported(@"
/// <summary>
/// Something.
/// </summary>
public class TestMe
{
}
");
        }

        [Test]
        public void Struct_is_not_reported()
        {
            No_issue_gets_reported(@"
/// <summary>
/// Something.
/// </summary>
public struct TestMe
{
}
");
        }

        [Test]
        public void No_documentation_gets_not_reported()
        {
            No_issue_gets_reported(@"
public sealed class TestMe
{
}
");
        }

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

        [Test]
        public void Missing_documentation_gets_reported()
        {
            Issue_gets_reported(@"
/// <summary>
/// Some documentation
/// </summary>
public sealed class TestMe
{
}
");
        }

        [Test]
        public void Wrong_placed_documentation_gets_reported()
        {
            Issue_gets_reported(@"
/// <summary>
/// This class cannot be inherited.
/// Some documentation
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
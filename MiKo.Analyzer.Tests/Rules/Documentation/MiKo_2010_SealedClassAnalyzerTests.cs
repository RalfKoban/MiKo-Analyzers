using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public class MiKo_2010_SealedClassAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Struct_is_not_reported() => No_issue_is_reported(@"
/// <summary>
/// Something.
/// </summary>
public struct TestMe
{
}
");

        [Test]
        public void Unsealed_class_is_not_reported() => No_issue_is_reported(@"
/// <summary>
/// Something.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void Sealed_non_public_class_is_not_reported() => No_issue_is_reported(@"
/// <summary>
/// Something.
/// </summary>
private sealed class TestMe
{
}
");

        [Test]
        public void No_documentation_is_not_reported() => No_issue_is_reported(@"
public sealed class TestMe
{
}
");

        [Test]
        public void Correct_documentation_is_not_reported() => No_issue_is_reported(@"
/// <summary>
/// This class cannot be inherited.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Missing_documentation_is_reported() => Issue_is_reported(@"
/// <summary>
/// Some documentation
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Wrong_placed_documentation_is_reported() => Issue_is_reported(@"
/// <summary>
/// This class cannot be inherited.
/// Some documentation
/// </summary>
public sealed class TestMe
{
}
");

        protected override string GetDiagnosticId() => MiKo_2010_SealedClassAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2010_SealedClassAnalyzer();
    }
}
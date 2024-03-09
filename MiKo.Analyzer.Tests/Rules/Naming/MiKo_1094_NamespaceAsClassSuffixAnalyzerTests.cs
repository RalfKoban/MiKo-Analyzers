using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1094_NamespaceAsClassSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_wrong_name_([Values("Management", "Handling")] string suffix)
            => An_issue_is_reported_for(@"

public class My" + suffix + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_test_class_([ValueSource(nameof(TestFixtures))] string testFixture) => No_issue_is_reported_for(@"
/// <summary>
/// Some documentation
/// </summary>
[" + testFixture + @"]
public sealed class TestManagement
{
}
");

        protected override string GetDiagnosticId() => MiKo_1094_NamespaceAsClassSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1094_NamespaceAsClassSuffixAnalyzer();
    }
}
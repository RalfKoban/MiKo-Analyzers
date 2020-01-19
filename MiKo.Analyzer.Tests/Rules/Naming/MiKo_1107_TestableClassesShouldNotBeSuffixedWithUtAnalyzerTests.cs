using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1107_TestableClassesShouldNotBeSuffixedWithUtAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_class() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_class() => An_issue_is_reported_for(@"
[TestFixture]
public class TestMeUt
{
}
");

        protected override string GetDiagnosticId() => MiKo_1107_TestableClassesShouldNotBeSuffixedWithUtAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1107_TestableClassesShouldNotBeSuffixedWithUtAnalyzer();
    }
}
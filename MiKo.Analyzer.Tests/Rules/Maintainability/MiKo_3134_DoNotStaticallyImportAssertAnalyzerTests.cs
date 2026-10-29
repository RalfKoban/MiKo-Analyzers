using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3134_DoNotStaticallyImportAssertAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AssertionTypes = [.. Constants.Names.AssertionTypes];

        [Test]
        public void No_issue_is_reported_for_normal_using() => No_issue_is_reported_for(@"

using NUnit.Framework;

public class TestMe
{
    [Test]
    public void TestSomething() => Assert.That(true, Is.True);
}
");

        [Test]
        public void No_issue_is_reported_for_static_using_of_non_Assert() => No_issue_is_reported_for(@"

using static System.Math;

public class TestMe
{
    public int Something() => Min(1, 2);
}
");

        [Test]
        public void An_issue_is_reported_for_static_using_of_([ValueSource(nameof(AssertionTypes))] string assertType) => An_issue_is_reported_for(@"

using NUnit.Framework;

using static NUnit.Framework." + assertType + @";

public class TestMe
{
    [Test]
    public void TestSomething() => That(true, Is.True);
}
");

        protected override string GetDiagnosticId() => MiKo_3134_DoNotStaticallyImportAssertAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3134_DoNotStaticallyImportAssertAnalyzer();
    }
}
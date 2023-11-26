using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3211_PublicTypesShouldNotHaveFinalizerAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_types_without_finalizers() => No_issue_is_reported_for(@"
public class TestMe1
{
    private class NestedTestMe
    {
    }
}

internal class TestMe2
{
}

protected class TestMe3
{
}

public interface ITestMe1
{
}

internal interface ITestMe2
{
}
");

        [Test]
        public void No_issue_is_reported_for_non_public_types_with_finalizers() => No_issue_is_reported_for(@"
public class TestMe1
{
    private class NestedTestMe
    {
        ~NestedTestMe1() { }
    }
}

internal class TestMe2
{
    ~TestMe2() { }
}

protected class TestMe3
{
    ~TestMe3() { }
}
");

        [Test]
        public void An_issue_is_reported_for_public_type_with_finalizers() => An_issue_is_reported_for(@"
public class TestMe
{
    ~TestMe() { }
}
");

        protected override string GetDiagnosticId() => MiKo_3211_PublicTypesShouldNotHaveFinalizerAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3211_PublicTypesShouldNotHaveFinalizerAnalyzer();
    }
}
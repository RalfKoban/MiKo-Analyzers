using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3212_DoNotProvideUnexpectedDisposeMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_without_Dispose_methods() => No_issue_is_reported_for(@"
public class TestMe1
{
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_parameterless_Dispose_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_Dispose_disposing_method() => No_issue_is_reported_for(@"
public class TestMe
{
    protected void Dispose(bool disposing)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_Dispose_method_that_returns_something() => An_issue_is_reported_for(@"
public class TestMe
{
    public int Dispose() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_Dispose_disposing_method_that_returns_something() => An_issue_is_reported_for(@"
public class TestMe
{
    public int Dispose(bool disposing) => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_Dispose_method_that_has_1_non_boolean_parameter() => An_issue_is_reported_for(@"
public class TestMe
{
    protected void Dispose(object o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_Dispose_method_that_has_more_than_1_parameter() => An_issue_is_reported_for(@"
public class TestMe
{
    protected void Dispose(bool disposing, object o) { }
}
");

        protected override string GetDiagnosticId() => MiKo_3212_DoNotProvideUnexpectedDisposeMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3212_DoNotProvideUnexpectedDisposeMethodsAnalyzer();
    }
}
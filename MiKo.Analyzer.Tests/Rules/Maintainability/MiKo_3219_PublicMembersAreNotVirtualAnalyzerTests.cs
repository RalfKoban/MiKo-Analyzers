using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3219_PublicMembersAreNotVirtualAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_type() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_non_virtual_method_with_visibility_([Values("private", "protected", "internal", "public")] string visibility) => No_issue_is_reported_for(@"
public class TestMe
{
    " + visibility + @" int DoSomething(int value) => value;
}
");

        [Test]
        public void No_issue_is_reported_for_protected_override_method() => No_issue_is_reported_for(@"
public class TestMe
{
    protected override int DoSomething(int value) => value;
}
");

        [Test]
        public void No_issue_is_reported_for_public_override_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public override int DoSomething(int value) => value;
}
");

        [Test]
        public void No_issue_is_reported_for_protected_virtual_method() => No_issue_is_reported_for(@"
public class TestMe
{
    protected virtual int DoSomething(int value) => value;
}
");

        [Test]
        public void An_issue_is_reported_for_public_virtual_method() => An_issue_is_reported_for(@"
public class TestMe
{
    public virtual int DoSomething(int value) => value;
}
");

        protected override string GetDiagnosticId() => MiKo_3219_PublicMembersAreNotVirtualAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3219_PublicMembersAreNotVirtualAnalyzer();
    }
}
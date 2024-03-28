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
        public void No_issue_is_reported_for_interface() => No_issue_is_reported_for(@"
public interface TestMe
{
    string DoSomething();
}
");

        [Test]
        public void No_issue_is_reported_for_generated_class() => No_issue_is_reported_for(@"
[System.CodeDom.Compiler.GeneratedCodeAttribute()]
public class TestMe
{
    public virtual string DoSomething() => string.Empty;
}
");

        [Test]
        public void No_issue_is_reported_for_generated_partial_class() => No_issue_is_reported_for(@"
[System.CodeDom.Compiler.GeneratedCodeAttribute()]
public partial class TestMe
{
    public virtual string DoSomething() => string.Empty;

    partial void Initialize();
}

public partial class TestMe
{
    partial void Initialize() { }
}
");

        [Test]
        public void No_issue_is_reported_for_static_method_with_visibility_([Values("private", "protected", "internal", "public")] string visibility) => No_issue_is_reported_for(@"
public class TestMe
{
    " + visibility + @" static int DoSomething(int value) => value;
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

        [Test]
        public void An_issue_is_reported_for_public_virtual_property() => An_issue_is_reported_for(@"
public class TestMe
{
    public virtual int SomeProperty
    {
        get => 42;
        set => throw new System.NotImplementedException();
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3219_PublicMembersAreNotVirtualAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3219_PublicMembersAreNotVirtualAnalyzer();
    }
}
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_without_Dispose_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_method_as_first_method_if_no_ctor_or_finalizer_is_available() => No_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_as_first_method_if_no_ctor_or_finalizer_is_available() => No_issue_is_reported_for(@"
public class TestMe
{
    void IDisposable.Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_method_as_first_method_after_ctor() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_as_first_method_after_ctor() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    void IDisposable.Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_method_as_first_method_after_finalizer() => No_issue_is_reported_for(@"
public class TestMe
{
    public ~TestMe() { }

    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_as_first_method_after_finalizer() => No_issue_is_reported_for(@"
public class TestMe
{
    public ~TestMe() { }

    void IDisposable.Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_method_before_other_methods() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public ~TestMe() { }

    public void Dispose() { }

    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_before_other_methods() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public ~TestMe() { }

    void IDisposable.Dispose() { }

    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_method_before_ctor() => An_issue_is_reported_for(@"
public class TestMe
{
    public void Dispose() { }

    public TestMe() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_interface_method_before_ctor() => An_issue_is_reported_for(@"
public class TestMe
{
    void IDisposable.Dispose() { }

    public TestMe() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_method_in_between_ctor_and_finalizer() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public void Dispose() { }

    public ~TestMe() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_interface_method_in_between_ctor_and_finalizer() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    void IDisposable.Dispose() { }

    public ~TestMe() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_method_after_other_methods() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public ~TestMe() { }

    public void DoSomething() { }

    public void Dispose() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_interface_method_after_other_methods() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public ~TestMe() { }

    public void DoSomething() { }

    void IDisposable.Dispose() { }
}
");

        //// TODO RKN: partial parts

        protected override string GetDiagnosticId() => MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer();
    }
}
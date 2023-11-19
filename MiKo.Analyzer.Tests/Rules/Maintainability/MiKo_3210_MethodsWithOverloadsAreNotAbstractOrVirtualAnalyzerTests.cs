using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3210_MethodsWithOverloadsAreNotAbstractOrVirtualAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_without_methods() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_different_named_methods_that_are_virtual() => No_issue_is_reported_for(@"
public class TestMe
{
    public virtual void DoSomething() { }

    public virtual void DoSomethingElse() { }
}
");

        [Test]
        public void No_issue_is_reported_for_interface_with_same_named_methods() => No_issue_is_reported_for(@"
public interface TestMe
{
    object DoSomething();

    int DoSomething(int i);

    float DoSomething(float f);
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_same_named_methods_and_only_the_method_with_most_parameters_is_virtual() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }

    public void DoSomething(int i, int j) { }

    public virtual void DoSomething(int i, int j, int k) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_same_named_methods_and_only_the_single_method_with_most_parameters_is_virtual() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i) { }

    public void DoSomething(int i, int j) { }

    public virtual void DoSomething(int i, int j, int k) { }

    public void DoSomething(object x) { }

    public void DoSomething(object x, object y) { }

    public virtual void DoSomething(object x, object y, object z) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_same_named_methods_and_only_the_methods_with_most_parameters_are_virtual() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i) { }

    public virtual void DoSomething(int i, int j) { }

    public virtual void DoSomething(int i, object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_different_named_methods_that_are_abstract() => No_issue_is_reported_for(@"
public abstract class TestMe
{
    public abstract void DoSomething();

    public abstract void DoSomethingElse();
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_same_named_methods_and_only_the_method_with_most_parameters_is_abstract() => No_issue_is_reported_for(@"
public abstract class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }

    public void DoSomething(int i, int j) { }

    public abstract void DoSomething(int i, int j, int k);
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_same_named_methods_and_only_the_single_method_with_most_parameters_is_abstract() => No_issue_is_reported_for(@"
public abstract class TestMe
{
    public void DoSomething(int i) { }

    public void DoSomething(int i, int j) { }

    public abstract void DoSomething(int i, int j, int k);

    public void DoSomething(object x) { }

    public void DoSomething(object x, object y) { }

    public abstract void DoSomething(object x, object y, object z);
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_same_named_methods_and_only_the_methods_with_most_parameters_are_abstract() => No_issue_is_reported_for(@"
public abstract class TestMe
{
    public void DoSomething(int i) { }

    public abstract void DoSomething(int i, int j);

    public abstract void DoSomething(int i, object o);
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_type_with_same_named_methods_and_all_methods_are_virtual() => An_issue_is_reported_for(3, @"
public class TestMe
{
    public virtual void DoSomething() { }

    public virtual void DoSomething(int i) { }

    public virtual void DoSomething(int i, int j) { }

    public virtual void DoSomething(int i, int j, int k) { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_type_with_same_named_methods_and_some_methods_are_virtual() => An_issue_is_reported_for(2, @"
public class TestMe
{
    public virtual void DoSomething() { }

    public virtual void DoSomething(int i) { }

    public void DoSomething(int i, int j) { }

    public virtual void DoSomething(int i, int j, int k) { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_type_with_same_named_methods_and_all_methods_are_abstract() => An_issue_is_reported_for(3, @"
public abstract class TestMe
{
    public abstract void DoSomething();

    public abstract void DoSomething(int i);

    public abstract void DoSomething(int i, int j);

    public abstract void DoSomething(int i, int j, int k);
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_type_with_same_named_methods_and_some_methods_are_abstract() => An_issue_is_reported_for(2, @"
public abstract class TestMe
{
    public void DoSomething() { }

    public abstract void DoSomething(int i);

    public abstract void DoSomething(int i, int j);

    public void DoSomething(int i, int j, int k) { }
}
");

        protected override string GetDiagnosticId() => MiKo_3210_MethodsWithOverloadsAreNotAbstractOrVirtualAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3210_MethodsWithOverloadsAreNotAbstractOrVirtualAnalyzer();
    }
}
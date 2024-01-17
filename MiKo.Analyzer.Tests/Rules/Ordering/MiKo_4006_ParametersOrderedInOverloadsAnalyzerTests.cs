using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4006_ParametersOrderedInOverloadsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_with_no_methods() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_single_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_differently_named_methods() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i, int j) { }

    public void DoSomethingElse(int j, int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_2_similar_named_methods_if_1st_has_no_parameters() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_4_similar_named_methods_if_all_follow_naming_scheme() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }

    public void DoSomething(int i, int j) { }

    public void DoSomething(int i, int j, int k) { }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_2_similar_named_methods_if_a_single_parameter_got_switched() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i, int j) { }

    public void DoSomething(int j, int k, int i) { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [TestCase("int b, int a, int d, int c", 2)]
        [TestCase("int b, int a, int c, int d", 1)]
        [TestCase("int a, int b, int d, int c", 1)]
        [TestCase("int a, int c, int b, int d", 2)]
        [TestCase("int a, int c, int d, int b", 1)]
        public void An_issue_is_reported_for_type_with_2_similar_named_methods_if_some_parameters_got_switched_(string parameters, int errors) => An_issue_is_reported_for(errors, @"
public class TestMe
{
    public void DoSomething(" + parameters + @") { }

    public void DoSomething(int a, int b, int c, int d, int e) { }
}
");

        [Timeout(10 * 60 * 1000), Test]
        public void Dogfood() => No_issue_is_reported_for_folder_(@"D:\Projects\WAGO\GIT\e!COCKPIT");

        protected override string GetDiagnosticId() => MiKo_4006_ParametersOrderedInOverloadsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4006_ParametersOrderedInOverloadsAnalyzer();
    }
}
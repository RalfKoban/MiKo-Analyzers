using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1112_TestsShouldNotUseArbitraryIdentifiersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
    private int m_arbitraryField;

    public int ArbitraryProperty { get; set; }

    public void ArbitraryDoSomething() { }

    public void DoSomethingArbitrary(int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_field_in_test_class_with_wrong_name_([ValueSource(nameof(TestFixtures))] string testFixture)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    private string m_arbitraryFieldName;
}
");

        [Test]
        public void An_issue_is_reported_for_const_field_in_test_class_with_wrong_name_([ValueSource(nameof(TestFixtures))] string testFixture)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    private const int ArbitraryField = 42;
}
");

        [Test]
        public void An_issue_is_reported_for_property_in_test_class_with_wrong_name_([ValueSource(nameof(TestFixtures))] string testFixture)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    private string ArbitraryProperty { get; set; }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_method_in_test_class_with_wrong_name_(
                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void SomeArbitraryMethod() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_parameter_of_test_method_in_test_class_with_wrong_name_(
                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void SomeTest(int arbitraryParameter) { }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_of_test_method_in_non_test_class_with_wrong_name_([ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"

public class TestMe
{
    [" + test + @"]
    public void SomeTest(int arbitraryParameter) { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_const_in_test_method_in_test_class_with_wrong_name_(
                                                                                [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void SomeArbitraryMethod()
    {
        const int ArbitraryValue = 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_const_in_test_method_in_non_test_class_with_wrong_name_([ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"

public class TestMe
{
    [" + test + @"]
    public void SomeArbitraryMethod()
    {
        const int ArbitraryValue = 42;
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_local_variable_in_test_method_in_test_class_with_wrong_name_(
                                                                                                    [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                                    [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void SomeTest()
    {
        int arbitraryValue = 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_variable_in_test_method_in_non_test_class_with_wrong_name_([ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"

public class TestMe
{
    [" + test + @"]
    public void SomeTest()
    {
        int arbitraryValue = 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_variable_in_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        int arbitraryValue = 42;
    }
}
");

        [TestCase("arbitrary", "arbitrary")]
        [TestCase("arbitraryValue", "value")]
        public void Code_gets_fixed_(string originalCode, string fixedCode)
        {
            const string Template = @"
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        int ### = 42;
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        protected override string GetDiagnosticId() => MiKo_1112_TestsShouldNotUseArbitraryIdentifiersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1112_TestsShouldNotUseArbitraryIdentifiersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1112_CodeFixProvider();
    }
}
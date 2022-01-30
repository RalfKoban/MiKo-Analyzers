using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1081_MethodsWithNumberSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_no_number_suffix() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_with_no_number_suffix() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoSomethingCore() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_OS_bit_number_suffix_([Values(32, 64)] int number) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething" + number + @"() { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_with_OS_bit_number_suffix_([Values(32, 64)] int number) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoSomethingCore" + number + @"() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_number_suffix_([Range(0, 10)] int number) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething" + number + @"() { }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_number_suffix_([Range(0, 10)] int number) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoSomethingCore" + number + @"() { }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method() => VerifyCSharpFix(
                                                         "class TestMe { void DoSomething42() { } }",
                                                         "class TestMe { void DoSomething() { } }");

        [Test]
        public void Code_gets_fixed_for_local_function() => VerifyCSharpFix(
                                                         "class TestMe { void DoSomething() { void DoSomethingCore42() { } } }",
                                                         "class TestMe { void DoSomething() { void DoSomethingCore() { } } }");

        protected override string GetDiagnosticId() => MiKo_1081_MethodsWithNumberSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1081_MethodsWithNumberSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1081_CodeFixProvider();
    }
}
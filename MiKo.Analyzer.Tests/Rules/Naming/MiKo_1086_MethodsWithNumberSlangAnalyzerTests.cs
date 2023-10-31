using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1086_MethodsWithNumberSlangAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_no_slang_number() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_slang_number_([Values(2, 4)] int number) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething" + number + @"Whatever() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_slang_number_suffix_([Values(2, 4)] int number) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething" + number + @"() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_ordering_numbers() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething_after_2nd_or_4th_attempt() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1086_MethodsWithNumberSlangAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1086_MethodsWithNumberSlangAnalyzer();
    }
}
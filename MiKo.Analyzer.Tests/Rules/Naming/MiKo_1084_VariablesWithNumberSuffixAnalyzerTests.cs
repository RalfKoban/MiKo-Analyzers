using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1084_VariablesWithNumberSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_with_no_number_suffix() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_with_number_suffix_if_its_type_has_no_number_suffix_([Range(0, 10)] int number) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        object o" + number + @" = new object();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_with_number_suffix_if_its_type_has_a_number_suffix_([Range(0, 10)] int number) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Int32 o" + number + @" = new object();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_with_OS_bit_number_suffix_if_its_type_has_a_number_suffix_([Values(32, 64)] int number) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        var o" + number + @" = new object();
    }
}
");

        [Test]
        public void Code_gets_fixed() => VerifyCSharpFix(
                                                     "class TestMe { void Method() { int i1 = 42; } }",
                                                     "class TestMe { void Method() { int i = 42; } }");

        protected override string GetDiagnosticId() => MiKo_1084_VariablesWithNumberSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1084_VariablesWithNumberSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1084_CodeFixProvider();
    }
}
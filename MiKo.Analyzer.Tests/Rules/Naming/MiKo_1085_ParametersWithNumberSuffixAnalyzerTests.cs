using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1085_ParametersWithNumberSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_with_no_number_suffix() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_with_number_suffix_([Range(0, 10)] int number) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(int i" + number + @") { }
}
");

        [Test]
        public void No_issue_is_reported_for_unfinished_parameter_in_code() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(int ) { }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_that_fits_rule_MiKo_1001() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(EventArgs e0, EventArgs e1) { }
}
");

        [Test]
        public void No_issue_is_reported_for_specific_string_extension_method() => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static string FormatWith(this string format, object arg0) => string.Format(format, arg0);
}
");

        [Test]
        public void Code_gets_fixed() => VerifyCSharpFix(
                                                     "class TestMe { void Method(object o1) { } }",
                                                     "class TestMe { void Method(object o) { } }");

        protected override string GetDiagnosticId() => MiKo_1085_ParametersWithNumberSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1085_ParametersWithNumberSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1085_CodeFixProvider();
    }
}
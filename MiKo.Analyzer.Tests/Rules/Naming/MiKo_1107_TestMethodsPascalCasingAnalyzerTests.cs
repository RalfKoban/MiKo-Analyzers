﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1107_TestMethodsPascalCasingAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method_with_correct_name_(
                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method_with_correct_Upper_and_lower_case_name_(
                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something_with_MyEvent_stuff() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_method_with_wrong_name_(
                                                                    [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                    [ValueSource(nameof(Tests))] string test,
                                                                    [Values("DoSomethingDoesSomething", "DoSomething_DoesSomething", "DoSomething_Expect_DoSomething")] string name)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void " + name + @"() { }
}
");

        [TestCase("DoSomethingDoesSomething", "Do_something_does_something")]
        [TestCase("doSomethingDoesSomething", "do_something_does_something")]
        [TestCase("DoSomethingX", "Do_something_X")]
        [TestCase("DoSomethingWithHTML", "Do_something_with_HTML")]
        [TestCase("DoSomethingWithHTMLandMore", "Do_something_with_HTML_and_more")]
        [TestCase("HTMLdoSomething", "HTML_do_something")]
        public void Code_gets_fixed_(string original, string fix)
        {
            const string Template = @"
[TestFixture]
public class TestMe
{
    [Test]
    public void ###() { }
}
";

            VerifyCSharpFix(Template.Replace("###", original), Template.Replace("###", fix));
        }

        protected override string GetDiagnosticId() => MiKo_1107_TestMethodsPascalCasingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1107_TestMethodsPascalCasingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1107_CodeFixProvider();
    }
}
﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"

[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method_with_correct_upper_and_lower_case_name_(
                                                                                             [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                             [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"

[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something_with_MyEvent_stuff() { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_inside_test_method() => No_issue_is_reported_for(@"
using NUnit;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething_does_something_with_MyEvent_stuff()
    {
        void DoSomethingMyEventCore() { }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_method_with_wrong_name_(
                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                      [ValueSource(nameof(Tests))] string test,
                                                                      [Values("DoSomethingDoesSomething", "DoSomething_DoesSomething", "DoSomething_Expect_DoSomething")] string name)
            => An_issue_is_reported_for(@"

[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void " + name + @"() { }
}
");

        [TestCase("DoSomething_doesPushSomething", "Do_something_pushes_something")]
        [TestCase("DoSomething_DoesPushSomething", "Do_something_pushes_something")]
        [TestCase("doSomethingDoesPushSomething", "do_something_pushes_something")]
        [TestCase("DoSomethingDoesPushSomething", "Do_something_pushes_something")]
        [TestCase("DoSomethingMustBeSomething", "Do_something_is_something")]
        [TestCase("DoSomethingMustNotBeSomething", "Do_something_is_not_something")]
        [TestCase("DoSomethingShallBeSomething", "Do_something_is_something")]
        [TestCase("DoSomethingShallNotBeSomething", "Do_something_is_not_something")]
        [TestCase("DoSomethingShouldBeSomething", "Do_something_is_something")]
        [TestCase("DoSomethingShouldNotBeSomething", "Do_something_is_not_something")]
        [TestCase("DoSomethingWithHTML", "Do_something_with_HTML")]
        [TestCase("DoSomethingWithHTMLandMore", "Do_something_with_HTML_and_more")]
        [TestCase("DoSomethingWithIGraphAndMore", "Do_something_with_IGraph_and_more", Description = "Codefix recognizes interfaces in method name")]
        [TestCase("DoSomethingWithAIGraphAndMore", "Do_something_with_a_IGraph_and_more", Description = "Codefix recognizes interfaces in method name after 'a'")]
        [TestCase("DoSomethingX", "Do_something_X")]
        [TestCase("HTMLdoSomething", "HTML_do_something")]
        [TestCase("ShowsAWarning", "Shows_a_warning")]
        [TestCase("ShouldFailForSomething", "Fails_for_something")]
        [TestCase("ShouldReturnSomething", "Returns_something")]
        [TestCase("ShouldThrowSomething", "Throws_something")]
        [TestCase("ThrowsArgumentExceptionForSomething", "Throws_ArgumentException_for_something")]
        [TestCase("ThrowsArgumentNullExceptionForSomething", "Throws_ArgumentNullException_for_something")]
        [TestCase("ThrowsArgumentOutOfRangeExceptionForSomething", "Throws_ArgumentOutOfRangeException_for_something")]
        [TestCase("ThrowsInvalidOperationExceptionForSomething", "Throws_InvalidOperationException_for_something")]
        [TestCase("ThrowsNotImplementedExceptionForSomething", "Throws_NotImplementedException_for_something")]
        [TestCase("ThrowsNotSupportedExceptionForSomething", "Throws_NotSupportedException_for_something")]
        [TestCase("ThrowsObjectDisposedExceptionForSomething", "Throws_ObjectDisposedException_for_something")]
        [TestCase("ThrowsOperationCanceledExceptionForSomething", "Throws_OperationCanceledException_for_something")]
        [TestCase("ThrowsTaskCanceledExceptionForSomething", "Throws_TaskCanceledException_for_something")]
        [TestCase("DoSomethingDoesNotThrowException_but_return_something", "Do_something_does_not_throw_exception_but_returns_something")]
        [TestCase("DoSomething_ThrowException_InsteadToReturnSomething", "Do_something_throws_exception_instead_to_return_something")]
        [TestCase("DoSomething_ThrowException_InReturnSomething", "Do_something_throws_exception_in_return_something")]
        [TestCase("DoSomething_ThrowException_RemoveSomething", "Do_something_throws_exception_remove_something")]
        [TestCase("DoSomething_ThrowException_WillRemoveSomething", "Do_something_throws_exception_removes_something")]
        [TestCase("DoSomething_ThrowException_ToRemoveSomething", "Do_something_throws_exception_to_remove_something")]
        [TestCase("DoSomething_ThrowException_RejectSomething", "Do_something_throws_exception_reject_something")]
        [TestCase("DoSomething_ThrowException_WillRejectSomething", "Do_something_throws_exception_rejects_something")]
        [TestCase("DoSomething_ThrowException_ToRejectSomething", "Do_something_throws_exception_to_reject_something")]
        [TestCase("DoSomething_ThrowException_AcceptSomething", "Do_something_throws_exception_accept_something")]
        [TestCase("DoSomething_ThrowException_WillAcceptSomething", "Do_something_throws_exception_accepts_something")]
        [TestCase("DoSomething_ThrowException_ToAcceptSomething", "Do_something_throws_exception_to_accept_something")]
        [TestCase("DoSomething_ThrowException_KeepSomething", "Do_something_throws_exception_keeps_something")]
        [TestCase("DoSomething_ThrowException_DoesNotKeepSomething", "Do_something_throws_exception_does_not_keep_something")]
        [TestCase("DoSomething_ThrowException_WillKeepSomething", "Do_something_throws_exception_keeps_something")]
        [TestCase("DoSomething_ThrowException_ToKeepSomething", "Do_something_throws_exception_to_keep_something")]
        public void Code_gets_fixed_for_test_method_(string original, string fix)
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

        [Test]
        public void Code_gets_fixed_for_test_method_if_method_name_starts_with_called_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public bool DoSomething() => false;
}

public class TestMeTests
{
    [Test]
    public void DoSomething_ThrowException_InsteadToReturnSomething()
    {
        var objectUnderTest = new TestMe();

        var result = objectUnderTest.DoSomething();

        Assert.That(result, Is.True);
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public bool DoSomething() => false;
}

public class TestMeTests
{
    [Test]
    public void DoSomething_throws_exception_instead_to_return_something()
    {
        var objectUnderTest = new TestMe();

        var result = objectUnderTest.DoSomething();

        Assert.That(result, Is.True);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1107_TestMethodsPascalCasingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1107_TestMethodsPascalCasingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1107_CodeFixProvider();
    }
}
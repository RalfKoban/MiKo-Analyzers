﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1104_TestTeardownMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method_(
                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                      [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_setup_method_(
                                                            [ValueSource(nameof(TestFixtures))] string fixture,
                                                            [ValueSource(nameof(TestSetUps))] string test)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_teardown_method_with_correct_name_(
                                                                                 [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                 [ValueSource(nameof(TestTearDowns))] string test)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void CleanupTest() { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_inside_test_teardown_method() => No_issue_is_reported_for(@"
using NUnit;

[TestFixture]
public class TestMe
{
    [TearDown]
    public void CleanupTest()
    {
        void Teardown() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_teardown_method_with_wrong_name_(
                                                                               [ValueSource(nameof(TestFixtures))] string fixture,
                                                                               [ValueSource(nameof(TestTearDowns))] string test)
            => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void Teardown() { }
}
");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(TestTearDowns))] string test) => VerifyCSharpFix(
                                                                                                      "using System; class TestMe { [" + test + "] public void Teardown()  { } }",
                                                                                                      "using System; class TestMe { [" + test + "] public void CleanupTest()  { } }");

        protected override string GetDiagnosticId() => MiKo_1104_TestTeardownMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1104_TestTeardownMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1104_CodeFixProvider();
    }
}
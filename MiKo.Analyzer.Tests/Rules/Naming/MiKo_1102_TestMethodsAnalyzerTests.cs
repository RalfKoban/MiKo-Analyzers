﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1102_TestMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_correct_name_(
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

        [Test]
        public void No_issue_is_reported_for_local_function_with_correct_name_inside_test_method_(
                                                                                              [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                              [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
        void DoSomethingCore() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_wrong_name_(
                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                      [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoTestSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_wrong_name_inside_test_method_(
                                                                                            [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                            [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
        void DoTestSomethingCore() { }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_([Values("Test", "Test_", "_Test", "_Test_", "TestCase", "TestCase_", "_TestCase", "_TestCase_")] string test)
        {
            var originalCode = @"
[TestFixture]
public class TestMe
{
    [Test]
    public void Do" + test + @"Something() { }
}";

            const string FixedCode = @"
[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething() { }
}";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function_([Values("Test", "Test_", "_Test", "_Test_", "TestCase", "TestCase_", "_TestCase", "_TestCase_")] string test)
        {
            var originalCode = @"
[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        void Do" + test + @"SomethingCore() { }
    }
}";

            const string FixedCode = @"
[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        void DoSomethingCore() { }
    }
}";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1102_TestMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1102_TestMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1102_CodeFixProvider();
    }
}
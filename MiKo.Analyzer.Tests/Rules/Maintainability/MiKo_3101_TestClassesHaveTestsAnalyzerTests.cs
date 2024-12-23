﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3101_TestClassesHaveTestsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_class_with_tests_(
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
        public void No_issue_is_reported_for_test_class_with_nested_tests_(
                                                                       [ValueSource(nameof(TestFixtures))] string fixture,
                                                                       [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    public class Nested
    {
        [" + test + @"]
        public void DoSomething() { }
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_partial_test_class_with_tests_(
                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
public partial class TestMe
{
    [" + test + @"]
    public void DoSomething() { }
}

[" + fixture + @"]
public partial class TestMe
{
    private void DoSomethingElse() { }
}
");

        [Test]
        public void No_issue_is_reported_for_partial_test_class_without_tests_([ValueSource(nameof(TestFixtures))] string fixture)
            => No_issue_is_reported_for(@"
public partial class TestMe
{
    public void DoSomething() { }
}

[" + fixture + @"]
public partial class TestMe
{
    private void DoSomethingElse() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_class_with_multiple_base_classes_with_tests_(
                                                                                           [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                           [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe3 : TestMe2
{
    private void DoSomething3() { }
}

public class TestMe2 : TestMe1
{
    [" + test + @"]
    public void DoSomething2() { }
}

public class TestMe1
{
    [" + test + @"]
    public void DoSomething1() { }
}
");

        [Test]
        public void An_issue_is_reported_for_test_class_without_tests_([ValueSource(nameof(TestFixtures))] string fixture) => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    private void DoSomethingElse() { }
}
");

        [Test]
        public void An_issue_is_reported_for_test_class_with_multiple_base_classes_without_tests_([ValueSource(nameof(TestFixtures))] string fixture) => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe3 : TestMe2
{
    private void DoSomething3() { }
}

public class TestMe2 : TestMe1
{
    private void DoSomething2() { }
}

public class TestMe1
{
    private void DoSomething1() { }
}
");

        protected override string GetDiagnosticId() => MiKo_3101_TestClassesHaveTestsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3101_TestClassesHaveTestsAnalyzer();
    }
}
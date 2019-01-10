﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3105_TestTeardownMethodsOrderedSecondAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_class([ValueSource(nameof(TestFixtures))] string testClassAttribute) => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_a_test_class_with_only_a_test_method(
                                                                            [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                            [ValueSource(nameof(TestsExceptSetUps))] string testAttribute)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_a_test_class_with_setup_method_as_only_method(
                                                                                        [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                                        [ValueSource(nameof(TestSetUps))] string testSetupAttribute)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_a_test_class_with_teardown_method_as_only_method(
                                                                                        [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                                        [ValueSource(nameof(TestTearDowns))] string testTeardownAttribute)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testTeardownAttribute + @"]
    public void CleanupTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_a_test_class_with_teardown_method_before_a_test_method(
                                                                                        [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                                        [ValueSource(nameof(TestTearDowns))] string testTeardownAttribute,
                                                                                        [ValueSource(nameof(TestsExceptSetUpTearDowns))] string testAttribute)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testTeardownAttribute + @"]
    public void CleanupTest()
    {
    }

    [" + testAttribute + @"]
    public void DoSomething()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_a_test_class_with_teardown_method_after_a_setup_method(
                                                                                        [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                                        [ValueSource(nameof(TestSetUps))] string testSetupAttribute,
                                                                                        [ValueSource(nameof(TestTearDowns))] string testTeardownAttribute)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }

    [" + testTeardownAttribute + @"]
    public void CleanupTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_test_class_with_teardown_method_before_a_setup_method(
                                                                                        [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                                        [ValueSource(nameof(TestSetUps))] string testSetupAttribute,
                                                                                        [ValueSource(nameof(TestTearDowns))] string testTeardownAttribute)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testTeardownAttribute + @"]
    public void CleanupTest()
    {
    }

    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_test_class_with_teardown_method_after_a_test_method(
                                                                                        [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                                        [ValueSource(nameof(TestTearDowns))] string testTeardownAttribute,
                                                                                        [ValueSource(nameof(TestsExceptSetUpTearDowns))] string testAttribute)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething()
    {
    }

    [" + testTeardownAttribute + @"]
    public void CleanupTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_test_class_with_teardown_method_after_a_non_test_method(
                                                                                        [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                                        [ValueSource(nameof(TestTearDowns))] string testTeardownAttribute)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    public void DoSomething()
    {
    }

    [" + testTeardownAttribute + @"]
    public void CleanupTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_non_test_class_with_teardown_method_after_a_test_method(
            [ValueSource(nameof(TestTearDowns))] string testTeardownAttribute,
                                                                                                [ValueSource(nameof(TestsExceptSetUpTearDowns))] string testAttribute)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething()
    {
    }

    [" + testTeardownAttribute + @"]
    public void CleanupTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_non_test_class_with_teardown_method_after_a_non_test_method([ValueSource(nameof(TestTearDowns))] string testTeardownAttribute)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
    }

    [" + testTeardownAttribute + @"]
    public void CleanupTest()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3105_TestTeardownMethodsOrderedSecondAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3105_TestTeardownMethodsOrderedSecondAnalyzer();
    }
}
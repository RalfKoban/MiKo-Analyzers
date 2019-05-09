﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4101_TestSetupMethodsOrderedFirstAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_class([ValueSource(nameof(TestFixtures))] string testFixture) => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_a_test_class_with_only_a_test_method(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_a_test_class_with_only_a_test_teardown_method(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(TestTearDowns))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_a_test_class_with_setup_method_as_only_method(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(TestSetUps))] string testSetupAttribute)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_a_test_class_with_setup_method_as_first_method(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(TestSetUps))] string testSetupAttribute,
                                                                            [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }

    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_test_class_with_setup_method_after_a_test_method(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(TestSetUps))] string testSetupAttribute,
                                                                            [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }
}
");
        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_test_class_with_setup_method_after_a_test_teardown_method(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(TestSetUps))] string testSetupAttribute,
                                                                            [ValueSource(nameof(TestTearDowns))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }
}
");


        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_test_class_with_setup_method_after_a_non_test_method(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(TestSetUps))] string testSetupAttribute)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    public void DoSomething()
    {
    }

    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_non_test_class_with_setup_method_after_a_test_method(
                                                                            [ValueSource(nameof(TestSetUps))] string testSetupAttribute,
                                                                            [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_a_non_test_class_with_setup_method_after_a_test_teardown_method(
                                                                            [ValueSource(nameof(TestSetUps))] string testSetupAttribute,
                                                                            [ValueSource(nameof(TestTearDowns))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_setup_method_after_a_non_test_method([ValueSource(nameof(TestSetUps))] string testSetupAttribute)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
    }

    [" + testSetupAttribute + @"]
    public void PrepareTest()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_4101_TestSetupMethodsOrderedFirstAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4101_TestSetupMethodsOrderedFirstAnalyzer();
    }
}
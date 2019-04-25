using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2301_TestArrangeActAssertCommentAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Comments =
            {
                "act",
                "arrange",
                "assert",
                "prepare",
                "run",
                "set-up",
                "setup",
                "test",
                "verify",

                "Act.",
                "Arrange.",
                "Assert.",
                "Prepare.",
                "Run.",
                "Set-up.",
                "Setup.",
                "Test.",
                "Verify.",
            };

        [Test]
        public void No_issue_is_reported_for_commented_non_test_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        // act
        // arrange
        // assert
    }
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_test_method([ValueSource(nameof(TestsExceptSetUpTearDowns))] string testAttribute) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + testAttribute + @"
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_test_method([ValueSource(nameof(TestsExceptSetUpTearDowns))] string testAttribute) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + testAttribute + @"
    public void DoSomething()
    {
        // some comment
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_commented_test_method(
                                                                        [ValueSource(nameof(TestsExceptSetUpTearDowns))] string testAttribute,
                                                                        [ValueSource(nameof(Comments))] string comment,
                                                                        [Values("", " ")] string gap) => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + testAttribute + @"
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2301_TestArrangeActAssertCommentAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2301_TestArrangeActAssertCommentAnalyzer();
    }
}
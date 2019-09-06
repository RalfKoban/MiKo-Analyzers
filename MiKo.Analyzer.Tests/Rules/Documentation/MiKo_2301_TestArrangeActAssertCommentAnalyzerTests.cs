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
                "execute",
                "execution",
                "prep",
                "prepare",
                "preparation",
                "preparations",
                "run",
                "set-up",
                "setup",
                "test",
                "verify",
                "verification",
                "verifications",

                "Act.",
                "Arrange.",
                "Assert.",
                "Execute.",
                "Execution.",
                "Prep.",
                "Prepare.",
                "Preparation.",
                "Preparations.",
                "Run.",
                "Set-up.",
                "Setup.",
                "Test.",
                "Verify.",
                "Verification.",
                "Verifications.",
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
        public void No_issue_is_reported_for_uncommented_test_method([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_test_method([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
        // some comment
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_test_method([Values("", " ")] string gap) => Assert.Multiple(() =>
                                                                                                                                    {
                                                                                                                                        foreach (var test in Tests)
                                                                                                                                        {
                                                                                                                                            foreach (var comment in Comments)
                                                                                                                                            {
                                                                                                                                                An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    });

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_non_test_method_in_test_class([Values("", " ")] string gap) => Assert.Multiple(() =>
                                                                                                                                                      {
                                                                                                                                                          foreach (var fixture in TestFixtures)
                                                                                                                                                          {
                                                                                                                                                              foreach (var comment in Comments)
                                                                                                                                                              {
                                                                                                                                                                  An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");
                                                                                                                                                              }
                                                                                                                                                          }
                                                                                                                                                      });

        protected override string GetDiagnosticId() => MiKo_2301_TestArrangeActAssertCommentAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2301_TestArrangeActAssertCommentAnalyzer();
    }
}
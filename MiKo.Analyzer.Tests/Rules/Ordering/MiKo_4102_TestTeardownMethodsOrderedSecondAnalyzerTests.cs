using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4102_TestTeardownMethodsOrderedSecondAnalyzerTests : CodeFixVerifier
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

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_only_a_test_method() => Assert.Multiple(() =>
                                                                                                           {
                                                                                                               foreach (var testFixture in TestFixtures)
                                                                                                               {
                                                                                                                   foreach (var test in Tests)
                                                                                                                   {
                                                                                                                       No_issue_is_reported_for(@"
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
                                                                                                                   }
                                                                                                               }
                                                                                                           });

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_setup_method_as_only_method() => Assert.Multiple(() =>
                                                                                                                    {
                                                                                                                        foreach (var testFixture in TestFixtures)
                                                                                                                        {
                                                                                                                            foreach (var setup in TestSetUps)
                                                                                                                            {
                                                                                                                                No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }
}
");
                                                                                                                            }
                                                                                                                        }
                                                                                                                    });

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_teardown_method_as_only_method() => Assert.Multiple(() =>
                                                                                                                       {
                                                                                                                           foreach (var testFixture in TestFixtures)
                                                                                                                           {
                                                                                                                               foreach (var tearDown in TestTearDowns)
                                                                                                                               {
                                                                                                                                   No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + tearDown + @"]
    public void CleanupTest()
    {
    }
}
");
                                                                                                                               }
                                                                                                                           }
                                                                                                                       });

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_teardown_method_before_a_test_method() => Assert.Multiple(() =>
                                                                                                                             {
                                                                                                                                 foreach (var testFixture in TestFixtures)
                                                                                                                                 {
                                                                                                                                     foreach (var tearDown in TestTearDowns)
                                                                                                                                     {
                                                                                                                                         foreach (var test in Tests)
                                                                                                                                         {
                                                                                                                                             No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + tearDown + @"]
    public void CleanupTest()
    {
    }

    [" + test + @"]
    public void DoSomething()
    {
    }
}
");
                                                                                                                                         }
                                                                                                                                     }
                                                                                                                                 }
                                                                                                                             });

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_teardown_method_after_a_setup_method() => Assert.Multiple(() =>
                                                                                                                             {
                                                                                                                                 foreach (var testFixture in TestFixtures)
                                                                                                                                 {
                                                                                                                                     foreach (var setup in TestSetUps)
                                                                                                                                     {
                                                                                                                                         foreach (var tearDown in TestTearDowns)
                                                                                                                                         {
                                                                                                                                             No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }

    [" + tearDown + @"]
    public void CleanupTest()
    {
    }
}
");
                                                                                                                                         }
                                                                                                                                     }
                                                                                                                                 }
                                                                                                                             });

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_teardown_method_before_a_setup_method() => Assert.Multiple(() =>
                                                                                                                              {
                                                                                                                                  foreach (var testFixture in TestFixtures)
                                                                                                                                  {
                                                                                                                                      foreach (var setup in TestSetUps)
                                                                                                                                      {
                                                                                                                                          foreach (var tearDown in TestTearDowns)
                                                                                                                                          {
                                                                                                                                              An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + tearDown + @"]
    public void CleanupTest()
    {
    }

    [" + setup + @"]
    public void PrepareTest()
    {
    }
}
");
                                                                                                                                          }
                                                                                                                                      }
                                                                                                                                  }
                                                                                                                              });

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_teardown_method_after_a_test_method() => Assert.Multiple(() =>
                                                                                                                            {
                                                                                                                                foreach (var testFixture in TestFixtures)
                                                                                                                                {
                                                                                                                                    foreach (var tearDown in TestTearDowns)
                                                                                                                                    {
                                                                                                                                        foreach (var test in Tests)
                                                                                                                                        {
                                                                                                                                            An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + tearDown + @"]
    public void CleanupTest()
    {
    }
}
");
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            });

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_teardown_method_after_a_non_test_method() => Assert.Multiple(() =>
                                                                                                                                {
                                                                                                                                    foreach (var testFixture in TestFixtures)
                                                                                                                                    {
                                                                                                                                        foreach (var tearDown in TestTearDowns)
                                                                                                                                        {
                                                                                                                                            An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    public void DoSomething()
    {
    }

    [" + tearDown + @"]
    public void CleanupTest()
    {
    }
}");
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                });

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_teardown_method_after_a_test_method() => Assert.Multiple(() =>
                                                                                                                                {
                                                                                                                                    foreach (var tearDown in TestTearDowns)
                                                                                                                                    {
                                                                                                                                        foreach (var test in Tests)
                                                                                                                                        {
                                                                                                                                            An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + tearDown + @"]
    public void CleanupTest()
    {
    }
}
");
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                });

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_teardown_method_after_a_non_test_method() => Assert.Multiple(() =>
                                                                                                                                    {
                                                                                                                                        foreach (var tearDown in TestTearDowns)
                                                                                                                                        {
                                                                                                                                            An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
    }

    [" + tearDown + @"]
    public void CleanupTest()
    {
    }
}
");
                                                                                                                                        }
                                                                                                                                    });

        protected override string GetDiagnosticId() => MiKo_4102_TestTeardownMethodsOrderedSecondAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4102_TestTeardownMethodsOrderedSecondAnalyzer();
    }
}
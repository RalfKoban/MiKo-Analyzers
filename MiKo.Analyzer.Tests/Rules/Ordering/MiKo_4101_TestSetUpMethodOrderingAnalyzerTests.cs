using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4101_TestSetUpMethodOrderingAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_class_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_only_a_Test_method() => Assert.Multiple(() =>
                                                                                                           {
                                                                                                               foreach (var fixture in TestFixtures)
                                                                                                               {
                                                                                                                   foreach (var test in Tests)
                                                                                                                   {
                                                                                                                       No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
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
        public void No_issue_is_reported_for_a_test_class_with_only_a_TearDown_method() => Assert.Multiple(() =>
                                                                                                           {
                                                                                                               foreach (var fixture in TestFixtures)
                                                                                                               {
                                                                                                                   foreach (var tearDown in TestTearDowns)
                                                                                                                   {
                                                                                                                       No_issue_is_reported_for(@"

using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + tearDown + @"]
    public void DoSomething()
    {
    }
}
");
                                                                                                                   }
                                                                                                               }
                                                                                                           });

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_a_SetUp_method_as_only_method() => Assert.Multiple(() =>
                                                                                                                    {
                                                                                                                        foreach (var fixture in TestFixtures)
                                                                                                                        {
                                                                                                                            foreach (var setup in TestSetUps)
                                                                                                                            {
                                                                                                                                No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
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
        public void No_issue_is_reported_for_a_test_class_with_a_SetUp_method_before_a_Test_method() => Assert.Multiple(() =>
                                                                                                                     {
                                                                                                                         foreach (var fixture in TestFixtures)
                                                                                                                         {
                                                                                                                             foreach (var setup in TestSetUps)
                                                                                                                             {
                                                                                                                                 foreach (var test in Tests)
                                                                                                                                 {
                                                                                                                                     No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
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
        public void No_issue_is_reported_for_a_SetUp_method_after_a_OneTimeSetUp_and_OneTimeTearDown_method() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }

    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
    }

    [SetUp]
    public void PrepareTest()
    {
    }

    [TearDown]
    public void CleanupTest()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_SetUp_method_before_a_OneTimeSetUp_method() => Assert.Multiple(() =>
                                                                                                                         {
                                                                                                                             foreach (var fixture in TestFixtures)
                                                                                                                             {
                                                                                                                                 foreach (var setup in TestSetUps)
                                                                                                                                 {
                                                                                                                                     foreach (var oneTime in TestOneTimeSetUps)
                                                                                                                                     {
                                                                                                                                         An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }

    [" + oneTime + @"]
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
        public void An_issue_is_reported_for_a_test_class_with_a_SetUp_method_before_a_OneTimeTearDown_method() => Assert.Multiple(() =>
                                                                                                                         {
                                                                                                                             foreach (var fixture in TestFixtures)
                                                                                                                             {
                                                                                                                                 foreach (var setup in TestSetUps)
                                                                                                                                 {
                                                                                                                                     foreach (var oneTime in TestOneTimeTearDowns)
                                                                                                                                     {
                                                                                                                                         An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }

    [" + oneTime + @"]
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
        public void An_issue_is_reported_for_a_test_class_with_a_SetUp_method_after_a_TearDown_method() => Assert.Multiple(() =>
                                                                                                                                  {
                                                                                                                                      foreach (var fixture in TestFixtures)
                                                                                                                                      {
                                                                                                                                          foreach (var setup in TestSetUps)
                                                                                                                                          {
                                                                                                                                              foreach (var tearDown in TestTearDowns)
                                                                                                                                              {
                                                                                                                                                  An_issue_is_reported_for(@"

using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + tearDown + @"]
    public void DoSomething()
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
        public void An_issue_is_reported_for_a_test_class_with_a_SetUp_method_after_a_Test_method() => Assert.Multiple(() =>
                                                                                                                           {
                                                                                                                               foreach (var fixture in TestFixtures)
                                                                                                                               {
                                                                                                                                   foreach (var setup in TestSetUps)
                                                                                                                                   {
                                                                                                                                       foreach (var test in Tests)
                                                                                                                                       {
                                                                                                                                           An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
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
        public void An_issue_is_reported_for_a_test_class_with_a_SetUp_method_after_a_non_Test_method() => Assert.Multiple(() =>
                                                                                                                             {
                                                                                                                                 foreach (var fixture in TestFixtures)
                                                                                                                                 {
                                                                                                                                     foreach (var setup in TestSetUps)
                                                                                                                                     {
                                                                                                                                         An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    public void DoSomething()
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
                                                                                                                             });

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_a_SetUp_method_after_a_Test_method() => Assert.Multiple(() =>
                                                                                                                             {
                                                                                                                                 foreach (var setup in TestSetUps)
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

    [" + setup + @"]
    public void PrepareTest()
    {
    }
}
");
                                                                                                                                     }
                                                                                                                                 }
                                                                                                                             });

        protected override string GetDiagnosticId() => MiKo_4101_TestSetUpMethodOrderingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4101_TestSetUpMethodOrderingAnalyzer();
    }
}
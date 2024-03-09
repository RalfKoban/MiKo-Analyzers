using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4103_TestOneTimeSetUpMethodOrderingAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_a_test_class_with_OneTimeSetUp_method_as_only_method() => Assert.Multiple(() =>
                                                                                                                            {
                                                                                                                                foreach (var fixture in TestFixtures)
                                                                                                                                {
                                                                                                                                    foreach (var oneTime in TestOneTimeSetUps)
                                                                                                                                    {
                                                                                                                                        No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + oneTime + @"]
    public void PrepareTest()
    {
    }
}
");
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            });

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_OneTimeSetUp_method_as_first_method() => Assert.Multiple(() =>
                                                                                                                             {
                                                                                                                                 foreach (var fixture in TestFixtures)
                                                                                                                                 {
                                                                                                                                     foreach (var setup in TestOneTimeSetUps)
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
        public void An_issue_is_reported_for_a_test_class_with_OneTimeSetUp_method_after_a_OneTimeTearDown_method() => Assert.Multiple(() =>
                                                                                                                                            {
                                                                                                                                                foreach (var fixture in TestFixtures)
                                                                                                                                                {
                                                                                                                                                    foreach (var setup in TestOneTimeSetUps)
                                                                                                                                                    {
                                                                                                                                                        foreach (var tearDown in TestOneTimeTearDowns)
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
        public void An_issue_is_reported_for_a_test_class_with_OneTimeSetUp_method_after_a_SetUp_method() => Assert.Multiple(() =>
                                                                                                                                  {
                                                                                                                                      foreach (var fixture in TestFixtures)
                                                                                                                                      {
                                                                                                                                          foreach (var oneTime in TestOneTimeSetUps)
                                                                                                                                          {
                                                                                                                                              foreach (var setup in TestSetUps)
                                                                                                                                              {
                                                                                                                                                  An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void DoSomething()
    {
    }

    [" + oneTime + @"]
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
        public void An_issue_is_reported_for_a_test_class_with_OneTimeSetUp_method_after_a_Test_method() => Assert.Multiple(() =>
                                                                                                                                 {
                                                                                                                                     foreach (var fixture in TestFixtures)
                                                                                                                                     {
                                                                                                                                         foreach (var oneTime in TestOneTimeSetUps)
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

    [" + oneTime + @"]
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
        public void An_issue_is_reported_for_a_test_class_with_OneTimeSetUp_method_after_a_non_Test_method() => Assert.Multiple(() =>
                                                                                                                                     {
                                                                                                                                         foreach (var fixture in TestFixtures)
                                                                                                                                         {
                                                                                                                                             foreach (var oneTime in TestOneTimeSetUps)
                                                                                                                                             {
                                                                                                                                                 An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    public void DoSomething()
    {
    }

    [" + oneTime + @"]
    public void PrepareTest()
    {
    }
}
");
                                                                                                                                             }
                                                                                                                                         }
                                                                                                                                     });

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_OneTimeSetUp_method_after_a_Test_method() => Assert.Multiple(() =>
                                                                                                                                     {
                                                                                                                                         foreach (var oneTime in TestOneTimeSetUps)
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

    [" + oneTime + @"]
    public void PrepareTest()
    {
    }
}
");
                                                                                                                                             }
                                                                                                                                         }
                                                                                                                                     });

        [Test]
        public void Code_gets_fixed_for_test_method()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
    }

    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }

    [Test]
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_initialization_method()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
    [SetUp]
    public void PrepareTest()
    {
    }

    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }

    [SetUp]
    public void PrepareTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_cleanup_method()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
    [TearDown]
    public void CleanupTest()
    {
    }

    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }

    [TearDown]
    public void CleanupTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_OneTime_test_cleanup_method()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
    }

    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }
}
";

            const string FixedCode = @"
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
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_full_fledged_test()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
    [SetUp]
    public void PrepareTest()
    {
    }

    [TearDown]
    public void CleanupTest()
    {
    }

    [Test]
    public void DoSomething()
    {
    }

    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
    }

    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
    [OneTimeSetUp]
    public void PrepareTestEnvironment()
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

    [Test]
    public void DoSomething()
    {
    }

    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_full_fledged_test_with_region_around_field()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
    [SetUp]
    public void PrepareTest()
    {
    }

    [TearDown]
    public void CleanupTest()
    {
    }

    [Test]
    public void DoSomething()
    {
    }

    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
    }

    [OneTimeSetUp]
    public void PrepareTestEnvironment()
    {
    }

    #region Fields

    private int m_field;

    #endregion
}
";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
    [OneTimeSetUp]
    public void PrepareTestEnvironment()
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

    [Test]
    public void DoSomething()
    {
    }

    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
    }

    #region Fields

    private int m_field;

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4103_TestOneTimeSetUpMethodOrderingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4103_TestOneTimeSetUpMethodOrderingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4103_CodeFixProvider();
    }
}
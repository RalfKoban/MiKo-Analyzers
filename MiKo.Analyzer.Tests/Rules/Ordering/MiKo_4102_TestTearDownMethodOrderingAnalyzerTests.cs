using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4102_TestTearDownMethodOrderingAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_a_test_class_with_only_a_Test_method_(
                                                                               [ValueSource(nameof(TestFixtures))] string fixture,
                                                                               [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_a_SetUp_method_as_only_method_(
                                                                                          [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                          [ValueSource(nameof(TestSetUps))] string setup)
            => No_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_a_TearDown_method_as_only_method_(
                                                                                             [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                             [ValueSource(nameof(TestTearDowns))] string tearDown)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + tearDown + @"]
    public void CleanupTest()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_a_TearDown_method_before_a_Test_method_(
                                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                   [ValueSource(nameof(TestTearDowns))] string tearDown,
                                                                                                   [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
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

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_a_TearDown_method_after_a_SetUp_method_(
                                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                   [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                   [ValueSource(nameof(TestTearDowns))] string tearDown)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
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

        [Test]
        public void No_issue_is_reported_for_a_TearDown_method_after_a_OneTimeSetUp_and_OneTimeTearDown_and_SetUp_method() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_a_test_class_with_a_TearDown_method_before_a_SetUp_method_(
                                                                                                    [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                    [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                    [ValueSource(nameof(TestTearDowns))] string tearDown)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
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

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_TearDown_method_before_a_OneTimeSetUp_method_(
                                                                                                           [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                           [ValueSource(nameof(TestTearDowns))] string tearDown,
                                                                                                           [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + tearDown + @"]
    public void CleanupTest()
    {
    }

    [" + oneTimeSetUp + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_TearDown_method_before_a_OneTimeTearDown_method_(
                                                                                                              [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                              [ValueSource(nameof(TestTearDowns))] string tearDown,
                                                                                                              [ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + tearDown + @"]
    public void CleanupTest()
    {
    }

    [" + oneTimeTearDown + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_TearDown_method_after_a_Test_method_(
                                                                                                  [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                  [ValueSource(nameof(TestTearDowns))] string tearDown,
                                                                                                  [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
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

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_TearDown_method_after_a_non_Test_method_(
                                                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                      [ValueSource(nameof(TestTearDowns))] string tearDown)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
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

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_a_TearDown_method_after_a_Test_method_(
                                                                                                      [ValueSource(nameof(TestTearDowns))] string tearDown,
                                                                                                      [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
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

    [TearDown]
    public void CleanupTest()
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
    [TearDown]
    public void CleanupTest()
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
    [TearDown]
    public void CleanupTest()
    {
    }

    [SetUp]
    public void PrepareTest()
    {
    }
}
";

            const string FixedCode = @"
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
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_OneTime_test_initialization_method()
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
    [TearDown]
    public void CleanupTest()
    {
    }

    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
    [OneTimeTearDown]
    public void CleanupTestEnvironment()
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
        public void Code_gets_fixed_for_full_fledged_test()
        {
            const string OriginalCode = @"
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

    [Test]
    public void DoSomething()
    {
    }

    [TearDown]
    public void CleanupTest()
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
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4102_TestTearDownMethodOrderingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4102_TestTearDownMethodOrderingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4102_CodeFixProvider();
    }
}
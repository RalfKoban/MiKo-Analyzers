using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        public void No_issue_is_reported_for_a_test_class_with_only_a_test_method_(
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
        public void No_issue_is_reported_for_a_test_class_with_only_a_test_cleanup_method_(
                                                                                       [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                       [ValueSource(nameof(TestTearDowns))] string tearDown)
            => No_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_only_a_OneTime_test_initialization_method_(
                                                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                      [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + oneTimeSetUp + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_only_a_OneTime_test_cleanup_method_(
                                                                                               [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                               [ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + oneTimeTearDown + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_only_a_AssemblyWide_test_initialization_method_(
                                                                                                           [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                           [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + assemblySetUp + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_only_a_AssemblyWide_test_cleanup_method_(
                                                                                                    [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                    [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + assemblyTearDown + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_a_test_initialization_method_as_only_method_(
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
        public void No_issue_is_reported_for_a_test_class_with_a_test_initialization_method_before_a_test_method_(
                                                                                                              [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                              [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                              [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_a_test_initialization_method_after_a_OneTime_test_initialization_and_OneTime_test_cleanup_method_(
                                                                                                                                                             [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                                             [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                                                             [ValueSource(nameof(TestTearDowns))] string tearDown,
                                                                                                                                                             [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp,
                                                                                                                                                             [ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + oneTimeSetUp + @"]
    public void PrepareTestEnvironment()
    {
    }

    [" + oneTimeTearDown + @"]
    public void CleanupTestEnvironment()
    {
    }

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
        public void No_issue_is_reported_for_a_test_class_with_a_test_initialization_method_after_a_AssemblyWide_test_initialization_and_AssemblyWide_test_cleanup_method_(
                                                                                                                                                                       [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                                                       [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                                                                       [ValueSource(nameof(TestTearDowns))] string tearDown,
                                                                                                                                                                       [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp,
                                                                                                                                                                       [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + assemblySetUp + @"]
    public void PrepareTestAssembly()
    {
    }

    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }

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
        public void An_issue_is_reported_for_a_test_class_with_a_test_initialization_method_before_a_AssemblyWide_test_initialization_method_(
                                                                                                                                          [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                          [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                                          [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }

    [" + assemblySetUp + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_test_initialization_method_before_a_AssemblyWide_test_cleanup_method_(
                                                                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                   [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                                   [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }

    [" + assemblyTearDown + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_test_initialization_method_before_a_OneTime_test_initialization_method_(
                                                                                                                                     [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                     [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                                     [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }

    [" + oneTimeSetUp + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_test_initialization_method_before_a_OneTime_test_cleanup_method_(
                                                                                                                              [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                              [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                              [ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }

    [" + oneTimeTearDown + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_test_initialization_method_after_a_test_cleanup_method_(
                                                                                                                     [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                     [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                     [ValueSource(nameof(TestTearDowns))] string tearDown)
            => An_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_test_initialization_method_after_a_test_method_(
                                                                                                             [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                             [ValueSource(nameof(TestSetUps))] string setup,
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

    [" + setup + @"]
    public void PrepareTest()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_test_initialization_method_after_a_non_test_method_(
                                                                                                                 [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                 [ValueSource(nameof(TestSetUps))] string setup)
            => An_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_a_test_initialization_method_after_a_test_method_(
                                                                                                                 [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                 [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
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

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
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

    [Test]
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_cleanup_method()
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
        public void Code_gets_fixed_for_NUnit_OneTime_test_initialization_method()
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
        public void Code_gets_fixed_for_NUnit_OneTime_test_cleanup_method()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
    [SetUp]
    public void PrepareTest()
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

    [SetUp]
    public void PrepareTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_full_fledged_test()
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

    [TearDown]
    public void CleanupTest()
    {
    }

    [Test]
    public void DoSomething()
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

        [Test]
        public void Code_gets_fixed_for_MSTest_test_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestMethod]
    public void DoSomething()
    {
    }

    [TestInitialize]
    public void PrepareTest()
    {
    }
}
";

            const string FixedCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestInitialize]
    public void PrepareTest()
    {
    }

    [TestMethod]
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_test_cleanup_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestCleanup]
    public void CleanupTest()
    {
    }

    [TestInitialize]
    public void PrepareTest()
    {
    }
}
";

            const string FixedCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestInitialize]
    public void PrepareTest()
    {
    }

    [TestCleanup]
    public void CleanupTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_OneTime_test_initialization_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestInitialize]
    public void PrepareTest()
    {
    }

    [ClassInitialize]
    public void PrepareTestEnvironment()
    {
    }
}
";

            const string FixedCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [ClassInitialize]
    public void PrepareTestEnvironment()
    {
    }

    [TestInitialize]
    public void PrepareTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_OneTime_test_cleanup_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestInitialize]
    public void PrepareTest()
    {
    }

    [ClassCleanup]
    public void CleanupTestEnvironment()
    {
    }
}
";

            const string FixedCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [ClassCleanup]
    public void CleanupTestEnvironment()
    {
    }

    [TestInitialize]
    public void PrepareTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_AssemblyWide_test_initialization_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestInitialize]
    public void PrepareTest()
    {
    }

    [AssemblyInitialize]
    public void PrepareTestAssembly()
    {
    }
}
";

            const string FixedCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [AssemblyInitialize]
    public void PrepareTestAssembly()
    {
    }

    [TestInitialize]
    public void PrepareTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_AssemblyWide_test_cleanup_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestInitialize]
    public void PrepareTest()
    {
    }

    [AssemblyCleanup]
    public void CleanupTestAssembly()
    {
    }
}
";

            const string FixedCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [AssemblyCleanup]
    public void CleanupTestAssembly()
    {
    }

    [TestInitialize]
    public void PrepareTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_full_fledged_test()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [AssemblyInitialize]
    public void PrepareTestAssembly()
    {
    }

    [AssemblyCleanup]
    public void CleanupTestAssembly()
    {
    }

    [ClassInitialize]
    public void PrepareTestEnvironment()
    {
    }

    [ClassCleanup]
    public void CleanupTestEnvironment()
    {
    }

    [TestCleanup]
    public void CleanupTest()
    {
    }

    [TestMethod]
    public void DoSomething()
    {
    }

    [TestInitialize]
    public void PrepareTest()
    {
    }
}
";

            const string FixedCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [AssemblyInitialize]
    public void PrepareTestAssembly()
    {
    }

    [AssemblyCleanup]
    public void CleanupTestAssembly()
    {
    }

    [ClassInitialize]
    public void PrepareTestEnvironment()
    {
    }

    [ClassCleanup]
    public void CleanupTestEnvironment()
    {
    }

    [TestInitialize]
    public void PrepareTest()
    {
    }

    [TestCleanup]
    public void CleanupTest()
    {
    }

    [TestMethod]
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4101_TestSetUpMethodOrderingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4101_TestSetUpMethodOrderingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4101_CodeFixProvider();
    }
}
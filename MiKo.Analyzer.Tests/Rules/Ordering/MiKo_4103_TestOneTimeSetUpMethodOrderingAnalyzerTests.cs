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
        public void No_issue_is_reported_for_a_test_class_with_OneTime_test_initialization_method_as_only_method_(
                                                                                                              [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                              [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + oneTimeSetUp + @"]
    public void PrepareTest()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_OneTime_test_initialization_method_as_first_method_(
                                                                                                               [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                               [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp,
                                                                                                               [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + oneTimeSetUp + @"]
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
        public void No_issue_is_reported_for_a_test_class_with_a_OneTime_test_initialization_method_after_a_AssemblyWide_test_initialization_and_AssemblyWide_test_cleanup_method_(
                                                                                                                                                                               [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                                                               [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                                                                               [ValueSource(nameof(TestTearDowns))] string tearDown,
                                                                                                                                                                               [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp,
                                                                                                                                                                               [ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown,
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
        public void An_issue_is_reported_for_a_test_class_with_OneTime_test_initialization_method_after_a_OneTime_test_cleanup_method_(
                                                                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                   [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp,
                                                                                                                                   [ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown)
            => An_issue_is_reported_for(@"

using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + oneTimeTearDown + @"]
    public void DoSomething()
    {
    }

    [" + oneTimeSetUp + @"]
    public void PrepareTestEnvironment()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_OneTime_test_initialization_method_after_a_test_initialization_method_(
                                                                                                                                  [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                  [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp,
                                                                                                                                  [ValueSource(nameof(TestSetUps))] string setup)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void DoSomething()
    {
    }

    [" + oneTimeSetUp + @"]
    public void PrepareTestEnvironment()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_OneTime_test_initialization_method_after_a_test_method_(
                                                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                   [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp,
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

    [" + oneTimeSetUp + @"]
    public void PrepareTestEnvironment()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_OneTime_test_initialization_method_after_a_non_test_method_(
                                                                                                                       [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                       [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    public void DoSomething()
    {
    }

    [" + oneTimeSetUp + @"]
    public void PrepareTestEnvironment()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_OneTime_test_initialization_method_after_a_test_method_(
                                                                                                                       [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp,
                                                                                                                       [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + oneTimeSetUp + @"]
    public void PrepareTestEnvironment()
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
        public void Code_gets_fixed_for_NUnit_test_initialization_method()
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
        public void Code_gets_fixed_for_NUnit_OneTime_test_cleanup_method()
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
        public void Code_gets_fixed_for_NUnit_full_fledged_test()
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
        public void Code_gets_fixed_for_NUnit_full_fledged_test_with_region_around_field()
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

    [TestMethod]
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_test_initialization_method()
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

    [TestCleanup]
    public void CleanupTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_Class_test_cleanup_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [ClassCleanup]
    public void CleanupTestEnvironment()
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

    [ClassCleanup]
    public void CleanupTestEnvironment()
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

    [ClassCleanup]
    public void CleanupTestEnvironment()
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

    [ClassCleanup]
    public void CleanupTestEnvironment()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_full_fledged_test_with_region_around_field()
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

    [ClassCleanup]
    public void CleanupTestEnvironment()
    {
    }

    [ClassInitialize]
    public void PrepareTestEnvironment()
    {
    }

    #region Fields

    private int m_field;

    #endregion
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

    [ClassCleanup]
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
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4107_TestAssemblyWideTearDownMethodOrderingAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_class_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void No_issue_is_reported_for_a_test_class_with_AssemblyWide_test_initialization_method_as_only_method_(
                                                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                   [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp)
            => No_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + assemblySetUp + @"]
    public void PrepareTestAssembly()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_AssemblyWide_test_cleanup_method_as_only_method_(
                                                                                                            [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                            [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
            => No_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_AssemblyWide_test_initialization_method_as_first_method_(
                                                                                                                    [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                    [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp,
                                                                                                                    [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
            => No_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_AssemblyWide_test_cleanup_method_before_a_test_method_(
                                                                                                                  [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                  [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown,
                                                                                                                  [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }

    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_full_fledged_test() => No_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
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
    public void Test()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_AssemblyWide_test_cleanup_method_before_a_AssemblyWide_test_initialization_method_(
                                                                                                                                              [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                              [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp,
                                                                                                                                              [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
        => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }

    [" + assemblySetUp + @"]
    public void PrepareTestAssembly()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_AssemblyWide_test_cleanup_method_after_a_OneTime_test_initialization_method_(
                                                                                                                                          [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                          [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp,
                                                                                                                                          [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
        => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + oneTimeSetUp + @"]
    public void PrepareTestEnvironment()
    {
    }

    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_AssemblyWide_test_cleanup_method_after_a_OneTime_test_cleanup_method_(
                                                                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                   [ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown,
                                                                                                                                   [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
        => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + oneTimeTearDown + @"]
    public void CleanupTestEnvironment()
    {
    }

    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_a_AssemblyWide_test_cleanup_method_after_a_test_initialization_method_(
                                                                                                                                  [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                  [ValueSource(nameof(TestSetUps))] string setup,
                                                                                                                                  [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
            => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }

    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_AssemblyWide_test_cleanup_method_after_a_test_cleanup_method_(
                                                                                                                         [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                         [ValueSource(nameof(TestTearDowns))] string tearDown,
                                                                                                                         [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
            => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + tearDown + @"]
    public void CleanupTest()
    {
    }

    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_AssemblyWide_test_cleanup_method_after_a_test_method_(
                                                                                                                 [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                 [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown,
                                                                                                                 [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_AssemblyWide_test_cleanup_method_after_a_non_test_method_(
                                                                                                                     [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                     [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown)
            => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    public void DoSomething()
    {
    }

    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_AssemblyWide_test_cleanup_method_after_a_test_method_(
                                                                                                                     [ValueSource(nameof(TestAssemblyTearDowns))] string assemblyTearDown,
                                                                                                                     [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + assemblyTearDown + @"]
    public void CleanupTestAssembly()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_test_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestMethod]
    public void DoSomething()
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

    [TestMethod]
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
        public void Code_gets_fixed_for_test_cleanup_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [TestCleanup]
    public void CleanupTest()
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

    [TestCleanup]
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [OneTimeSetUp]
    public void PrepareTestEnvironment()
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

    [OneTimeSetUp]
    public void PrepareTestEnvironment()
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [OneTimeTearDown]
    public void CleanupTestEnvironment()
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

    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_AssemblyWide_test_initialization_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [AssemblyCleanup]
    public void CleanupTestAssembly()
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

    [AssemblyCleanup]
    public void CleanupTestAssembly()
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

        protected override string GetDiagnosticId() => MiKo_4107_TestAssemblyWideTearDownMethodOrderingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4107_TestAssemblyWideTearDownMethodOrderingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4107_CodeFixProvider();
    }
}
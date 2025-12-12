using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4106_TestAssemblyWideSetUpMethodOrderingAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_a_test_class_with_AssemblyWide_test_initialization_method_as_first_method_(
                                                                                                                    [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                    [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp,
                                                                                                                    [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + assemblySetUp + @"]
    public void PrepareTestAssembly()
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
        public void An_issue_is_reported_for_a_test_class_with_AssemblyWide_test_initialization_method_after_a_AssemblyWide_test_cleanup_method_(
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
        public void An_issue_is_reported_for_a_test_class_with_AssemblyWide_test_initialization_method_after_a_test_initialization_method_(
                                                                                                                                       [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                                       [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp,
                                                                                                                                       [ValueSource(nameof(TestSetUps))] string setup)
            => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    [" + setup + @"]
    public void PrepareTest()
    {
    }

    [" + assemblySetUp + @"]
    public void PrepareTestAssembly()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_AssemblyWide_test_initialization_method_after_a_test_method_(
                                                                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                        [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp,
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

    [" + assemblySetUp + @"]
    public void PrepareTestAssembly()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_AssemblyWide_test_initialization_method_after_a_non_test_method_(
                                                                                                                            [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                                            [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp)
            => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

[" + fixture + @"]
public class TestMe
{
    public void DoSomething()
    {
    }

    [" + assemblySetUp + @"]
    public void PrepareTest()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_non_test_class_with_AssemblyWide_test_initialization_method_after_a_test_method_(
                                                                                                                            [ValueSource(nameof(TestAssemblySetUps))] string assemblySetUp,
                                                                                                                            [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }

    [" + assemblySetUp + @"]
    public void PrepareTest()
    {
    }
}
");

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

    [TestCleanup]
    public void CleanupTest()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_MSTest_Class_test_initialization_method()
        {
            const string OriginalCode = @"
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestMe
{
    [ClassInitialize]
    public void PrepareTestEnvironment()
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

    [ClassInitialize]
    public void PrepareTestEnvironment()
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

    [ClassCleanup]
    public void CleanupTestEnvironment()
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

        protected override string GetDiagnosticId() => MiKo_4106_TestAssemblyWideSetUpMethodOrderingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4106_TestAssemblyWideSetUpMethodOrderingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4106_CodeFixProvider();
    }
}
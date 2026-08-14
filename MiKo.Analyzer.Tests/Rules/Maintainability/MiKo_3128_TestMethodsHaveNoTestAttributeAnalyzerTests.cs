using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3128_TestMethodsHaveNoTestAttributeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_marked_as_test() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest([Values(42)] int value)
        {
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
");

        [TestCase("Assert")]
        [TestCase("Constraint")]
        public void No_issue_is_reported_for_type_ending_with_(string ending) => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe" + ending + @"
    {
        public void SomeTest()
        {
            Assert.That(42, Is.Not.EqualTo(4711));
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_not_marked_as_test() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public void SomeTest()
        {
            Assert.That(42, Is.Not.EqualTo(4711));
        }
    }
}
");

        [Test]
        public void Code_is_not_fixed_for_test_method_not_marked_as_test_with_parameters()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(4711));
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_not_marked_as_test_without_parameters()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public void SomeTest()
        {
            Assert.That(42, Is.Not.EqualTo(4711));
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest()
        {
            Assert.That(42, Is.Not.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3128_TestMethodsHaveNoTestAttributeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3128_TestMethodsHaveNoTestAttributeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3128_CodeFixProvider();
    }
}
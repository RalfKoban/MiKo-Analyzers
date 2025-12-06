using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3122_TestMethodsDoNotHaveMultipleParametersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_method_with_3_parameters() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int a, int b, int c) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_0_parameters() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_1_parameter() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest([Values(42)] int a) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_2_parameters() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest([Values(1)] int a, [Values(2)] int b) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_3_parameters() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest([Values(1)] int a, [Values(2)] int b, [Values(3)] int c) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_4_parameters() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest([Values(1)] int a, [Values(2)] int b, [Values(3)] int c, [Values(4)] int d) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_case_method_with_1_parameter() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42)]
        public void SomeTest(int a) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_case_method_with_2_parameters() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(1, 2)]
        public void SomeTest(int a, int b) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_case_method_with_3_parameters() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(1, 2, 3)]
        public void SomeTest(int a, int b, int c) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_case_method_with_4_parameters() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(1, 2, 3, 4)]
        public void SomeTest(int a, int b, int c, int d) { }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3122_TestMethodsDoNotHaveMultipleParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3122_TestMethodsDoNotHaveMultipleParametersAnalyzer();
    }
}
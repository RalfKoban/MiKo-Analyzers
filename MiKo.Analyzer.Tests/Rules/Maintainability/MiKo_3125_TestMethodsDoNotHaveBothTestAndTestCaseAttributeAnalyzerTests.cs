using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3125_TestMethodsDoNotHaveBothTestAndTestCaseAttributeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_test_method_with_Test_attribute() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_Test_and_Combinatorial_attribute() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test, Combinatorial]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_TestCase_attribute() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42)]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Test_and_TestCase_attribute_as_different_attribute_lists() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        [TestCase(42)]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Test_and_multiple_TestCase_attribute_as_different_attribute_lists() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(0815)]
        [Test]
        [TestCase(42)]
        [TestCase(4711)]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Test_and_TestCase_attribute_in_same_attribute_list_when_Test_is_first() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test, TestCase(42)]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Test_and_TestCase_attribute_in_same_attribute_list_when_Test_is_between() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42), Test, TestCase(4711)]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Test_and_TestCase_attribute_in_same_attribute_list_when_Test_is_last() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42), Test]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_test_method_with_Test_and_TestCase_attribute_as_different_attribute_lists()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        [TestCase(42)]
        public void DoSomething(int i) { }
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
        [TestCase(42)]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Test_and_multiple_TestCase_attribute_as_different_attribute_lists()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(0815)]
        [Test]
        [TestCase(42)]
        [TestCase(4711)]
        public void DoSomething(int i) { }
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
        [TestCase(0815)]
        [TestCase(42)]
        [TestCase(4711)]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Test_and_TestCase_attribute_in_same_attribute_list_when_Test_is_first()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test, TestCase(42)]
        public void DoSomething(int i) { }
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
        [TestCase(42)]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Test_and_TestCase_attribute_in_same_attribute_list_when_Test_is_between()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42), Test, TestCase(4711)]
        public void DoSomething(int i) { }
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
        [TestCase(42), TestCase(4711)]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Test_and_TestCase_attribute_in_same_attribute_list_when_Test_is_last()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42), Test]
        public void DoSomething(int i) { }
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
        [TestCase(42)]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3125_TestMethodsDoNotHaveBothTestAndTestCaseAttributeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3125_TestMethodsDoNotHaveBothTestAndTestCaseAttributeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3125_CodeFixProvider();
    }
}
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3126_TheoryMethodsDoNotHaveTestAttributeAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_test_method_with_Fact_attribute() => No_issue_is_reported_for(@"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_Theory_attribute() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_TestCase_attribute_as_different_attribute_lists() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        [TestCase(42)]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_multiple_TestCase_attribute_as_different_attribute_lists() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(0815)]
        [Theory]
        [TestCase(42)]
        [TestCase(4711)]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_TestCase_attribute_in_same_attribute_list_when_Theory_is_first() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory, TestCase(42)]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_TestCase_attribute_in_same_attribute_list_when_Theory_is_between() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42), Theory, TestCase(4711)]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_TestCase_attribute_in_same_attribute_list_when_Theory_is_last() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42), Theory]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_Test_attribute_as_different_attribute_lists() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        [Test]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_Test_attribute_in_same_attribute_list_when_Theory_is_first() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory, Test]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_Test_attribute_in_same_attribute_list_when_Theory_is_last() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test, Theory]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_Fact_attribute_as_different_attribute_lists() => An_issue_is_reported_for(@"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory]
        [Fact]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_Fact_attribute_in_same_attribute_list_when_Theory_is_first() => An_issue_is_reported_for(@"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory, Fact]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_Fact_attribute_in_same_attribute_list_when_Theory_is_last() => An_issue_is_reported_for(@"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact, Theory]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_TestCase_attribute_as_different_attribute_lists()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
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
        [Theory]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_TestCase_and_Theory_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42)][Theory]
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
        [Theory]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_TestCase_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory][TestCase(42)]
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
        [Theory]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_multiple_TestCase_attribute_as_different_attribute_lists()
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
        [Theory]
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
        [Theory]
        [TestCase(42)]
        [TestCase(4711)]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_TestCase_attribute_in_same_attribute_list_when_Theory_is_first()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory, TestCase(42)]
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
        [Theory]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_TestCase_attribute_in_same_attribute_list_when_Theory_is_between()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42), Theory, TestCase(4711)]
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
        [Theory, TestCase(4711)]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_TestCase_attribute_in_same_attribute_list_when_Theory_is_last()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCase(42), Theory]
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
        [Theory]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Test_attribute_as_different_attribute_lists()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        [Test]
        public void DoSomething() { }
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
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Test_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory][Test]
        public void DoSomething() { }
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
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Test_and_Theory_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test][Theory]
        public void DoSomething() { }
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
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Test_attribute_in_same_attribute_list_when_Theory_is_first()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory, Test]
        public void DoSomething() { }
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
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Test_attribute_in_same_attribute_list_when_Theory_is_last()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test, Theory]
        public void DoSomething() { }
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
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Fact_attribute_as_different_attribute_lists()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory]
        [Fact]
        public void DoSomething() { }
    }
}
";
            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Fact_and_Theory_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact][Theory]
        public void DoSomething() { }
    }
}
";
            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Fact_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory][Fact]
        public void DoSomething() { }
    }
}
";
            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Fact_attribute_in_same_attribute_list_when_Theory_is_first()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory, Fact]
        public void DoSomething() { }
    }
}
";

            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Fact_attribute_in_same_attribute_list_when_Theory_is_last()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact, Theory]
        public void DoSomething() { }
    }
}
";

            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_fully_named_Theory_and_TestCase_attribute_as_different_attribute_lists()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TheoryAttribute]
        [TestCaseAttribute(42)]
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
        [TheoryAttribute]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_fully_named_TestCase_and_Theory_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestCaseAttribute(42)][TheoryAttribute]
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
        [TheoryAttribute]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_fully_named_Theory_and_TestCase_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TheoryAttribute][TestCaseAttribute(42)]
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
        [TheoryAttribute]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_fully_named_Theory_and_Test_attribute_as_different_attribute_lists()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TheoryAttribute]
        [TestAttribute]
        public void DoSomething() { }
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
        [TheoryAttribute]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_fully_named_Theory_and_Test_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TheoryAttribute][TestAttribute]
        public void DoSomething() { }
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
        [TheoryAttribute]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_fully_named_Test_and_Theory_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [TestAttribute][TheoryAttribute]
        public void DoSomething() { }
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
        [TheoryAttribute]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_fully_named_Theory_and_Fact_attribute_as_different_attribute_lists()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [TheoryAttribute]
        [FactAttribute]
        public void DoSomething() { }
    }
}
";
            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [TheoryAttribute]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_fully_named_Fact_and_Theory_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [FactAttribute][TheoryAttribute]
        public void DoSomething() { }
    }
}
";
            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [TheoryAttribute]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_fully_named_Theory_and_Fact_attribute_as_different_attribute_lists_on_same_line()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [TheoryAttribute][FactAttribute]
        public void DoSomething() { }
    }
}
";
            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [TheoryAttribute]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_Test_attribute_as_different_attribute_lists_when_Category_attribute_is_also_present() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        [Test]
        [Category(""x"")]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Theory_and_TestCase_attribute_as_different_attribute_lists_when_Category_attribute_is_also_present() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        [TestCase(42)]
        [Category(""x"")]
        public void DoSomething(int i) { }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Test_attribute_as_different_attribute_lists_when_Category_attribute_is_also_present_in_separate_list()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        [Test]
        [Category(""x"")]
        public void DoSomething() { }
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
        [Theory]
        [Category(""x"")]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_TestCase_attribute_as_different_attribute_lists_when_Category_attribute_is_also_present_in_separate_list()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        [TestCase(42)]
        [Category(""x"")]
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
        [Theory]
        [Category(""x"")]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Test_attribute_in_same_attribute_list_as_Category()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        [Test, Category(""x"")]
        public void DoSomething() { }
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
        [Theory]
        [Category(""x"")]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_TestCase_attribute_in_same_attribute_list_as_Category()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory]
        [TestCase(42), Category(""x"")]
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
        [Theory]
        [Category(""x"")]
        public void DoSomething(int i) { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Category_in_same_attribute_list_and_Test_as_separate_attribute_list()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Theory, Category(""x"")]
        [Test]
        public void DoSomething() { }
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
        [Theory, Category(""x"")]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_Theory_and_Fact_attribute_as_different_attribute_lists_when_Trait_attribute_is_also_present_in_separate_list()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory]
        [Fact]
        [Trait(""Category"", ""x"")]
        public void DoSomething() { }
    }
}
";
            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Theory]
        [Trait(""Category"", ""x"")]
        public void DoSomething() { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3126_TheoryMethodsDoNotHaveTestAttributeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3126_TheoryMethodsDoNotHaveTestAttributeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3126_CodeFixProvider();
    }
}
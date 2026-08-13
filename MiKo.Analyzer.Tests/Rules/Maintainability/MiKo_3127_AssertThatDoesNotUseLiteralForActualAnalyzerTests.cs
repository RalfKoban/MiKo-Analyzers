using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3127_AssertThatDoesNotUseLiteralForActualAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_test_method_with_parameter() => No_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_test_method_with_parameter_of_enum_type() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest([Values(StringComparison.Ordinal)] StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
");

        [TestCase("42", "4711")]
        [TestCase("-42", "-4711")]
        [TestCase("42.0", "47.11")]
        [TestCase("42f", "47.11f")]
        [TestCase("0xBB", "0xAA")]
        [TestCase("0b0000_0000", "0b1111_1111")]
        [TestCase("'a'", "'b'")]
        [TestCase("\"something\"", "\"some text\"")]
        [TestCase("true", "false")]
        [TestCase("StringComparison.Ordinal", "StringComparison.OrdinalIgnoreCase")]
        public void No_issue_is_reported_for_test_method_with_variable_(string actual, string expected) => No_issue_is_reported_for(@"
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
            var value = " + actual + @";

            Assert.That(value, Is.EqualTo(" + expected + @"));
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_parameter_as_expected() => An_issue_is_reported_for(@"
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
            Assert.That(4711, Is.EqualTo(value));
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_parameter_of_enum_type_as_expected() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest([Values(StringComparison.Ordinal)] StringComparison value)
        {
            Assert.That(StringComparison.OrdinalIgnoreCase, Is.EqualTo(value));
        }
    }
}
");

        [TestCase("42", "4711")]
        [TestCase("-42", "-4711")]
        [TestCase("42.0", "47.11")]
        [TestCase("42f", "47.11f")]
        [TestCase("0xBB", "0xAA")]
        [TestCase("0b0000_0000", "0b1111_1111")]
        [TestCase("'a'", "'b'")]
        [TestCase("\"something\"", "\"some text\"")]
        [TestCase("true", "false")]
        [TestCase("StringComparison.Ordinal", "StringComparison.OrdinalIgnoreCase")]
        public void An_issue_is_reported_for_test_method_with_variable_(string actual, string expected) => An_issue_is_reported_for(@"
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
            var value = " + actual + @";

            Assert.That(" + expected + @", Is.EqualTo(value));
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_test_method_with_parameter_as_expected()
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
        public void SomeTest([Values(42)] int value)
        {
            Assert.That(4711, Is.EqualTo(value));
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
        public void SomeTest([Values(42)] int value)
        {
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_parameter_of_enum_type_as_expected()
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
        public void SomeTest([Values(StringComparison.Ordinal)] StringComparison value)
        {
            Assert.That(StringComparison.OrdinalIgnoreCase, Is.EqualTo(value));
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
        public void SomeTest([Values(StringComparison.Ordinal)] StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("42", "4711")]
        [TestCase("-42", "-4711")]
        [TestCase("42.0", "47.11")]
        [TestCase("42f", "47.11f")]
        [TestCase("0xBB", "0xAA")]
        [TestCase("0b0000_0000", "0b1111_1111")]
        [TestCase("'a'", "'b'")]
        [TestCase("\"something\"", "\"some text\"")]
        [TestCase("true", "false")]
        [TestCase("StringComparison.Ordinal", "StringComparison.OrdinalIgnoreCase")]
        public void Code_gets_fixed_for_test_method_with_variable_(string actual, string expected)
        {
            var originalCode = @"
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
            var value = " + actual + @";

            Assert.That(" + expected + @", Is.EqualTo(value));
        }
    }
}
";

            var fixedCode = @"
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
            var value = " + actual + @";

            Assert.That(value, Is.EqualTo(" + expected + @"));
        }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("42", "4711")]
        [TestCase("-42", "-4711")]
        [TestCase("42.0", "47.11")]
        [TestCase("42f", "47.11f")]
        [TestCase("0xBB", "0xAA")]
        [TestCase("0b0000_0000", "0b1111_1111")]
        [TestCase("'a'", "'b'")]
        [TestCase("\"something\"", "\"some text\"")]
        [TestCase("true", "false")]
        [TestCase("StringComparison.Ordinal", "StringComparison.OrdinalIgnoreCase")]
        public void Code_gets_fixed_for_test_method_with_variable_when_spanning_multiple_lines_(string actual, string expected)
        {
            var originalCode = @"
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
            var value = " + actual + @";

            Assert.That(
                     " + expected + @",
                     Is.EqualTo(value));
        }
    }
}
";

            var fixedCode = @"
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
            var value = " + actual + @";

            Assert.That(
                     value,
                     Is.EqualTo(" + expected + @"));
        }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_variable_and_chained_IgnoreCase()
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
        public void SomeTest()
        {
            var value = ""something"";

            Assert.That(""some text"", Is.EqualTo(value).IgnoreCase);
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
            var value = ""something"";

            Assert.That(value, Is.EqualTo(""some text"").IgnoreCase);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_variable_and_chained_Within()
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
        public void SomeTest()
        {
            var value = 42.0;

            Assert.That(47.11, Is.EqualTo(value).Within(0.5));
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
            var value = 42.0;

            Assert.That(value, Is.EqualTo(47.11).Within(0.5));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void An_issue_is_reported_for_test_method_with_variable_and_Is_Not_EqualTo() => An_issue_is_reported_for(@"
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
            var value = 42;

            Assert.That(4711, Is.Not.EqualTo(value));
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_test_method_with_variable_and_Is_Not_EqualTo()
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
        public void SomeTest()
        {
            var value = 42;

            Assert.That(4711, Is.Not.EqualTo(value));
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
            var value = 42;

            Assert.That(value, Is.Not.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_variable_and_Is_Not_EqualTo_and_chained_IgnoreCase()
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
        public void SomeTest()
        {
            var value = ""something"";

            Assert.That(""some text"", Is.Not.EqualTo(value).IgnoreCase);
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
            var value = ""something"";

            Assert.That(value, Is.Not.EqualTo(""some text"").IgnoreCase);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_variable_and_Is_Not_EqualTo_and_chained_Within()
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
        public void SomeTest()
        {
            var value = 42.0;

            Assert.That(47.11, Is.Not.EqualTo(value).Within(0.5));
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
            var value = 42.0;

            Assert.That(value, Is.Not.EqualTo(47.11).Within(0.5));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_variable_and_Is_EqualTo_and_chained_Within_with_nested_invocation()
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
        public void SomeTest()
        {
            var value = 42.0;

            Assert.That(47.11, Is.EqualTo(value).Within(GetTolerance()));
        }

        private double GetTolerance() => 0.5;
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
            var value = 42.0;

            Assert.That(value, Is.EqualTo(47.11).Within(GetTolerance()));
        }

        private double GetTolerance() => 0.5;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3127_AssertThatDoesNotUseLiteralForActualAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3127_AssertThatDoesNotUseLiteralForActualAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3127_CodeFixProvider();
    }
}
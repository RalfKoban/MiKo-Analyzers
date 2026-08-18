using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public partial class MiKo_3130_AssertFailInsideIfBlockAnalyzerTests
    {
        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is()
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
        public void SomeTest(int value)
        {
            if (value is 42)
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_block_with_is()
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
        public void SomeTest(int value)
        {
            if (value is 42)
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is()
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
        public void SomeTest(int value)
        {
            if (value is 42)
                Assert.That(value, Is.EqualTo(4711));
            else
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is()
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
        public void SomeTest(int value)
        {
            if (value is 42)
                Assert.That(value, Is.EqualTo(4711));
            else
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is()
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
        public void SomeTest(int value)
        {
            if (value is 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is()
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
        public void SomeTest(int value)
        {
            if (value is 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_more_items_before_if_block_with_is()
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
            var value = 0815;

            if (value is 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
                Assert.Fail(""some failure"");
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
            var value = 0815;

            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_more_items_before_if_block_with_is()
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
            var value = 0815;

            if (value is 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
            {
                Assert.Fail(""some failure"");
            }
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
            var value = 0815;

            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is_not()
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
        public void SomeTest(int value)
        {
            if (value is not 42)
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_block_with_is_not()
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
        public void SomeTest(int value)
        {
            if (value is not 42)
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is_not()
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
        public void SomeTest(int value)
        {
            if (value is not 42)
                Assert.That(value, Is.EqualTo(4711));
            else
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is_not()
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
        public void SomeTest(int value)
        {
            if (value is not 42)
                Assert.That(value, Is.EqualTo(4711));
            else
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is_not()
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
        public void SomeTest(int value)
        {
            if (value is not 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is_not()
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
        public void SomeTest(int value)
        {
            if (value is not 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_more_items_before_if_block_with_is_not()
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
            var value = 0815;

            if (value is not 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
                Assert.Fail(""some failure"");
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
            var value = 0815;

            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_more_items_before_if_block_with_is_not()
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
            var value = 0815;

            if (value is not 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
            {
                Assert.Fail(""some failure"");
            }
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
            var value = 0815;

            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_equals()
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
        public void SomeTest(int value)
        {
            if (value == 42)
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_block_with_equals()
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
        public void SomeTest(int value)
        {
            if (value == 42)
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_equals()
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
        public void SomeTest(int value)
        {
            if (value == 42)
                Assert.That(value, Is.EqualTo(4711));
            else
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_equals()
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
        public void SomeTest(int value)
        {
            if (value == 42)
                Assert.That(value, Is.EqualTo(4711));
            else
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_equals()
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
        public void SomeTest(int value)
        {
            if (value == 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_equals()
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
        public void SomeTest(int value)
        {
            if (value == 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_more_items_before_if_block_with_equals()
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
            var value = 0815;

            if (value == 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
                Assert.Fail(""some failure"");
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
            var value = 0815;

            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_more_items_before_if_block_with_equals()
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
            var value = 0815;

            if (value == 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
            {
                Assert.Fail(""some failure"");
            }
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
            var value = 0815;

            Assert.That(value, Is.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_not_equals()
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
        public void SomeTest(int value)
        {
            if (value != 42)
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_block_with_not_equals()
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
        public void SomeTest(int value)
        {
            if (value != 42)
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.EqualTo(42), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_not_equals()
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
        public void SomeTest(int value)
        {
            if (value != 42)
                Assert.That(value, Is.EqualTo(4711));
            else
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_not_equals()
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
        public void SomeTest(int value)
        {
            if (value != 42)
                Assert.That(value, Is.EqualTo(4711));
            else
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_not_equals()
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
        public void SomeTest(int value)
        {
            if (value != 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
                Assert.Fail(""some failure"");
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_not_equals()
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
        public void SomeTest(int value)
        {
            if (value != 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
            {
                Assert.Fail(""some failure"");
            }
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
        public void SomeTest(int value)
        {
            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_more_items_before_if_block_with_not_equals()
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
            var value = 0815;

            if (value != 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
                Assert.Fail(""some failure"");
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
            var value = 0815;

            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_more_items_before_if_block_with_not_equals()
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
            var value = 0815;

            if (value != 42)
            {
                Assert.That(value, Is.EqualTo(4711));
            }
            else
            {
                Assert.Fail(""some failure"");
            }
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
            var value = 0815;

            Assert.That(value, Is.Not.EqualTo(42), ""some failure"");
            Assert.That(value, Is.EqualTo(4711));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}

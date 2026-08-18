using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public partial class MiKo_3130_AssertFailInsideIfBlockAnalyzerTests
    {
        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is StringComparison.Ordinal)
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_block_with_is_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is StringComparison.Ordinal)
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is StringComparison.Ordinal)
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is StringComparison.Ordinal)
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_more_items_before_if_block_with_is_StringComparison_Ordinal()
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
            var value = StringComparison.InvariantCulture;

            if (value is StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
            var value = StringComparison.InvariantCulture;

            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_more_items_before_if_block_with_is_StringComparison_Ordinal()
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
            var value = StringComparison.InvariantCulture;

            if (value is StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
            var value = StringComparison.InvariantCulture;

            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is_not_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is not StringComparison.Ordinal)
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_block_with_is_not_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is not StringComparison.Ordinal)
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is_not_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is not StringComparison.Ordinal)
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is_not_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is not StringComparison.Ordinal)
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is_not_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is not StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is_not_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value is not StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_more_items_before_if_block_with_is_not_StringComparison_Ordinal()
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
            var value = StringComparison.InvariantCulture;

            if (value is not StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
            var value = StringComparison.InvariantCulture;

            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_more_items_before_if_block_with_is_not_StringComparison_Ordinal()
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
            var value = StringComparison.InvariantCulture;

            if (value is not StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
            var value = StringComparison.InvariantCulture;

            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value == StringComparison.Ordinal)
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_block_with_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value == StringComparison.Ordinal)
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value == StringComparison.Ordinal)
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value == StringComparison.Ordinal)
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value == StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value == StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_more_items_before_if_block_with_equals_StringComparison_Ordinal()
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
            var value = StringComparison.InvariantCulture;

            if (value == StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
            var value = StringComparison.InvariantCulture;

            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_more_items_before_if_block_with_equals_StringComparison_Ordinal()
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
            var value = StringComparison.InvariantCulture;

            if (value == StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
            var value = StringComparison.InvariantCulture;

            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_not_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value != StringComparison.Ordinal)
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_block_with_not_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value != StringComparison.Ordinal)
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_not_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value != StringComparison.Ordinal)
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_not_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value != StringComparison.Ordinal)
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_not_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value != StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_not_equals_StringComparison_Ordinal()
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
        public void SomeTest(StringComparison value)
        {
            if (value != StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_more_items_before_if_block_with_not_equals_StringComparison_Ordinal()
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
            var value = StringComparison.InvariantCulture;

            if (value != StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
            var value = StringComparison.InvariantCulture;

            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_more_items_before_if_block_with_not_equals_StringComparison_Ordinal()
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
            var value = StringComparison.InvariantCulture;

            if (value != StringComparison.Ordinal)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
            var value = StringComparison.InvariantCulture;

            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_equals_StringComparison_Ordinal_and_value_on_left_side()
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
        public void SomeTest(StringComparison value)
        {
            if (StringComparison.Ordinal == value)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_not_equals_StringComparison_Ordinal_and_value_on_left_side()
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
        public void SomeTest(StringComparison value)
        {
            if (StringComparison.Ordinal != value)
            {
                Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
        public void SomeTest(StringComparison value)
        {
            Assert.That(value, Is.Not.EqualTo(StringComparison.Ordinal), ""some failure"");
            Assert.That(value, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}

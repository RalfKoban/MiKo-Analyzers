using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public partial class MiKo_3130_AssertFailInsideIfBlockAnalyzerTests
    {
        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_equals_null()
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
        public void SomeTest(object value)
        {
            if (value == null)
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_not_equals_null()
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
        public void SomeTest(object value)
        {
            if (value != null)
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is_null()
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
        public void SomeTest(object value)
        {
            if (value is null)
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is_not_null()
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
        public void SomeTest(object value)
        {
            if (value is not null)
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_equals_null()
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
        public void SomeTest(object value)
        {
            if (value == null)
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_equals_null()
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
        public void SomeTest(object value)
        {
            if (value == null)
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_equals_null()
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
        public void SomeTest(object value)
        {
            if (value == null)
            {
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_equals_null()
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
        public void SomeTest(object value)
        {
            if (value == null)
            {
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_not_equals_null()
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
        public void SomeTest(object value)
        {
            if (value != null)
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_not_equals_null()
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
        public void SomeTest(object value)
        {
            if (value != null)
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_not_equals_null()
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
        public void SomeTest(object value)
        {
            if (value != null)
            {
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_not_equals_null()
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
        public void SomeTest(object value)
        {
            if (value != null)
            {
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is_null()
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
        public void SomeTest(object value)
        {
            if (value is null)
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is_null()
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
        public void SomeTest(object value)
        {
            if (value is null)
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is_null()
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
        public void SomeTest(object value)
        {
            if (value is null)
            {
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is_null()
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
        public void SomeTest(object value)
        {
            if (value is null)
            {
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is_not_null()
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
        public void SomeTest(object value)
        {
            if (value is not null)
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is_not_null()
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
        public void SomeTest(object value)
        {
            if (value is not null)
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is_not_null()
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
        public void SomeTest(object value)
        {
            if (value is not null)
            {
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is_not_null()
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
        public void SomeTest(object value)
        {
            if (value is not null)
            {
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_null_equals()
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
        public void SomeTest(object value)
        {
            if (null == value)
            {
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_null_not_equals()
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
        public void SomeTest(object value)
        {
            if (null != value)
                Assert.That(42, Is.EqualTo(0815));
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
        public void SomeTest(object value)
        {
            Assert.That(value, Is.Not.Null, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
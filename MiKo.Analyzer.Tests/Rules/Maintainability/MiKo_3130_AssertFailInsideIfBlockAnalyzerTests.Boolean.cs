//// ncrunch: rdi off
using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public partial class MiKo_3130_AssertFailInsideIfBlockAnalyzerTests
    {
        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value == true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value == false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is_true()
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
        public void SomeTest(bool value)
        {
            if (value is true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is_false()
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
        public void SomeTest(bool value)
        {
            if (value is false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_not_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value != true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_not_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value != false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is_not_true()
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
        public void SomeTest(bool value)
        {
            if (value is not true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_if_statement_with_is_not_false()
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
        public void SomeTest(bool value)
        {
            if (value is not false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value == true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value == true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value == true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value == true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value == false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value == false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value == false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value == false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is_true()
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
        public void SomeTest(bool value)
        {
            if (value is true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is_true()
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
        public void SomeTest(bool value)
        {
            if (value is true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is_true()
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
        public void SomeTest(bool value)
        {
            if (value is true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is_true()
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
        public void SomeTest(bool value)
        {
            if (value is true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is_false()
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
        public void SomeTest(bool value)
        {
            if (value is false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is_false()
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
        public void SomeTest(bool value)
        {
            if (value is false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is_false()
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
        public void SomeTest(bool value)
        {
            if (value is false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is_false()
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
        public void SomeTest(bool value)
        {
            if (value is false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_not_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value != true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_not_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value != true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_not_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value != true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_not_equals_true()
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
        public void SomeTest(bool value)
        {
            if (value != true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_not_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value != false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_not_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value != false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_not_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value != false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_not_equals_false()
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
        public void SomeTest(bool value)
        {
            if (value != false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is_not_true()
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
        public void SomeTest(bool value)
        {
            if (value is not true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is_not_true()
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
        public void SomeTest(bool value)
        {
            if (value is not true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is_not_true()
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
        public void SomeTest(bool value)
        {
            if (value is not true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is_not_true()
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
        public void SomeTest(bool value)
        {
            if (value is not true)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.False, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_no_block_as_if_statement_with_is_not_false()
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
        public void SomeTest(bool value)
        {
            if (value is not false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_no_block_as_if_statement_with_is_not_false()
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
        public void SomeTest(bool value)
        {
            if (value is not false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_statement_with_is_not_false()
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
        public void SomeTest(bool value)
        {
            if (value is not false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Assert_Fail_as_only_statement_in_else_block_with_is_not_false()
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
        public void SomeTest(bool value)
        {
            if (value is not false)
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
        public void SomeTest(bool value)
        {
            Assert.That(value, Is.True, ""some failure"");
            Assert.That(42, Is.EqualTo(0815));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
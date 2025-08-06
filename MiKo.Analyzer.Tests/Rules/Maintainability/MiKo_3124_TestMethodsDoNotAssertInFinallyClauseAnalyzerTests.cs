using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3124_TestMethodsDoNotAssertInFinallyClauseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_method_with_empty_finally_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            try
            {
            }
            finally
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_method_with_empty_finally_block_and_assertion_in_try_block() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            try
            {
                Assert.Fail(""for reasons"");
            }
            finally
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_method_with_code_in_finally_block_and_assertion_in_try_block() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            IDisposable disposable = null;

            try
            {
                Assert.Fail(""for reasons"");
            }
            finally
            {
                disposable?.Dispose();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_empty_finally_block() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            try
            {
            }
            finally
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_empty_finally_block_and_assertion_in_try_block() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            try
            {
                Assert.Fail(""for reasons"");
            }
            finally
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_code_in_finally_block_and_assertion_in_try_block() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            IDisposable disposable = null;

            try
            {
                Assert.Fail(""for reasons"");
            }
            finally
            {
                disposable?.Dispose();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_test_method_with_code_and_assertion_at_start_of_finally_block() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                Assert.Fail(""for reasons"");

                disposable?.Dispose();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_test_method_with_code_and_assertion_in_the_middle_of_finally_block() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();

                Assert.Fail(""for reasons"");

                disposable?.Dispose();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_test_method_with_code_and_assertion_at_end_of_finally_block() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();

                Assert.Fail(""for reasons"");
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_code_and_assertion_at_start_of_finally_block() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                Assert.Fail(""for reasons"");

                disposable?.Dispose();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_code_and_assertion_in_the_middle_of_finally_block() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();

                Assert.Fail(""for reasons"");

                disposable?.Dispose();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_code_and_assertion_at_end_of_finally_block() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();

                Assert.Fail(""for reasons"");
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_assertion_as_only_statement_in_finally_block()
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
        public void DoSomething()
        {
            try
            {
                // do something
            }
            finally
            {
                Assert.Fail(""for reasons"");
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
        public void DoSomething()
        {
            try
            {
                // do something
            }
            finally
            {
            }

            Assert.Fail(""for reasons"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assertion_at_start_of_finally_block()
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
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                Assert.Fail(""for reasons"");

                disposable?.Dispose();
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
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();
            }

            Assert.Fail(""for reasons"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assertion_in_the_middle_of_finally_block()
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
        public void DoSomething()
        {
            IDisposable disposable1, disposable2;

            try
            {
                disposable1 = disposable2 = null;
            }
            finally
            {
                disposable1?.Dispose();

                Assert.Fail(""for reasons"");

                disposable2?.Dispose();
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
        public void DoSomething()
        {
            IDisposable disposable1, disposable2;

            try
            {
                disposable1 = disposable2 = null;
            }
            finally
            {
                disposable1?.Dispose();
                disposable2?.Dispose();
            }

            Assert.Fail(""for reasons"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assertion_at_end_of_finally_block()
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
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();

                Assert.Fail(""for reasons"");
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
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();
            }

            Assert.Fail(""for reasons"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assertion_in_nested_block_at_end_of_finally_block()
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
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();

                {
                    Assert.Fail(""for reasons"");
                }
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
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();
            }

            Assert.Fail(""for reasons"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assertion_in_nested_block_at_start_of_finally_block()
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
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                {
                    Assert.Fail(""for reasons"");
                }

                disposable?.Dispose();
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
        public void DoSomething()
        {
            IDisposable disposable;

            try
            {
                disposable = null;
            }
            finally
            {
                disposable?.Dispose();
            }

            Assert.Fail(""for reasons"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3124_TestMethodsDoNotAssertInFinallyClauseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3124_TestMethodsDoNotAssertInFinallyClauseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3124_CodeFixProvider();
    }
}
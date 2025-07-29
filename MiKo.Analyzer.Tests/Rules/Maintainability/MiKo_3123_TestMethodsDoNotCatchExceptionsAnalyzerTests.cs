using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3123_TestMethodsDoNotCatchExceptionsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_method_with_no_catch_block() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_non_test_method_with_catch_block() => No_issue_is_reported_for(@"
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
            catch
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_method_with_catch_exception_block() => No_issue_is_reported_for(@"
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
            catch (Exception)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_method_with_catch_exception_ex_block() => No_issue_is_reported_for(@"
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
            catch (Exception ex)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_no_catch_block() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_test_method_with_catch_block() => An_issue_is_reported_for(@"
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
            catch
            {
            }
        }
    }
}
");

        [Test]
        public void Ano_issue_is_reported_for_test_method_with_catch_exception_block() => An_issue_is_reported_for(@"
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
            catch (Exception)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_catch_exception_ex_block() => An_issue_is_reported_for(@"
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
            catch (Exception ex)
            {
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_catch_exception_ex_block()
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
        public void DoSomething(String s)
        {
            try
            {
                var x = 123;
                var y = x.ToString();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
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
        public void DoSomething(String s)
        {
            Assert.That(() =>
            {
                var x = 123;
                var y = x.ToString();
            }, Throws.Nothing);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_catch_exception_ex_and_finally_block()
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
        public void DoSomething(String s)
        {
            try
            {
                var x = 123;
                var y = x.ToString();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
            finally
            {
                var z = 567;
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
        public void DoSomething(String s)
        {
            try
            {
                Assert.That(() =>
                {
                    var x = 123;
                    var y = x.ToString();
                }, Throws.Nothing);
            }
            finally
            {
                var z = 567;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_xUnit_test_method_with_catch_exception_ex_block()
        {
            const string OriginalCode = @"
using System;

using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public void DoSomething(String s)
        {
            try
            {
                var x = 123;
                var y = x.ToString();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }
    }
}
";

            const string FixedCode = @"
using System;

using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public void DoSomething(String s)
        {
            var x = 123;
            var y = x.ToString();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_xUnit_test_method_with_catch_exception_ex_and_finally_block()
        {
            const string OriginalCode = @"
using System;

using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public void DoSomething(String s)
        {
            try
            {
                var x = 123;
                var y = x.ToString();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
            finally
            {
                var z = 567;
            }
        }
    }
}
";

            const string FixedCode = @"
using System;

using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public void DoSomething(String s)
        {
            try
            {
                var x = 123;
                var y = x.ToString();
            }
            finally
            {
                var z = 567;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3123_TestMethodsDoNotCatchExceptionsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3123_TestMethodsDoNotCatchExceptionsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3123_CodeFixProvider();
    }
}
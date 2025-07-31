using System;
using System.Reflection;

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
        public void Code_gets_fixed_for_NUnit_test_method_with_assertion_in_catch_block()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public enum Severity { None, Fatal, Whatever }

    public class CustomException : Exception
    {
        public Severity Severity { get; }
    }

    [TestFixture]
    public class TestMe
    {
        private int _transactionCreationCounter = 0;
        private int _closeTransactionCounter = 0;
        private int _rollbackCounter = 0;
        private Exception innerOperationException = null;

        [Test]
        public void DoSomething(String s)
        {
            try
            {
                var a = 123;
                var b = a.ToString();
            }
            catch (CustomException ex)
            {
                // verify
                Assert.That(ex.InnerException, Is.EqualTo(innerOperationException));
                Assert.That(ex.Severity, Is.EqualTo(Severity.Fatal));
                Assert.That(_transactionCreationCounter, Is.EqualTo(1));
                Assert.That(_closeTransactionCounter, Is.EqualTo(1));
                Assert.That(_rollbackCounter, Is.EqualTo(1));
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
    public enum Severity { None, Fatal, Whatever }

    public class CustomException : Exception
    {
        public Severity Severity { get; }
    }

    [TestFixture]
    public class TestMe
    {
        private int _transactionCreationCounter = 0;
        private int _closeTransactionCounter = 0;
        private int _rollbackCounter = 0;
        private Exception innerOperationException = null;

        [Test]
        public void DoSomething(String s)
        {
            Assert.That(() =>
            {
                var a = 123;
                var b = a.ToString();
            }, Throws.TypeOf<CustomException>().With.InnerException.EqualTo(innerOperationException).And.Property(nameof(CustomException.Severity)).EqualTo(Severity.Fatal));

            Assert.That(_transactionCreationCounter, Is.EqualTo(1));
            Assert.That(_closeTransactionCounter, Is.EqualTo(1));
            Assert.That(_rollbackCounter, Is.EqualTo(1));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_Fail_in_catch_block()
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
                var a = 123;
                var b = a.ToString();
            }
            catch
            {
                Assert.Fail(""should never happen"");
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
                var a = 123;
                var b = a.ToString();
            }, Throws.Nothing);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_Fail_in_try_block_and_no_exception_in_catch_block()
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
                var a = 123;
                var b = a.ToString();

                Assert.Fail(""some error message"");
            }
            catch
            {
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
                var a = 123;
                var b = a.ToString();
            }, Throws.Exception, ""some error message"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_Fail_in_try_block_and_catch_block_with_custom_exception()
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
                var a = 123;
                var b = a.ToString();

                Assert.Fail(""should never happen"");
            }
            catch (NotSupportedException)
            {
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
                var a = 123;
                var b = a.ToString();
            }, Throws.TypeOf<NotSupportedException>(), ""should never happen"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase(nameof(Exception))]
        [TestCase(nameof(ArgumentException))]
        [TestCase(nameof(ArgumentNullException))]
        [TestCase(nameof(InvalidOperationException))]
        [TestCase(nameof(TargetInvocationException))]
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_Fail_in_try_block_and_catch_block_with_well_known_exception_(string exceptionName)
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
        public void DoSomething(String s)
        {
            try
            {
                var a = 123;
                var b = a.ToString();

                Assert.Fail(""should never happen"");
            }
            catch (" + exceptionName + @")
            {
            }
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
        public void DoSomething(String s)
        {
            Assert.That(() =>
            {
                var a = 123;
                var b = a.ToString();
            }, Throws." + exceptionName + @", ""should never happen"");
        }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_catch_exception_block()
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
                var a = 123;
                var b = a.ToString();
            }
            catch (Exception)
            {
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
                var a = 123;
                var b = a.ToString();
            }, Throws.Nothing);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

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
                var a = 123;
                var b = a.ToString();
            }
            catch (Exception ex)
            {
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
                var a = 123;
                var b = a.ToString();
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
                var a = 123;
                var b = a.ToString();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                var c = 567;
                var d = c.ToString();
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
                    var a = 123;
                    var b = a.ToString();
                }, Throws.Nothing);
            }
            finally
            {
                var c = 567;
                var d = c.ToString();
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_xUnit_test_method_with_assertion_in_catch_block()
        {
            const string OriginalCode = @"
using System;

using Xunit;

namespace Bla
{
    public class CustomException : Exception { }

    public class TestMe
    {
        [Fact]
        public void DoSomething(String s)
        {
            try
            {
                var a = 123;
                var b = a.ToString();
            }
            catch (CustomException ex)
            {
                // verify
                Assert.Equal(s, ex.Message);
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
    public class CustomException : Exception { }

    public class TestMe
    {
        [Fact]
        public void DoSomething(String s)
        {
            CustomException ex = Assert.Throws<CustomException>(() =>
            {
                var a = 123;
                var b = a.ToString();
            });

            // verify
            Assert.Equal(s, ex.Message);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_xUnit_test_method_with_Assert_Fail_in_catch_block()
        {
            const string OriginalCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public void DoSomething(string s)
        {
            try
            {
                var a = 123;
                var b = a.ToString();
            }
            catch
            {
                Assert.Fail(""should never happen"");
            }
        }
    }
}
";

            const string FixedCode = @"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public void DoSomething(string s)
        {
            var a = 123;
            var b = a.ToString();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_xUnit_test_method_with_Assert_Fail_in_try_block_and_no_exception_in_catch_block()
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
                var a = 123;
                var b = a.ToString();

                Assert.Fail(""some error message"");
            }
            catch
            {
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
            Assert.Throws<Exception>(() =>
            {
                var a = 123;
                var b = a.ToString();
            });
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_xUnit_test_method_with_Assert_Fail_in_try_block_and_catch_block_with_custom_exception()
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
                var a = 123;
                var b = a.ToString();

                Assert.Fail(""should never happen"");
            }
            catch (NotSupportedException)
            {
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
            Assert.Throws<NotSupportedException>(() =>
            {
                var a = 123;
                var b = a.ToString();
            });
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_xUnit_test_method_with_catch_exception_block()
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
                var a = 123;
                var b = a.ToString();
            }
            catch (Exception)
            {
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
            var a = 123;
            var b = a.ToString();
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
                var a = 123;
                var b = a.ToString();
            }
            catch (Exception ex)
            {
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
            var a = 123;
            var b = a.ToString();
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
                var a = 123;
                var b = a.ToString();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                var c = 567;
                var d = c.ToString();
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
                var a = 123;
                var b = a.ToString();
            }
            finally
            {
                var c = 567;
                var d = c.ToString();
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        //// TODO RKN:
        //// for xUnit use 'Assert.ThrowsAsync()' and change method to 'async Task' (incl. adjustment of using)

        protected override string GetDiagnosticId() => MiKo_3123_TestMethodsDoNotCatchExceptionsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3123_TestMethodsDoNotCatchExceptionsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3123_CodeFixProvider();
    }
}
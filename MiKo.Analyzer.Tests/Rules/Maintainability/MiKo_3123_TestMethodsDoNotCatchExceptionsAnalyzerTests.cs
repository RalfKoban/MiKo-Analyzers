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
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_That_EqualTo_assertions_in_catch_block()
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
                Assert.That(ex.HResult, Is.EqualTo(42));
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
            }, Throws.TypeOf<CustomException>().With.InnerException.EqualTo(innerOperationException).And.Property(nameof(CustomException.Severity)).EqualTo(Severity.Fatal).And.Property(nameof(CustomException.HResult)).EqualTo(42));

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
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_That_Not_EqualTo_assertions_in_catch_block()
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
                Assert.That(ex.InnerException, Is.Not.EqualTo(innerOperationException));
                Assert.That(ex.Severity, Is.Not.EqualTo(Severity.Fatal));
                Assert.That(ex.HResult, Is.EqualTo(42));
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
            }, Throws.TypeOf<CustomException>().With.InnerException.Not.EqualTo(innerOperationException).And.Property(nameof(CustomException.Severity)).Not.EqualTo(Severity.Fatal).And.Property(nameof(CustomException.HResult)).EqualTo(42));

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
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_AreEqual_assertions_in_catch_block()
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
                Assert.AreEqual(innerOperationException, ex.InnerException);
                Assert.AreEqual(Severity.Fatal, ex.Severity);
                Assert.AreEqual(42, ex.HResult);
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
        private Exception innerOperationException = null;

        [Test]
        public void DoSomething(String s)
        {
            Assert.That(() =>
            {
                var a = 123;
                var b = a.ToString();
            }, Throws.TypeOf<CustomException>().With.InnerException.EqualTo(innerOperationException).And.Property(nameof(CustomException.Severity)).EqualTo(Severity.Fatal).And.Property(nameof(CustomException.HResult)).EqualTo(42));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_AreNotEqual_assertions_in_catch_block()
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
                Assert.AreNotEqual(innerOperationException, ex.InnerException);
                Assert.AreNotEqual(Severity.Fatal, ex.Severity);
                Assert.AreNotEqual(42, ex.HResult);
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
        private Exception innerOperationException = null;

        [Test]
        public void DoSomething(String s)
        {
            Assert.That(() =>
            {
                var a = 123;
                var b = a.ToString();
            }, Throws.TypeOf<CustomException>().With.InnerException.Not.EqualTo(innerOperationException).And.Property(nameof(CustomException.Severity)).Not.EqualTo(Severity.Fatal).And.Property(nameof(CustomException.HResult)).Not.EqualTo(42));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_swapped_Assert_AreEqual_assertions_in_catch_block()
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
                Assert.AreEqual(ex.InnerException, innerOperationException);
                Assert.AreEqual(ex.Severity, Severity.Fatal);
                Assert.AreEqual(ex.HResult, 42);
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
        private Exception innerOperationException = null;

        [Test]
        public void DoSomething(String s)
        {
            Assert.That(() =>
            {
                var a = 123;
                var b = a.ToString();
            }, Throws.TypeOf<CustomException>().With.InnerException.EqualTo(innerOperationException).And.Property(nameof(CustomException.Severity)).EqualTo(Severity.Fatal).And.Property(nameof(CustomException.HResult)).EqualTo(42));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_swapped_Assert_AreNotEqual_assertions_in_catch_block()
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
                Assert.AreNotEqual(ex.InnerException, innerOperationException);
                Assert.AreNotEqual(ex.Severity, Severity.Fatal);
                Assert.AreNotEqual(ex.HResult, 42);
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
        private Exception innerOperationException = null;

        [Test]
        public void DoSomething(String s)
        {
            Assert.That(() =>
            {
                var a = 123;
                var b = a.ToString();
            }, Throws.TypeOf<CustomException>().With.InnerException.Not.EqualTo(innerOperationException).And.Property(nameof(CustomException.Severity)).Not.EqualTo(Severity.Fatal).And.Property(nameof(CustomException.HResult)).Not.EqualTo(42));
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
                Assert.Fail(""unexpected exception"");
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
            }, Throws.Nothing, ""unexpected exception"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_Fail_in_catch_exception_block()
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
                Assert.Fail(""unexpected exception"");
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
            }, Throws.Nothing, ""unexpected exception"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_Fail_in_catch_exception_ex_block()
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
                Assert.Fail(""unexpected exception"");
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
            }, Throws.Nothing, ""unexpected exception"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_NUnit_test_method_with_Assert_Fail_in_catch_exception_ex_block_and_usage_of_exception_in_failure_message()
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
                Assert.Fail(""unexpected exception:\n{0}\n{1}\n{2}"", ex.GetType().Name, ex.Message, ex.Stacktrace);
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
            }, Throws.Nothing, ""unexpected exception:\n{0}\n{1}\n{2}"", ex.GetType().Name, ex.Message, ex.Stacktrace);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode, allowNewCompilerDiagnostics: true); // 'ex' is no longer valid, needs to be manually fixed by developers
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

                Assert.Fail(""unexpected exception"");
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
            }, Throws.TypeOf<NotSupportedException>(), ""unexpected exception"");
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

                Assert.Fail(""unexpected exception"");
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
            }, Throws." + exceptionName + @", ""unexpected exception"");
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
        public void Code_gets_fixed_for_NUnit_test_method_with_assertion_in_catch_block_and_an_additional_finally_block()
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
                Assert.That(s, Is.EqualTo(""Some value""));
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
                }, Throws.Exception);
                Assert.That(s, Is.EqualTo(""Some value""));
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
        public void Code_gets_fixed_for_Xunit_test_method_with_assertion_in_catch_block()
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
        public void Code_gets_fixed_for_Xunit_test_method_with_Assert_Fail_in_catch_block()
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
                Assert.Fail(""unexpected exception"");
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
        public void Code_gets_fixed_for_Xunit_test_method_with_Assert_Fail_in_try_block_and_no_exception_in_catch_block()
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
        public void Code_gets_fixed_for_Xunit_test_method_with_Assert_Fail_in_try_block_and_catch_block_with_custom_exception()
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

                Assert.Fail(""unexpected exception"");
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
        public void Code_gets_fixed_for_Xunit_test_method_with_catch_exception_block()
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
        public void Code_gets_fixed_for_Xunit_test_method_with_catch_exception_ex_block()
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
        public void Code_gets_fixed_for_Xunit_test_method_with_catch_exception_ex_and_finally_block()
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

        [Test]
        public void Code_gets_fixed_for_Xunit_test_method_with_assertion_in_catch_block_and_an_additional_finally_block()
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
                Assert.Equal(""Some value"", s);
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
                Exception ex = Assert.Throws<Exception>(() =>
                {
                    var a = 123;
                    var b = a.ToString();
                });
                Assert.Equal(""Some value"", s);
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
        public void Code_gets_fixed_for_Xunit_async_test_method_with_assertion_in_catch_block()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

using Xunit;

namespace Bla
{
    public class CustomException : Exception { }

    public class TestMe
    {
        [Fact]
        public async Task DoSomething(String s)
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
using System.Threading.Tasks;

using Xunit;

namespace Bla
{
    public class CustomException : Exception { }

    public class TestMe
    {
        [Fact]
        public async Task DoSomething(String s)
        {
            CustomException ex = await Assert.ThrowsAsync<CustomException>(() =>
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
        public void Code_gets_fixed_for_Xunit_async_test_method_with_Assert_Fail_in_try_block_and_no_exception_in_catch_block()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public async Task DoSomething(String s)
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
using System.Threading.Tasks;

using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public async Task DoSomething(String s)
        {
            await Assert.ThrowsAsync<Exception>(() =>
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
        public void Code_gets_fixed_for_Xunit_test_method_with_throw_statement()
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

                throw new CustomException();
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
using System.Threading.Tasks;
using Xunit;

namespace Bla
{
    public class CustomException : Exception { }

    public class TestMe
    {
        [Fact]
        public async Task DoSomething(String s)
        {
            CustomException ex = await Assert.ThrowsAsync<CustomException>(() =>
            {
                var a = 123;
                var b = a.ToString();

                throw new CustomException();
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
        public void Code_gets_fixed_for_Xunit_async_test_method_with_throw_statement()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

using Xunit;

namespace Bla
{
    public class CustomException : Exception { }

    public class TestMe
    {
        [Fact]
        public async Task DoSomething(String s)
        {
            try
            {
                var a = 123;
                var b = a.ToString();

                throw new CustomException();
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
using System.Threading.Tasks;

using Xunit;

namespace Bla
{
    public class CustomException : Exception { }

    public class TestMe
    {
        [Fact]
        public async Task DoSomething(String s)
        {
            CustomException ex = await Assert.ThrowsAsync<CustomException>(() =>
            {
                var a = 123;
                var b = a.ToString();

                throw new CustomException();
            });

            // verify
            Assert.Equal(s, ex.Message);
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
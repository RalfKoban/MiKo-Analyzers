using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3129_AssertThatDoesNotUseAwaitForActualAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_test_method_with_method_call() => No_issue_is_reported_for(@"
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
            Assert.That(DoSomething(), Is.EqualTo(4711));
        }

        private int DoSomething() => 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_awaited_method_call() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest()
        {
            Assert.That(await DoSomethingAsync(), Is.EqualTo(4711));
        }

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_awaited_method_call_with_ConfigureAwait() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest()
        {
            Assert.That(await DoSomethingAsync().ConfigureAwait(false), Is.EqualTo(4711));
        }

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_test_method_with_awaited_method_call_as_expression_body()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest() => Assert.That(await DoSomethingAsync(), Is.EqualTo(4711));

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest()
        {
            var awaitedResult = await DoSomethingAsync();

            Assert.That(awaitedResult, Is.EqualTo(4711));
        }

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_awaited_method_call()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest()
        {
            Assert.That(await DoSomethingAsync(), Is.EqualTo(4711));
        }

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest()
        {
            var awaitedResult = await DoSomethingAsync();

            Assert.That(awaitedResult, Is.EqualTo(4711));
        }

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_awaited_method_call_with_ConfigureAwait()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest()
        {
            Assert.That(await DoSomethingAsync().ConfigureAwait(false), Is.EqualTo(4711));
        }

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest()
        {
            var awaitedResult = await DoSomethingAsync().ConfigureAwait(false);

            Assert.That(awaitedResult, Is.EqualTo(4711));
        }

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_test_method_with_awaited_method_call_and_other_calls_before()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest()
        {
            const int Expected = 4711;

            Assert.That(await DoSomethingAsync(), Is.EqualTo(Expected));
        }

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public async Task SomeTest()
        {
            const int Expected = 4711;

            var awaitedResult = await DoSomethingAsync();

            Assert.That(awaitedResult, Is.EqualTo(Expected));
        }

        private Task<int> DoSomethingAsync() => Task.FromResult(42);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3129_AssertThatDoesNotUseAwaitForActualAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3129_AssertThatDoesNotUseAwaitForActualAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3129_CodeFixProvider();
    }
}
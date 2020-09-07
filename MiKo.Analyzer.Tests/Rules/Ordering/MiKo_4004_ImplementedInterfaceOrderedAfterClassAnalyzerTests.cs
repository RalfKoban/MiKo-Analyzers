using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4004_ImplementedInterfaceOrderedAfterClassAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_reported_for_class_without_interface() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_reported_for_class_with_single_interface() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ITestMe
    {
    }

    public class TestMe : ITestMe
    {
    }
}
");

        [Test]
        public void No_issue_reported_for_class_with_correctly_ordered_interfaces() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public interface ITestMe
    {
    }

    public class TestMe : ITestMe, IDisposable
    {
        public void Dispose()
        {
        }
    }
}
");

        [Test]
        public void An_issue_reported_for_class_with_incorrectly_ordered_interfaces() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public interface ITestMe
    {
    }

    public class TestMe : IDisposable, ITestMe
    {
        public void Dispose()
        {
        }
    }
}
");

        [Test]
        public void No_issue_reported_for_class_with_correctly_ordered_interfaces_and_base_class() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class Base
    {
    }

    public interface ITestMe
    {
    }

    public class TestMe : Base, ITestMe, IDisposable
    {
        public void Dispose()
        {
        }
    }
}
");

        [Test]
        public void An_issue_reported_for_class_with_incorrectly_ordered_interfaces_and_base_class() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class Base
    {
    }

    public interface ITestMe
    {
    }

    public class TestMe : Base, IDisposable, ITestMe
    {
        public void Dispose()
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_type_with_base_class()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class Base
    {
    }

    public interface ITestMe
    {
    }

    public class TestMe : Base, IDisposable, ITestMe
    {
        public void Dispose()
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class Base
    {
    }

    public interface ITestMe
    {
    }

    public class TestMe : Base, ITestMe, IDisposable
    {
        public void Dispose()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_type_without_base_class()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public interface ITestMe
    {
    }

    public class TestMe : IDisposable, ITestMe
    {
        public void Dispose()
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public interface ITestMe
    {
    }

    public class TestMe : ITestMe, IDisposable
    {
        public void Dispose()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4004_ImplementedInterfaceOrderedAfterClassAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4004_ImplementedInterfaceOrderedAfterClassAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4004_CodeFixProvider();
    }
}
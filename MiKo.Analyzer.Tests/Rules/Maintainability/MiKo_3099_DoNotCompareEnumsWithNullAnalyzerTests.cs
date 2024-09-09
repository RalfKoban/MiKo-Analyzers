using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3099_DoNotCompareEnumsWithNullAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Comparisons = ["==", "!=", "is", "is not"];

        [Test]
        public void No_issue_is_reported_for_null_as_argument() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(object o)
        {
            return DoSomething(null);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_null_as_return_value() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(object o)
        {
            return null;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_null_comparison_to_object_([ValueSource(nameof(Comparisons))] string comparison) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(object o)
        {
            if (o " + comparison + @" null)
                return new object();
            
            return new object();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_null_comparison_to_enum_([ValueSource(nameof(Comparisons))] string comparison) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(StringComparison comparison)
        {
            if (comparison " + comparison + @" null)
                return new object();
            
            return new object();
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_equality_for_null_comparison_to_enum()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(StringComparison comparison)
        {
            if (comparison == null)
                return new object();
            
            return new object();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(StringComparison comparison)
        {
            if (false)
                return new object();
            
            return new object();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode, allowNewCompilerDiagnostics: true); // CS0162 unreachable code detected
        }

        [Test]
        public void Code_gets_fixed_for_inequality_for_null_comparison_to_enum()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(StringComparison comparison)
        {
            if (comparison != null)
                return new object();
            
            return new object();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(StringComparison comparison)
        {
            if (true)
                return new object();
            
            return new object();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode, allowNewCompilerDiagnostics: true); // CS0162 unreachable code detected
        }

        [Test]
        public void Code_gets_fixed_for_is_null_comparison_to_enum()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(StringComparison comparison)
        {
            if (comparison is null)
                return new object();
            
            return new object();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(StringComparison comparison)
        {
            if (false)
                return new object();
            
            return new object();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode, allowNewCompilerDiagnostics: true); // CS0162 unreachable code detected
        }

        [Test]
        public void Code_gets_fixed_for_is_not_null_comparison_to_enum()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(StringComparison comparison)
        {
            if (comparison is not null)
                return new object();
            
            return new object();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething(StringComparison comparison)
        {
            if (true)
                return new object();
            
            return new object();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode, allowNewCompilerDiagnostics: true); // CS0162 unreachable code detected
        }

        protected override string GetDiagnosticId() => MiKo_3099_DoNotCompareEnumsWithNullAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3099_DoNotCompareEnumsWithNullAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3099_CodeFixProvider();
    }
}
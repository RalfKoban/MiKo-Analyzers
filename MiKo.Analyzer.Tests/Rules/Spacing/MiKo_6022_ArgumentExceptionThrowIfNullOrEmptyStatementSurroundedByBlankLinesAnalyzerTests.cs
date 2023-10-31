using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6022_ArgumentExceptionThrowIfNullOrEmptyStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentException_access_without_ThrowIfNullOrEmpty() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomething(ex => ex.Message);
        }

        public void DoSomething(Func<ArgumentException, string> callback)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentException_access_without_ThrowIfNullOrEmpty_2() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public string DoSomething()
        {
            return new ArgumentException().Message;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_single_ThrowIfNullOrEmpty_statement_as_first_statement_with_blank_line_after_it() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o)
        {
            ArgumentException.ThrowIfNullOrEmpty(o);

            DoSomething(new string());
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_ThrowIfNullOrEmpty_statement_as_first_statements_with_blank_line_after_all_of_them() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o1, string o2, string o3)
        {
            ArgumentException.ThrowIfNullOrEmpty(o1);
            ArgumentException.ThrowIfNullOrEmpty(o2);
            ArgumentException.ThrowIfNullOrEmpty(o3);

            DoSomething(new string(), new string(), new string());
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_combined_ThrowIf_statements_as_first_statements_with_blank_line_after_all_of_them() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o1, object o2, object o3)
        {
            ArgumentNullException.ThrowIfNull(o1);
            ArgumentException.ThrowIfNullOrEmpty(o2);
            ObjectDisposedException.ThrowIf(true, o3);

            DoSomething(new object(), new object(), new object());
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNullOrEmpty_statement_as_first_statement_with_no_blank_line_after_it() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o)
        {
            ArgumentException.ThrowIfNullOrEmpty(o);
            DoSomething(new string());
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_multiple_ThrowIfNullOrEmpty_statement_as_first_statements_with_no_blank_line_after_all_of_them() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o1, string o2, string o3)
        {
            ArgumentException.ThrowIfNullOrEmpty(o1);
            ArgumentException.ThrowIfNullOrEmpty(o2);
            ArgumentException.ThrowIfNullOrEmpty(o3);
            DoSomething(new string(), new string(), new string());
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_single_ThrowIfNullOrEmpty_statement_as_first_statement_with_blank_line_after_it()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o)
        {
            ArgumentException.ThrowIfNullOrEmpty(o);
            DoSomething(new string());
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
        public void DoSomething(string o)
        {
            ArgumentException.ThrowIfNullOrEmpty(o);

            DoSomething(new string());
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_ThrowIfNullOrEmpty_statement_as_first_statements_with_blank_line_after_all_of_them()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o1, string o2, string o3)
        {
            ArgumentException.ThrowIfNullOrEmpty(o1);
            ArgumentException.ThrowIfNullOrEmpty(o2);
            ArgumentException.ThrowIfNullOrEmpty(o3);
            DoSomething(new string(), new string(), new string());
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
        public void DoSomething(string o1, string o2, string o3)
        {
            ArgumentException.ThrowIfNullOrEmpty(o1);
            ArgumentException.ThrowIfNullOrEmpty(o2);
            ArgumentException.ThrowIfNullOrEmpty(o3);

            DoSomething(new string(), new string(), new string());
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6022_ArgumentExceptionThrowIfNullOrEmptyStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6022_ArgumentExceptionThrowIfNullOrEmptyStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6022_CodeFixProvider();
    }
}
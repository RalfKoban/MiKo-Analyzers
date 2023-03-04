using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public class MiKo_6021_ArgumentNullExceptionThrowIfNullStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_ArgumentNullException_access_without_ThrowIfNull() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomething(ex => ex.Message);
        }

        public void DoSomething(Func<ArgumentNullException, string> callback)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_access_without_ThrowIfNull_2() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public string DoSomething()
        {
            return new ArgumentNullException().Message;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_single_ThrowIfNull_statement_as_first_statement_with_blank_line_after_it() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o)
        {
            ArgumentNullException.ThrowIfNull(o);

            DoSomething(new object());
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_ThrowIfNull_statement_as_first_statements_with_blank_line_after_all_of_them() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o1, object o2, object o3)
        {
            ArgumentNullException.ThrowIfNull(o1);
            ArgumentNullException.ThrowIfNull(o2);
            ArgumentNullException.ThrowIfNull(o3);

            DoSomething(new object(), new object(), new object());
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_statement_as_first_statement_with_no_blank_line_after_it() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o)
        {
            ArgumentNullException.ThrowIfNull(o);
            DoSomething(new object());
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_multiple_ThrowIfNull_statement_as_first_statements_with_no_blank_line_after_all_of_them() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o1, object o2, object o3)
        {
            ArgumentNullException.ThrowIfNull(o1);
            ArgumentNullException.ThrowIfNull(o2);
            ArgumentNullException.ThrowIfNull(o3);
            DoSomething(new object(), new object(), new object());
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_single_ThrowIfNull_statement_as_first_statement_with_blank_line_after_it()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o)
        {
            ArgumentNullException.ThrowIfNull(o);
            DoSomething(new object());
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
        public void DoSomething(object o)
        {
            ArgumentNullException.ThrowIfNull(o);

            DoSomething(new object());
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_ThrowIfNull_statement_as_first_statements_with_blank_line_after_all_of_them()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o1, object o2, object o3)
        {
            ArgumentNullException.ThrowIfNull(o1);
            ArgumentNullException.ThrowIfNull(o2);
            ArgumentNullException.ThrowIfNull(o3);
            DoSomething(new object(), new object(), new object());
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
        public void DoSomething(object o1, object o2, object o3)
        {
            ArgumentNullException.ThrowIfNull(o1);
            ArgumentNullException.ThrowIfNull(o2);
            ArgumentNullException.ThrowIfNull(o3);

            DoSomething(new object(), new object(), new object());
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6021_ArgumentNullExceptionThrowIfNullStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6021_ArgumentNullExceptionThrowIfNullStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6021_CodeFixProvider();
    }
}
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6023_ArgumentOutOfRangeExceptionThrowIfStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MethodNames =
                                                       {
                                                           "ThrowIfZero",
                                                           "ThrowIfNegative",
                                                           "ThrowIfNegativeOrZero",
                                                           "ThrowIfGreaterThan",
                                                       };

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
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_access_without_ThrowIf() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomething(ex => ex.Message);
        }

        public void DoSomething(Func<ArgumentOutOfRangeException, string> callback)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_access_without_ThrowIf_2() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public string DoSomething()
        {
            return new ArgumentOutOfRangeException().Message;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_single_ThrowIf_statement_as_first_statement_with_blank_line_after_it_([ValueSource(nameof(MethodNames))] string methodName) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o)
        {
            ArgumentOutOfRangeException." + methodName + @"(o);

            DoSomething(new string());
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_ThrowIf_statement_as_first_statements_with_blank_line_after_all_of_them_([ValueSource(nameof(MethodNames))] string methodName) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o1, string o2, string o3)
        {
            ArgumentOutOfRangeException." + methodName + @"(o1);
            ArgumentOutOfRangeException." + methodName + @"(o2);
            ArgumentOutOfRangeException." + methodName + @"(o3);

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
            ArgumentOutOfRangeException.ThrowIfZero();
            ArgumentOutOfRangeException.ThrowIfNegative();
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero();
            ArgumentOutOfRangeException.ThrowIfGreaterThan();
            ArgumentException.ThrowIfNullOrEmpty(o2);
            ObjectDisposedException.ThrowIf(true, o3);

            DoSomething(new object(), new object(), new object());
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIf_statement_as_first_statement_with_no_blank_line_after_it_([ValueSource(nameof(MethodNames))] string methodName) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o)
        {
            ArgumentOutOfRangeException." + methodName + @"(o);
            DoSomething(new string());
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_multiple_ThrowIf_statement_as_first_statements_with_no_blank_line_after_all_of_them_([ValueSource(nameof(MethodNames))] string methodName) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o1, string o2, string o3)
        {
            ArgumentOutOfRangeException." + methodName + @"(o1);
            ArgumentOutOfRangeException." + methodName + @"(o2);
            ArgumentOutOfRangeException." + methodName + @"(o3);
            DoSomething(new string(), new string(), new string());
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_single_ThrowIf_statement_as_first_statement_with_blank_line_after_it_([ValueSource(nameof(MethodNames))] string methodName)
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o)
        {
            ArgumentOutOfRangeException.###(o);
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
            ArgumentOutOfRangeException.###(o);

            DoSomething(new string());
        }
    }
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", methodName), FixedCode.Replace("###", methodName));
        }

        [Test]
        public void Code_gets_fixed_for_multiple_ThrowIf_statement_as_first_statements_with_blank_line_after_all_of_them_([ValueSource(nameof(MethodNames))] string methodName)
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string o1, string o2, string o3)
        {
            ArgumentOutOfRangeException.###(o1);
            ArgumentOutOfRangeException.###(o2);
            ArgumentOutOfRangeException.###(o3);
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
            ArgumentOutOfRangeException.###(o1);
            ArgumentOutOfRangeException.###(o2);
            ArgumentOutOfRangeException.###(o3);

            DoSomething(new string(), new string(), new string());
        }
    }
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", methodName), FixedCode.Replace("###", methodName));
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6023_ArgumentOutOfRangeExceptionThrowIfStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6023_ArgumentOutOfRangeExceptionThrowIfStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6023_CodeFixProvider();
    }
}
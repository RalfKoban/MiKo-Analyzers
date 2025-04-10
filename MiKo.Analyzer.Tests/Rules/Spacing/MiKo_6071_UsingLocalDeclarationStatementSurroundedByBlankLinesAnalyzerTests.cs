using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6071_UsingLocalDeclarationStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_multiple_non_using_statements_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var something = new TestMe();
            DoSomething();
            var something = new TestMe();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_statement_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            using var something = new TestMe();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_using_statements_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            using var something1 = new TestMe();
            using var something2 = new TestMe();
            using var something3 = new TestMe();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_statement_with_blank_line_before() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            DoSomething();

            using var something = new TestMe();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_statement_with_blank_line_after() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            using var something = new TestMe();

            DoSomething();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_using_statement_without_blank_line_before() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            DoSomething();
            using var something = new TestMe();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_using_statement_without_blank_line_after() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            using var something = new TestMe();
            DoSomething();
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_separate_using_statements_without_blank_line_before_or_after() => An_issue_is_reported_for(2, @"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            using var something = new TestMe();
            DoSomething();
            using var something = new TestMe();
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_using_statement_without_blank_line_before()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            DoSomething();
            using var something = new TestMe();
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            DoSomething();

            using var something = new TestMe();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_using_statement_without_blank_line_after()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            using var something = new TestMe();
            DoSomething();
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            using var something = new TestMe();

            DoSomething();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_using_statement_without_blank_line_before_and_after()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            DoSomething();
            using var something = new TestMe();
            DoSomething();
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe : IDisposable
    {
        public void DoSomething()
        {
            DoSomething();

            using var something = new TestMe();

            DoSomething();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6071_UsingLocalDeclarationStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6071_UsingLocalDeclarationStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6071_CodeFixProvider();
    }
}
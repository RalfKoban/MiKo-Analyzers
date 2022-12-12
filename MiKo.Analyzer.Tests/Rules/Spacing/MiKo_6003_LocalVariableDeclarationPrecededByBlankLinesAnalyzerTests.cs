using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6003_LocalVariableDeclarationPrecededByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_call() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_as_first_statement() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            var x = 1;

            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_as_first_statement_inside_if_block() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                var x = 1;

                DoSomething();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_as_first_statement_inside_if_non_block() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            DoSomething();
            if (something)
                var x = 1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_as_first_statement_inside_else_non_block() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            DoSomething();
            if (something)
            {
            }
            else
                var x = 1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_as_first_statement_inside_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    var x = 1;

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_preceded_by_if_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            var x = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_preceded_by_code_inside_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomething(42);
                    var x = 1;

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_preceded_by_method_call() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            DoSomething();
            var x = 1;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line_after_if_block()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            var x = 1;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            var x = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line_after_method_call()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            DoSomething(something);
            var x = 1;
            DoSomething(something);
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            DoSomething(something);

            var x = 1;
            DoSomething(something);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_variables_with_missing_preceding_line()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            DoSomething(something);
            var x = something;
            var y = something;
            DoSomething(something);
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            DoSomething(something);

            var x = something;
            var y = something;
            DoSomething(something);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6003_LocalVariableDeclarationPrecededByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6003_LocalVariableDeclarationPrecededByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6003_CodeFixProvider();
    }
}
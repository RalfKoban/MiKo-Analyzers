using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3301_ParenthesizedLambdaExpressionUsesExpressionBodyAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_that_returns_a_value() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() => 42);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_that_returns_no_value() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore(() => i++);
        }

        private void DoSomethingCore(Action callback)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_with_empty_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingCore(() => { });
        }

        private void DoSomethingCore(Action callback)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_with_empty_statement_inside_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingCore(() => { ; });
        }

        private void DoSomethingCore(Action callback)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_with_single_variable_declaration_statement_inside_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingCore(() => { var x = 123; });
        }

        private void DoSomethingCore(Action callback)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_with_multiple_statements_inside_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingCore(() =>
                                {
                                    var x = 0;
                                    var y = 1;
                                });
        }

        private void DoSomethingCore(Action callback)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_lambda_expression_that_consists_of_a_single_line_and_returns_a_value() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() => { return 42 });
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_lambda_expression_that_consists_of_a_single_line_and_returns_no_value() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore(() => { i++; });
        }

        private void DoSomethingCore(Action callback)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_lambda_expression_with_single_parameter_that_consists_of_a_single_line_and_returns_no_value() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingCore((i) => { i++; });
        }

        private void DoSomethingCore(Func<int, int> callback)
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_parenthesized_lambda_expression_that_consists_of_a_single_line_and_returns_a_value()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() => { return 42; });
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
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
        public int DoSomething()
        {
            return DoSomethingCore(() => 42);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_lambda_expression_that_consists_of_a_single_line_and_returns_no_value()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore(() => { i++; });
        }

        private void DoSomethingCore(Action callback)
        {
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
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore(() => i++);
        }

        private void DoSomethingCore(Action callback)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_lambda_expression_with_single_parameter_that_consists_of_a_single_line_and_returns_no_value()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingCore((i) => { return i++; });
        }

        private void DoSomethingCore(Func<int, int> callback)
        {
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
        public void DoSomething()
        {
            DoSomethingCore(i => i++);
        }

        private void DoSomethingCore(Func<int, int> callback)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_lambda_expression_with_multiple_parameters_that_consists_of_a_single_line_and_returns_no_value()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingCore((x, y) => { return x + y; });
        }

        private void DoSomethingCore(Func<int, int, int> callback)
        {
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
        public void DoSomething()
        {
            DoSomethingCore((x, y) => x + y);
        }

        private void DoSomethingCore(Func<int, int, int> callback)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_lambda_expression_with_throw_statement()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingCore(() => { throw new NotImplementedException(""Something went wrong""); });
        }

        private void DoSomethingCore(Action callback)
        {
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
        public void DoSomething()
        {
            DoSomethingCore(() => throw new NotImplementedException(""Something went wrong""));
        }

        private void DoSomethingCore(Action callback)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_async_parenthesized_lambda_expression()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public async void DoSomething()
        {
            var result = async () =>
                            {
                                await Task.CompletedTask;
                            };
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
        public async void DoSomething()
        {
            var result = async () => await Task.CompletedTask;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_async_parenthesized_lambda_expression_with_single_parameter()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public async void DoSomething()
        {
            DoSomethingCore(async (i) =>
                            {
                                return await Task.FromResult(i);
                            });
        }

        private void DoSomethingCore(Func<int, Task<int>> callback)
        {
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
        public async void DoSomething()
        {
            DoSomethingCore(async i => await Task.FromResult(i));
        }

        private void DoSomethingCore(Func<int, Task<int>> callback)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3301_ParenthesizedLambdaExpressionUsesExpressionBodyAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3301_ParenthesizedLambdaExpressionUsesExpressionBodyAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3301_CodeFixProvider();
    }
}
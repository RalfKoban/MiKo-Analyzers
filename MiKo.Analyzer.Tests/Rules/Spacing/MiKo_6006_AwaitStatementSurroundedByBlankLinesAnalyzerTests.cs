using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6006_AwaitStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_awaited_call_as_only_statement() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            await Task.CompletedTask;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_awaited_call_as_only_statement_in_expression_body() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething() => await Task.CompletedTask;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_awaited_call_as_only_statement_in_switch_section() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    await Task.CompletedTask;

                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_awaited_calls_as_only_statements() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            await Task.CompletedTask;
            await Task.CompletedTask;
            await Task.CompletedTask;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_awaited_call_as_only_statement_in_ParenthesizedLambdaExpression() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            DoSomethingElse();
            Callback(async () => await Task.CompletedTask);
            DoSomethingElse();
        }

        private void Callback(Func<Task> callback)
        { }

        private void DoSomethingElse()
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_awaited_call_preceded_by_blank_line() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            DoSomethingElse();

            await Task.CompletedTask;
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_awaited_call_preceded_by_blank_line_if_its_result_gets_assigned_to_local_variable() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            DoSomethingElse();

            var result = await Task.FromResult(true);
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_awaited_calls_preceded_by_blank_line_if_their_results_get_assigned_to_local_variables() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            DoSomethingElse();

            var result1 = await Task.FromResult(true);
            var result2 = await Task.FromResult(false);
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_awaited_calls_preceded_by_blank_line_if_their_results_get_assigned_to_same_local_variable() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            DoSomethingElse();

            var result = await Task.FromResult(true);
            result |= await Task.FromResult(false);
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_awaited_call_followed_by_blank_line() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            await Task.CompletedTask;

            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_awaited_call_followed_by_blank_line_if_its_result_gets_assigned_to_local_variable() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            var result = await Task.FromResult(true);

            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_awaited_call_preceded_by_awaited_using_assigned_to_local_variable() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            await using var item = new Item();
            await Task.FromResult(item);
        }
    }

    public partial class Item : IAsyncDisposable
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_awaited_call_not_preceded_by_blank_line() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            DoSomethingElse();
            await Task.CompletedTask;
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_awaited_call_not_followed_by_blank_line() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            await Task.CompletedTask;
            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_awaited_call_not_preceded_by_blank_line_in_switch_section() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomethingElse();
                    await Task.CompletedTask;

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_awaited_call_not_followed_by_blank_line_in_switch_section() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    await Task.CompletedTask;
                    DoSomethingElse();

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_awaited_call_not_preceded_by_blank_line_if_its_result_gets_assigned_to_local_variable() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            DoSomethingElse();
            var result = await Task.FromResult(true);
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_awaited_call_not_followed_by_blank_line_if_its_result_gets_assigned_to_local_variable() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            var result = await Task.FromResult(true);
            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_awaited_call_not_followed_by_blank_line_if_its_result_gets_assigned_to_local_variable_when_coming_before_another_awaited_call_without_assignment() => An_issue_is_reported_for(2, @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            var result = await Task.FromResult(true);
            await Task.FromResult(false);
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_awaited_call_not_preceded_by_blank_line_if_its_result_gets_assigned_to_local_variable_when_coming_after_another_awaited_call_without_assignment() => An_issue_is_reported_for(2, @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            await Task.FromResult(false);
            var result = await Task.FromResult(true);
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_awaited_call_not_preceded_by_blank_line()
        {
            const string OriginalCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            DoSomethingElse();
            await Task.CompletedTask;
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            const string FixedCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            DoSomethingElse();

            await Task.CompletedTask;
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_awaited_call_not_followed_by_blank_line()
        {
            const string OriginalCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            await Task.CompletedTask;
            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            const string FixedCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            await Task.CompletedTask;

            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_awaited_call_not_followed_by_blank_line_in_switch_section()
        {
            const string OriginalCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    await Task.CompletedTask;
                    DoSomethingElse();

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            const string FixedCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    await Task.CompletedTask;

                    DoSomethingElse();

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_awaited_call_not_preceded_by_blank_line_in_switch_section()
        {
            const string OriginalCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomethingElse();
                    await Task.CompletedTask;

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            const string FixedCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomethingElse();

                    await Task.CompletedTask;

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_awaited_call_not_preceded_by_blank_line_if_its_result_gets_assigned_to_local_variable()
        {
            const string OriginalCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            DoSomethingElse();
            var result = await Task.FromResult(true);
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            const string FixedCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        public async Task DoSomething()
        {
            DoSomethingElse();

            var result = await Task.FromResult(true);
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_awaited_call_not_preceded_by_blank_line_if_its_result_gets_assigned_to_field()
        {
            const string OriginalCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        private bool result;

        public async Task DoSomething()
        {
            DoSomethingElse();
            result = await Task.FromResult(true);
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            const string FixedCode = @"
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        private bool result;

        public async Task DoSomething()
        {
            DoSomethingElse();

            result = await Task.FromResult(true);
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6006_AwaitStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6006_AwaitStatementSurroundedByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6006_CodeFixProvider();
    }
}
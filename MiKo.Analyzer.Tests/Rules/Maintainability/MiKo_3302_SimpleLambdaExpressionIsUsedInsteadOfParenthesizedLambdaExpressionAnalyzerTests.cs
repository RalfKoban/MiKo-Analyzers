using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public class MiKo_3302_SimpleLambdaExpressionIsUsedInsteadOfParenthesizedLambdaExpressionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_simple_lambda_expression_body() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i => 42);
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_that_has_no_parameters() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_that_has_2_parameters() => No_issue_is_reported_for(@"
namespace Bla
{
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore((i, j) => i + j);
        }

        private void DoSomethingCore(Func<int,int, int> callback)
        {
        }
    }
}}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_block_that_has_1_parameter() => No_issue_is_reported_for(@"
namespace Bla
{
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore((i) => { return i++; });
        }

        private void DoSomethingCore(Func<int,int> callback)
        {
        }
    }
}}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_that_has_1_parameter_but_an_type_information() => No_issue_is_reported_for(@"
namespace Bla
{
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore((int i) => i++);
        }

        private void DoSomethingCore(Func<int,int> callback)
        {
        }
    }
}}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_lambda_expression_body_that_has_1_parameter() => An_issue_is_reported_for(@"
namespace Bla
{
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore((i) => i++);
        }

        private void DoSomethingCore(Func<int,int> callback)
        {
        }
    }
}}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
namespace Bla
{
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore((i) => i++);
        }

        private void DoSomethingCore(Func<int, int> callback)
        {
        }
    }
}}
";

            const string FixedCode = @"
namespace Bla
{
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;

            DoSomethingCore(i => i++);
        }

        private void DoSomethingCore(Func<int, int> callback)
        {
        }
    }
}}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3302_SimpleLambdaExpressionIsUsedInsteadOfParenthesizedLambdaExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3302_SimpleLambdaExpressionIsUsedInsteadOfParenthesizedLambdaExpressionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3302_CodeFixProvider();
    }
}
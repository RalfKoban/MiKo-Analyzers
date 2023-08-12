using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public class MiKo_6046_CalculationOperationsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CalculationOperators = { "+", "-", "%", "*", "/", "<<", ">>" };

        [Test]
        public void No_issue_is_reported_if_complete_operation_is_on_same_line_([ValueSource(nameof(CalculationOperators))] string calculationOperator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int a, int b)
    {
        var result = a " + calculationOperator + @" b;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_constants_spans_multiple_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
                   + ""with some other text""
                   + ""and even more text"";
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_variables_spans_multiple_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
                   + Environment.NewLine
                   + Environment.NewLine
                   + text2;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_operator_is_on_different_line_than_left_operand_([ValueSource(nameof(CalculationOperators))] string calculationOperator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int a, int b)
    {
        var result = a
            " + calculationOperator + @" b;
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_if_operator_is_on_different_line_than_right_operand_([ValueSource(nameof(CalculationOperators))] string calculationOperator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int a, int b)
    {
        var result = a " + calculationOperator + @"
            b;
    }
}
");

        [Test]
        public void Code_gets_fixed_if_operator_is_on_different_line_than_left_operand_([ValueSource(nameof(CalculationOperators))] string calculationOperator)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int a, int b)
    {
        var result = a 
            " + calculationOperator + @" b;
    }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int a, int b)
    {
        var result = a " + calculationOperator + @" b;
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_operator_is_on_different_line_than_right_operand_([ValueSource(nameof(CalculationOperators))] string calculationOperator)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int a, int b)
    {
        var result = a " + calculationOperator + @"
            b;
    }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int a, int b)
    {
        var result = a " + calculationOperator + @" b;
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6046_CalculationOperationsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6046_CalculationOperationsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6046_CodeFixProvider();
    }
}
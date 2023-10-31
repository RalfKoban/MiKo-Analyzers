using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public class MiKo_6045_ComparisonsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ComparisonOperators = { "==", "!=", "<", "<=", ">", ">=" };

        [Test]
        public void No_issue_is_reported_if_complete_operation_is_on_same_line_([ValueSource(nameof(ComparisonOperators))] string comparisonOperator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a " + comparisonOperator + @" b)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_if_operator_is_on_different_line_than_left_operand_([ValueSource(nameof(ComparisonOperators))] string comparisonOperator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a
            " + comparisonOperator + @" b)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_if_operator_is_on_different_line_than_right_operand_([ValueSource(nameof(ComparisonOperators))] string comparisonOperator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a " + comparisonOperator + @"
            b)
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_if_operator_is_on_different_line_than_left_operand_([ValueSource(nameof(ComparisonOperators))] string comparisonOperator)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a 
            " + comparisonOperator + @" b)
        {
        }
    }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a " + comparisonOperator + @" b)
        {
        }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_operator_is_on_different_line_than_right_operand_([ValueSource(nameof(ComparisonOperators))] string comparisonOperator)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a " + comparisonOperator + @"
            b)
        {
        }
    }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a " + comparisonOperator + @" b)
        {
        }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6045_ComparisonsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6045_ComparisonsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6045_CodeFixProvider();
    }
}
﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public class MiKo_6044_OperatorsOfBinaryExpressionsAreOnSameLineAsRightOperandAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_complete_operation_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a && b)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_if_operator_is_on_same_line_as_right_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a
            && b)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_if_operator_is_on_different_line_than_right_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a &&
            b)
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_if_operator_is_on_different_line_than_right_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a &&
            b)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b)
    {
        if (a
            && b)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_operators_with_comments_are_on_different_lines_than_right_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b, bool c)
    {
        if (a && // comment 1
            b || // comment 2
            c) // comment 3
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool a, bool b, bool c)
    {
        if (a // comment 1
            && b // comment 2
            || c) // comment 3
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_complex_operators_with_comments_are_on_different_lines_than_right_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int a, int b, int c, int d, int e)
    {
        if (a <= 1 || // comment 1
            (b == 0 || c <= 0) // comment 2
            &&
            (d > 1 || e < 0) // comment 3
           )
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int a, int b, int c, int d, int e)
    {
        if (a <= 1 // comment 1
            || (b == 0 || c <= 0) // comment 2
            && (d > 1 || e < 0) // comment 3
           )
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6044_OperatorsOfBinaryExpressionsAreOnSameLineAsRightOperandAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6044_OperatorsOfBinaryExpressionsAreOnSameLineAsRightOperandAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6044_CodeFixProvider();
    }
}
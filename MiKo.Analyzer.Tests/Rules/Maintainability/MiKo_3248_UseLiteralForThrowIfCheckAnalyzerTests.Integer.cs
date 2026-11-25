using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_argument_and_no_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument) => " + invocation + @"(argument);
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_argument_and_numeric_literal_as_other_and_paramName_(
            [ValueSource(nameof(ValueChecksWithOther))] string invocation,
            [Values("-1", "+1", "-1L", "-1.5", "-1.5d", "-1.5f", "-1.5m", "-0x1F")] string literal)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, string paramName) => " + invocation + @"(argument, " + literal + @", paramName);
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_argument_and_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, string paramName) => " + invocation + @"(argument, paramName);
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_argument_and_parenthesized_negative_numeric_literal_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, string paramName) => " + invocation + @"(argument, (-1), paramName);
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_argument_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int other) => " + invocation + @"(argument, other);
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_argument_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int other, string paramName) => " + invocation + @"(argument, other, paramName);
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_numeric_literal_as_argument_and_other_and_paramName_(
            [ValueSource(nameof(ValueChecksWithOther))] string invocation,
            [Values("-1", "+1", "-1.5")] string literal)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int other, string paramName) => " + invocation + @"(" + literal + @", other, paramName);
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_numeric_literal_as_argument_and_paramName_(
            [ValueSource(nameof(ValueChecksWithoutOther))] string invocation,
            [Values("-1", "+1", "-1.5")] string literal)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string paramName) => " + invocation + @"(" + literal + @", paramName);
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_ThrowIfLessThan_invocation_with_argument_and_negative_numeric_literal_and_no_paramName() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value) => ArgumentOutOfRangeException.ThrowIfLessThan(value, -1);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_bitwise_complement_of_numeric_literal_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, string paramName) => " + invocation + @"(argument, ~1, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_conditional_expression_as_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, bool useFirst, int first, int second) => " + invocation + @"(argument, useFirst ? first : second);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_conditional_expression_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, bool useFirst, int first, int second, string paramName) => " + invocation + @"(argument, useFirst ? first : second, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_negated_parenthesized_expression_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, string paramName) => " + invocation + @"(argument, -(1 + 2), paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_negated_variable_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int other, string paramName) => " + invocation + @"(argument, -other, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_null_coalescing_assignment_as_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int? other) => " + invocation + @"(argument, other ??= 0);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_null_coalescing_assignment_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int? other, string paramName) => " + invocation + @"(argument, other ??= 0, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_null_coalescing_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int? other) => " + invocation + @"(argument, other ?? 0);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_null_coalescing_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int? other, string paramName) => " + invocation + @"(argument, other ?? 0, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_null_conditional_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Number { get; set; }

    public void DoSomething(int argument, TestMe item) => " + invocation + @"(argument, (int)item?.Number);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_null_conditional_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Number { get; set; }

    public void DoSomething(int argument, TestMe item, string paramName) => " + invocation + @"(argument, (int)item?.Number, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_switch_expression_as_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int? value, int fallback) => " + invocation + @"(argument, value switch { null => fallback, _ => value.Value });
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_switch_expression_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int? value, int fallback, string paramName) => " + invocation + @"(argument, value switch { null => fallback, _ => value.Value }, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_conditional_expression_argument_and_no_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool useFirst, int first, int second) => " + invocation + @"(useFirst ? first : second);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_conditional_expression_argument_and_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool useFirst, int first, int second, string paramName) => " + invocation + @"(useFirst ? first : second, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_conditional_expression_as_argument_and_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool useFirst, int first, int second, int other) => " + invocation + @"(useFirst ? first : second, other);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_conditional_expression_as_argument_and_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool useFirst, int first, int second, int other, string paramName) => " + invocation + @"(useFirst ? first : second, other, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_negated_variable_as_argument_and_no_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument) => " + invocation + @"(-argument);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_coalescing_argument_and_no_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? argument) => " + invocation + @"(argument ?? 0);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_coalescing_argument_and_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int? SomeValue { get; set; }

    public void DoSomething() => " + invocation + @"(SomeValue ?? 0, 42);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_coalescing_argument_and_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? argument, string paramName) => " + invocation + @"(argument ?? 0, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_coalescing_argument_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int? SomeValue { get; set; }

    public void DoSomething(string paramName) => " + invocation + @"(SomeValue ?? 0, 42, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_coalescing_assignment_argument_and_no_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? argument) => " + invocation + @"(argument ??= 0);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_coalescing_assignment_argument_and_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? argument, string paramName) => " + invocation + @"(argument ??= 0, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_coalescing_assignment_as_argument_and_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? argument, int other) => " + invocation + @"(argument ??= 0, other);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_coalescing_assignment_as_argument_and_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? argument, int other, string paramName) => " + invocation + @"(argument ??= 0, other, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_conditional_argument_and_no_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Number { get; set; }

    public void DoSomething(TestMe item) => " + invocation + @"((int)item?.Number);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_conditional_argument_and_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Number { get; set; }

    public void DoSomething(TestMe item, int other) => " + invocation + @"((int)item?.Number, other);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_conditional_argument_and_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(TestMe item, string paramName) => " + invocation + @"((int)item?.Number, paramName);

    public int Number { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_null_conditional_argument_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Number { get; set; }

    public void DoSomething(TestMe item, int other, string paramName) => " + invocation + @"((int)item?.Number, other, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_switch_expression_argument_and_no_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? value, int fallback) => " + invocation + @"(value switch { null => fallback, _ => value.Value });
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_switch_expression_argument_and_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? value, int fallback, string paramName) => " + invocation + @"(value switch { null => fallback, _ => value.Value }, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_switch_expression_as_argument_and_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? value, int fallback, int other) => " + invocation + @"(value switch { null => fallback, _ => value.Value }, other);
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_switch_expression_as_argument_and_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? value, int fallback, int other, string paramName) => " + invocation + @"(value switch { null => fallback, _ => value.Value }, other, paramName);
}
");
    }
}
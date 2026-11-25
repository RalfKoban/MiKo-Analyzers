using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_value_argument_with_relational_or_pattern_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value) => " + invocation + @"(value is < 0 or > 100, true);
}
");

        [Test]
        public void An_issue_is_reported_for_value_argument_with_relational_pattern_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value) => " + invocation + @"(value is > 100, true);
}
");

        [Test]
        public void An_issue_is_reported_for_value_argument_with_constant_pattern_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value) => " + invocation + @"(value is 42, true);
}
");

        [Test]
        public void An_issue_is_reported_for_value_argument_with_negated_pattern_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value) => " + invocation + @"(value is not 0, true);
}
");

        [Test]
        public void An_issue_is_reported_for_value_argument_with_parenthesized_pattern_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value) => " + invocation + @"((value is < 0 or > 100), true);
}
");

        [Test]
        public void An_issue_is_reported_for_other_argument_with_relational_or_pattern_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int value, bool flag) => " + invocation + @"(flag, value is < 0 or > 100);
}
");
    }
}

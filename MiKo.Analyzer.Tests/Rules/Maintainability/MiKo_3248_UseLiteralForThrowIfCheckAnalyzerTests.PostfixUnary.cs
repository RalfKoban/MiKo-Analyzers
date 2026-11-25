using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_value_argument_with_post_increment_or_decrement_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation, [Values("++", "--")] string op) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x) => " + invocation + @"(x" + op + @");
}
");

        [Test]
        public void An_issue_is_reported_for_value_argument_with_post_increment_or_decrement_and_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation, [Values("++", "--")] string op) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, string paramName) => " + invocation + @"(x" + op + @", paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_value_argument_with_parenthesized_post_increment_or_decrement_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation, [Values("++", "--")] string op) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x) => " + invocation + @"((x" + op + @"));
}
");

        [Test]
        public void An_issue_is_reported_for_named_value_argument_with_post_increment_or_decrement_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation, [Values("++", "--")] string op) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x) => " + invocation + @"(value: x" + op + @");
}
");

        [Test]
        public void An_issue_is_reported_for_value_argument_with_post_increment_or_decrement_and_other_argument_([ValueSource(nameof(ValueChecksWithOther))] string invocation, [Values("++", "--")] string op) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int limit) => " + invocation + @"(x" + op + @", limit);
}
");

        [Test]
        public void An_issue_is_reported_for_other_argument_with_post_increment_or_decrement_([ValueSource(nameof(ValueChecksWithOther))] string invocation, [Values("++", "--")] string op) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int number, int x) => " + invocation + @"(number, x" + op + @");
}
");
    }
}

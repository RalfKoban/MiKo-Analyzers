using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_named_argument_with_simple_value_([ValueSource(nameof(NullChecks))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first) => " + invocation + @"(argument: first);
}
");

        [Test]
        public void An_issue_is_reported_for_named_argument_with_complex_value_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"(argument: first ?? second);
}
");

        [Test]
        public void No_issue_is_reported_for_named_value_argument_with_simple_value_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int number) => " + invocation + @"(value: number);
}
");

        [Test]
        public void An_issue_is_reported_for_named_value_argument_with_complex_value_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int first, int second) => " + invocation + @"(value: first + second);
}
");

        [Test]
        public void No_issue_is_reported_for_named_value_and_other_arguments_with_simple_values_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int number, int limit) => " + invocation + @"(other: limit, value: number);
}
");

        [Test]
        public void An_issue_is_reported_for_named_value_argument_with_complex_value_and_other_argument_with_simple_value_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int first, int second, int limit) => " + invocation + @"(other: limit, value: first + second);
}
");

        [Test]
        public void An_issue_is_reported_for_named_other_argument_with_complex_value_and_value_argument_with_simple_value_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int number, int first, int second) => " + invocation + @"(other: first + second, value: number);
}
");
    }
}

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_interpolated_string_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"($""{first}{second}"");
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_interpolated_string_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second, string paramName) => " + invocation + @"($""{first}{second}"", paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_interpolated_string_with_single_variable_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first) => " + invocation + @"($""{first}"");
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_interpolated_string_with_text_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"($""prefix {first} and {second}"");
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_parenthesized_interpolated_string_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"(($""{first}{second}""));
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_parenthesized_interpolated_string_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second, string paramName) => " + invocation + @"(($""{first}{second}""), paramName);
}
");
    }
}
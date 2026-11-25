using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_tuple_with_simple_elements_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => ArgumentNullException.ThrowIfNull((first, second));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_tuple_with_simple_elements_as_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second, string paramName) => ArgumentNullException.ThrowIfNull((first, second), paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_tuple_with_named_elements_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => ArgumentNullException.ThrowIfNull((First: first, Second: second));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_parenthesized_tuple_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => ArgumentNullException.ThrowIfNull(((first, second)));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_nested_tuple_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second, string third) => ArgumentNullException.ThrowIfNull((first, (second, third)));
}
");
    }
}

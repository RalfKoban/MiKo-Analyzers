using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_array_creation_with_size_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int count) => ArgumentNullException.ThrowIfNull(new object[count]);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_array_creation_with_size_as_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int count, string paramName) => ArgumentNullException.ThrowIfNull(new object[count], paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_array_creation_with_initializer_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => ArgumentNullException.ThrowIfNull(new string[] { value });
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_implicit_array_creation_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => ArgumentNullException.ThrowIfNull(new[] { value });
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_implicit_array_creation_as_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string paramName) => ArgumentNullException.ThrowIfNull(new[] { value }, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_object_creation_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => ArgumentNullException.ThrowIfNull(new { value });
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_object_creation_as_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string paramName) => ArgumentNullException.ThrowIfNull(new { value }, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_parenthesized_array_creation_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int count) => ArgumentNullException.ThrowIfNull((new object[count]));
}
");
    }
}

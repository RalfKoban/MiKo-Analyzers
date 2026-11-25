using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_simple_lambda_inside_cast_as_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string paramName) => ArgumentNullException.ThrowIfNull((Func<string, string>)(x => x), paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_simple_lambda_inside_cast_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => ArgumentNullException.ThrowIfNull((Func<string, string>)(x => x));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_parenthesized_lambda_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string paramName) => ArgumentNullException.ThrowIfNull(() => value, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_parenthesized_lambda_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => ArgumentNullException.ThrowIfNull(() => value);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_block_bodied_lambda_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string paramName) => ArgumentNullException.ThrowIfNull(() => { return value; }, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_block_bodied_lambda_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => ArgumentNullException.ThrowIfNull(() => { return value; });
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_parenthesized_lambda_in_parentheses_as_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string paramName) => ArgumentNullException.ThrowIfNull((() => value), paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_parenthesized_lambda_in_parentheses_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => ArgumentNullException.ThrowIfNull((() => value));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_lambda_inside_cast_as_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string paramName) => ArgumentNullException.ThrowIfNull((Func<string>)(() => value), paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_lambda_inside_cast_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => ArgumentNullException.ThrowIfNull((Func<string>)(() => value));
}
");
    }
}
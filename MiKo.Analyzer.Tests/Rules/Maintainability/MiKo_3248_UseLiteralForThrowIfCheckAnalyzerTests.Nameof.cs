using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_String_invocation_with_argument_and_nameof_as_other_and_no_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => " + invocation + @"(value, nameof(value));
}
");

        [Test]
        public void No_issue_is_reported_for_String_invocation_with_argument_and_nameof_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string paramName) => " + invocation + @"(value, nameof(value), paramName);
}
");

        [Test]
        public void No_issue_is_reported_for_String_invocation_with_argument_and_nameof_of_member_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public void DoSomething(string value, string paramName) => " + invocation + @"(value, nameof(TestMe.Name), paramName);
}
");

        [Test]
        public void No_issue_is_reported_for_String_invocation_with_nameof_as_argument_and_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string other, string paramName) => " + invocation + @"(nameof(value), other, paramName);
}
");

        [Test]
        public void No_issue_is_reported_for_String_invocation_with_nameof_as_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => " + invocation + @"(nameof(value));
}
");

        [Test]
        public void No_issue_is_reported_for_String_invocation_with_argument_and_nameof_as_paramName_([ValueSource(nameof(NullChecks))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string argument) => " + invocation + @"(argument, nameof(argument));
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_argument_and_nameof_as_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument) => " + invocation + @"(argument, nameof(argument));
}
");

        [Test]
        public void No_issue_is_reported_for_Integer_invocation_with_argument_other_and_nameof_as_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int argument, int other) => " + invocation + @"(argument, other, nameof(argument));
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_complex_argument_and_nameof_as_other_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"(first ?? second, nameof(first));
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_argument_and_concatenation_with_nameof_as_other_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => " + invocation + @"(value, nameof(value) + ""x"");
}
");
    }
}
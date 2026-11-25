using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] NullChecks =
                                                      [
                                                          "ArgumentNullException.ThrowIfNull",
                                                          "ArgumentNullException.ThrowIfNullOrEmpty",
                                                          "ArgumentNullException.ThrowIfNullOrWhiteSpace",
                                                          "ArgumentException.ThrowIfNullOrEmpty",
                                                          "ArgumentException.ThrowIfNullOrWhiteSpace",
                                                      ];

        private static readonly string[] ValueChecksWithoutOther =
                                                                   [
                                                                       "ArgumentOutOfRangeException.ThrowIfNegative",
                                                                       "ArgumentOutOfRangeException.ThrowIfNegativeOrZero",
                                                                       "ArgumentOutOfRangeException.ThrowIfZero",
                                                                   ];

        private static readonly string[] ValueChecksWithOther =
                                                                [
                                                                    "ArgumentOutOfRangeException.ThrowIfEqual",
                                                                    "ArgumentOutOfRangeException.ThrowIfNotEqual",
                                                                    "ArgumentOutOfRangeException.ThrowIfGreaterThan",
                                                                    "ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual",
                                                                    "ArgumentOutOfRangeException.ThrowIfLessThan",
                                                                    "ArgumentOutOfRangeException.ThrowIfLessThanOrEqual",
                                                                ];

        [Test]
        public void No_issue_is_reported_for_normal_invocation_with_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(42);

    private void DoSomethingCore(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_String_invocation_with_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string argument) => " + invocation + @"(argument);
}
");

        [Test]
        public void No_issue_is_reported_for_String_invocation_with_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string argument, string paramName) => " + invocation + @"(argument, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_conditional_expression_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool useFirst, string first, string second) => " + invocation + @"(useFirst ? first : second);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_conditional_expression_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool useFirst, string first, string second, string paramName) => " + invocation + @"(useFirst ? first : second, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_invocation_as_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => " + invocation + @"(value.Trim());
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_null_coalescing_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"(first ?? second);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_null_coalescing_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second, string paramName) => " + invocation + @"(first ?? second, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_null_coalescing_assignment_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"(first ??= second);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_null_coalescing_assignment_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second, string paramName) => " + invocation + @"(first ??= second, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_null_conditional_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public void DoSomething(TestMe item) => " + invocation + @"(item?.Name);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_null_conditional_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(TestMe item, string paramName) => " + invocation + @"(item?.Name, paramName);

    public string Name { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_parenthesized_conditional_expression_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool useFirst, string first, string second, string paramName) => " + invocation + @"((useFirst ? first : second), paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_parenthesized_null_coalescing_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"((first ?? second));
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_parenthesized_null_coalescing_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second, string paramName) => " + invocation + @"((first ?? second), paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_parenthesized_null_coalescing_assignment_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"((first ??= second));
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_parenthesized_switch_expression_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string fallback, string paramName) => " + invocation + @"((value switch { null => fallback, _ => value }), paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_switch_expression_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string fallback) => " + invocation + @"(value switch { null => fallback, _ => value });
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_switch_expression_argument_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string fallback, string paramName) => " + invocation + @"(value switch { null => fallback, _ => value }, paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_string_concatenation_argument_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string first, string second) => " + invocation + @"(first + second);
}
");

        protected override string GetDiagnosticId() => MiKo_3248_UseLiteralForThrowIfCheckAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3248_UseLiteralForThrowIfCheckAnalyzer();
    }
}
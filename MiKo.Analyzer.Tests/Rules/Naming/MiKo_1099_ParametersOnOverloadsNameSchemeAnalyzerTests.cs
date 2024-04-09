using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1099_ParametersOnOverloadsNameSchemeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_with_no_methods() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_single_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_differently_named_methods() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i) { }

    public void DoSomethingElse(int j) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_2_similar_named_methods_if_1st_has_no_parameters() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_4_similar_named_methods_if_all_follow_naming_scheme() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }

    public void DoSomething(int i, int j) { }

    public void DoSomething(int i, int j, int k) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_similar_named_methods_if_parameters_types_differ() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i, int j) { }

    public void DoSomething(int i, float k, int j) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_similar_named_methods_if_parameters_got_mixed_in() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int a, int b, int c) { }

    public void DoSomething(int a, float x, int b, float y, int c, float z) { }
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_similar_named_methods_if_follow_up_parameters_all_differ() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(int a, int b, int c) { }

    public void DoSomething(int a, IEnumerable<int> other) { }
}
");

        [Test]
        public void No_issue_is_reported_for_methods_with_different_parameters() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public interface IDoSomething
{
    Task<TResult> DoSomething<TResult>(Func<TResult> callback, CultureInfo culture, CancellationToken cancellationToken);

    Task<TResult> DoSomething<TResult>(Func<TResult> callback, CultureInfo culture);

    Task DoSomething(Action action, CultureInfo culture);
}");

        [Test]
        public void No_issue_is_reported_for_DoSomething_1() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this string value, char c) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_2() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this ReadOnlySpan<char> value, char c) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_3() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this ReadOnlySpan<char> value, string finding) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_4() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this string value, string finding, StringComparison comparison) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_5() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this ReadOnlySpan<char> value, string finding, StringComparison comparison) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_6() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_1_1() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this string value, char c) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_2_1() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this ReadOnlySpan<char> value, char c) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_3_1() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this ReadOnlySpan<char> value, string finding) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_4_1() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this string value, string finding, StringComparison comparison) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_DoSomething_5_1() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(this ReadOnlySpan<char> value, string finding, StringComparison comparison) => false;

    public static bool DoSomething(this ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_different_parameter_types_but_same_amount() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(int i, string s) => false;

    public static bool DoSomething(string value, string s) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_methods_where_overload_parameter_has_number_as_suffix() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(int value) => false;

    public static bool DoSomething(int value1, int value2) => false;
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_similar_named_methods_if_parameters_names_are_swapped() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i, int j) { }

    public void DoSomething(int i, int k, int j) { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_type_with_similar_named_methods_if_other_parameters_got_mixed_in_but_order_of_parameters_changed() => An_issue_is_reported_for(2, @"
public class TestMe
{
    public void DoSomething(int a, int b, int c) { }

    public void DoSomething(int a, float x, int c, float z, int b, float y) { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_type_with_similar_named_methods_if_parameters_are_named_differently() => An_issue_is_reported_for(3, @"
public class TestMe
{
    public void DoSomething(int x, int y, int z) { }

    public void DoSomething(int a, int b, int c, int d) { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_type_with_similar_named_methods_if_all_parameters_got_messed_up() => An_issue_is_reported_for(3, @"
public class TestMe
{
    public void DoSomething(int c, int b, int a) { }

    public void DoSomething(int a, int c, int b, float x, float y, float z) { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_type_with_similar_named_methods_if_all_3_parameters_got_reversed() => An_issue_is_reported_for(2, @"
public class TestMe
{
    public void DoSomething(int c, int b, int a) { }

    public void DoSomething(int a, int b, int c, int d) { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_type_with_similar_named_methods_if_all_4_parameters_got_reversed() => An_issue_is_reported_for(4, @"
public class TestMe
{
    public void DoSomething(int d, int c, int b, int a) { }

    public void DoSomething(int a, int b, int c, int d, int e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_methods_where_overload_parameter_has_number_as_suffix_but_was_switched() => An_issue_is_reported_for(@"
public class TestMe
{
    public static bool DoSomething(int value2) => false;

    public static bool DoSomething(int value1, int value2) => false;
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
public class TestMe
{
    public static bool DoSomething(int value) => false;

    public static bool DoSomething(int i, string s) => false;
}
";

            const string FixedCode = @"
public class TestMe
{
    public static bool DoSomething(int i) => false;

    public static bool DoSomething(int i, string s) => false;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1099_ParametersOnOverloadsNameSchemeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1099_ParametersOnOverloadsNameSchemeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1099_CodeFixProvider();
    }
}
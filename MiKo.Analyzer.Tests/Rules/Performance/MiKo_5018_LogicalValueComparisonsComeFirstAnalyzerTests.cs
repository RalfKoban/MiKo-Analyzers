using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5018_LogicalValueComparisonsComeFirstAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_comparison_if_boolean_flag_comes_first() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool flag, int i) => flag && i == 42;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_comparison_if_string_comparison_comes_first() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(string s, int i) => s == ""whatever"" && i == 42;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_comparison() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int i) => i == 42 || i == 0815;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_pattern_matching_value_type_comparison() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Value { get; }

    public bool DoSomething(object o) => o is TestMe t && t.Value == 42;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_multiple_null_checks_and_value_type_comparison() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Nested { get }
    public int Value { get; }

    public bool DoSomething(TestMe arg) => arg != null && arg.Nested != null && arg.Nested.Value == 42;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_conditional_null_checks_and_value_type_comparison() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Nested { get }
    public int Value { get; }

    public bool DoSomething(TestMe arg) => arg != null && arg.Nested?.Value == 42;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_array_content_checks_via_value_type_comparison() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int[] values)
    {
        if (values.Length == 8
         && values[0] == 0
         && values[1] == 1
         && values[2] == 2
         && values[3] == 3
         && values[4] == 4
         && values[5] == 5
         && values[6] == 6
         && values[7] == 7)
        {
            return true;
        }

        return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_comparison_if_constants_come_first() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int i) => 42 == i || 0815 == i;
}
");

        [Test]
        public void No_issue_is_reported_for_public_static_readonly_field_comparison_when_field_comes_first() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private static void SetOwnerWindow(IntPtr intPtrOwned, IntPtr intPtrOwner)
    {
        if (IntPtr.Zero == intPtrOwned || IntPtr.Zero == intPtrOwner) return;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_public_static_readonly_field_field_comparison_when_field_comes_last() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private static void SetOwnerWindow(IntPtr intPtrOwned, IntPtr intPtrOwner)
    {
        if (intPtrOwned == IntPtr.Zero || intPtrOwner == IntPtr.Zero) return;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_and_reference_type_OR_comparison_if_value_type_comparison_comes_first() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int i, object o) => i == 42 || o.ToString() == ""whatever"";
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_and_array_type_AND_comparison_if_value_type_comparison_comes_first() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => i == 42 && array.SequenceEqual(Array.Empty<int>());
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => i == 42 || array.SequenceEqual(Array.Empty<int>());
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_NotEquals_Null_expression_with_null_on_left_side() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => null != other && Value == other.Value && Data == other.Data;
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_NotEquals_Null_expression_with_null_on_right_side() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other != null && Value == other.Value && Data == other.Data;
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_Equals_Null_expression() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool SomeComparison(TestMe other) => other == null || (Value != other.Value && Data != other.Data);
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_IsNotNull_pattern_expression() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other is not null && Value == other.Value && Data == other.Data;
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_IsNull_pattern_expression() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool SomeComparison(TestMe other) => other is null || (Value != other.Value && Data != other.Data);
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_enum_OR_comparison() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public StringComparison Comparison { get; set; }

    public Guid Id { get; set; }

    public bool SomeComparison(TestMe other) => other.Id == Guid.Empty && (other.Comparison == StringComparison.Ordinal || other.Comparison == StringComparison.OrdinalIgnoreCase);
}
");

        [Test]
        public void No_issue_is_reported_for_parameters_only_value_type_comparisons() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static bool SomeComparison(Guid guid1, Guid guid2, Guid guid3, Guid guid4) => guid1 == guid3 && guid2 == guid4;
}
");

        [Test]
        public void No_issue_is_reported_for_CancellationToken_IsCancellationRequested_call() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public static bool SomeComparison(CancellationToken token, int value) => token.IsCancellationRequested || value == -1;
}
");

        [Test]
        public void No_issue_is_reported_for_version_comparison() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public static bool SomeComparison(Version version) => version.Major == 0 && version.Minor == 0 && version.Build == 0 && version.Revision == 0;
}
");

        [Test]
        public void No_issue_is_reported_for_version_comparison_when_numbers_are_on_left_side() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public static bool SomeComparison(Version version) => 0 == version.Major && 0 == version.Minor && 0 == version.Build && 0 == version.Revision;
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_array_content_checks_via_value_type_comparison_and_additional_value_type_comparison() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int i, int[] values)
    {
        if (values.Length == 8
         && values[0] == 0
         && values[1] == 1
         && values[2] == 2
         && values[3] == 3
         && values[4] == 4
         && values[5] == 5
         && values[6] == 6
         && values[7] == 7
         && i == 42)
        {
            return true;
        }

        return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_value_type_and_array_type_AND_comparison_if_array_type_comparison_comes_first() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => array.SequenceEqual(Array.Empty<int>()) && i == 42;
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_value_type_and_array_type_OR_comparison_if_array_type_comparison_comes_first() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => array.SequenceEqual(Array.Empty<int>()) || i == 42;
}
");

        [Test]
        public void An_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_reference_type_comparison_comes_first_and_NotEquals_Null_expression() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other != null && Data == other.Data && Value == other.Value;
}
");

        [Test]
        public void An_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_reference_type_comparison_comes_first_and_IsNotNull_pattern_expression() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other is not null && Data == other.Data && Value == other.Value;
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_parenthesized_value_type_and_collection_type_comparison() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public int Id { get; set; }
    public List<Data> Data { get; set; }

    public bool Equals(TestMe other)
    {
        return ((Data == other.Data) || (Data != null && Data.SequenceEqual(otherData))) && Id == other.Id;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_value_type_and_reference_type_OR_comparison_if_string_invocation_comes_first() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int i, object o) => o.ToString() == ""whatever"" || i == 42;
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_reference_type_comparison_and_multiple_null_checks_and_value_type_comparison() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Nested { get }
    public int Value { get; }

    public bool DoSomething(TestMe arg, object o) => o.ToString() == ""whatever"" && arg != null && arg.Nested != null && arg.Nested.Value == 42;
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_conditional_value_type_comparison_and_reference_type_comparison_if_string_invocation_comes_first() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Nested { get }
    public int Value { get; }

    public bool DoSomething(TestMe arg, object o) => o.ToString() == ""whatever"" && arg?.Nested?.Value == 42;
}
");

        [Test]
        public void Code_gets_fixed_for_class_with_value_type_and_array_type_AND_comparison_if_array_type_comparison_comes_first()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => array.SequenceEqual(Array.Empty<int>()) && i == 42;
}
";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => i == 42 && array.SequenceEqual(Array.Empty<int>());
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_value_type_and_array_type_OR_comparison_if_array_type_comparison_comes_first()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => array.SequenceEqual(Array.Empty<int>()) || i == 42;
}
";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => i == 42 || array.SequenceEqual(Array.Empty<int>());
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_class_with_value_type_and_array_type_OR_comparison_if_reference_type_comparison_comes_first_and_NotEquals_Null_expression()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other != null && Data == other.Data && Value == other.Value;
}
";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other != null && Value == other.Value && Data == other.Data;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_class_with_value_type_and_array_type_OR_comparison_if_reference_type_comparison_comes_first_and_IsNotNull_pattern_expression()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other is not null && Data == other.Data && Value == other.Value;
}
";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other is not null && Value == other.Value && Data == other.Data;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_class_with_value_type_and_array_type_OR_comparison_in_if_clause_if_reference_type_comparison_comes_first_and_IsNotNull_pattern_expression()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other)
    {
        if (other is not null && Data == other.Data && Value == other.Value)
            return true;

        return false;
    }
}
";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other)
    {
        if (other is not null && Value == other.Value && Data == other.Data)
            return true;

        return false;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_parenthesized_value_type_and_collection_type_comparison()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public int Id { get; set; }
    public List<Data> Data { get; set; }

    public bool Equals(TestMe other)
    {
        return ((Data == other.Data) || (Data != null && Data.SequenceEqual(otherData))) && Id == other.Id;
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public int Id { get; set; }
    public List<Data> Data { get; set; }

    public bool Equals(TestMe other)
    {
        return Id == other.Id && ((Data == other.Data) || (Data != null && Data.SequenceEqual(otherData)));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_parenthesized_value_type_and_collection_type_comparison_when_spanning_multiple_lines()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public List<Guid> Guids { get; set; }
    public List<object> Objects { get; set; }

    public bool Equals(TestMe other)
    {
        if (Guids.Count > 0 &&
            Guids.All(_ => _ != Guid.Empty) &&
            Objects.Count == 1 &&
            Objects.First() != null)
        {
            return true;
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public List<Guid> Guids { get; set; }
    public List<object> Objects { get; set; }

    public bool Equals(TestMe other)
    {
        if (Guids.Count > 0 &&
            Objects.Count == 1 &&
            Guids.All(_ => _ != Guid.Empty) &&
            Objects.First() != null)
        {
            return true;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_array_content_checks_via_value_type_comparison_and_additional_value_type_comparison()
        {
            const string OriginalText = @"
using System;

public class TestMe
{
    public bool DoSomething(int i, int[] values)
    {
        if (values.Length == 8
         && values[0] == 0
         && values[1] == 1
         && values[2] == 2
         && values[3] == 3
         && values[4] == 4
         && values[5] == 5
         && values[6] == 6
         && values[7] == 7
         && i == 42)
        {
            return true;
        }

        return false;
    }
}
";

            const string FixedText = @"
using System;

public class TestMe
{
    public bool DoSomething(int i, int[] values)
    {
        if (i == 42
         && values.Length == 8
         && values[0] == 0
         && values[1] == 1
         && values[2] == 2
         && values[3] == 3
         && values[4] == 4
         && values[5] == 5
         && values[6] == 6
         && values[7] == 7)
        {
            return true;
        }

        return false;
    }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_reference_type_comparison_and_multiple_null_checks_and_value_type_comparison()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public TestMe Nested { get }
    public int Value { get; }

    public bool DoSomething(TestMe arg, object o) => o.ToString() == ""whatever"" && arg != null && arg.Nested != null && arg.Nested.Value == 42;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public TestMe Nested { get }
    public int Value { get; }

    public bool DoSomething(TestMe arg, object o) => arg != null && arg.Nested != null && arg.Nested.Value == 42 && o.ToString() == ""whatever"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_conditional_value_type_comparison_and_reference_type_comparison_if_string_invocation_comes_first()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public TestMe Nested { get }
    public int Value { get; }

    public bool DoSomething(TestMe arg, object o) => o.ToString() == ""whatever"" && arg?.Nested?.Value == 42;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public TestMe Nested { get }
    public int Value { get; }

    public bool DoSomething(TestMe arg, object o) => arg?.Nested?.Value == 42 && o.ToString() == ""whatever"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_conditional_value_type_comparison_and_boolean_method_calls_if_method_calls_come_first()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison Comparison { get }

    public bool DoSomething(TestMe t) => IsSomething(t) && FindSomething(t) == t && t.Comparison == StringComparison.Ordinal;

    public bool IsSomething(object o) => true;

    public object FindSomething(object o) => o;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison Comparison { get }

    public bool DoSomething(TestMe t) => t.Comparison == StringComparison.Ordinal && IsSomething(t) && FindSomething(t) == t;

    public bool IsSomething(object o) => true;

    public object FindSomething(object o) => o;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_5018_LogicalValueComparisonsComeFirstAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5018_LogicalValueComparisonsComeFirstAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5018_CodeFixProvider();
    }
}